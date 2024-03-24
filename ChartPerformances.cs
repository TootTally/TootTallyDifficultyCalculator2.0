﻿using System;
using System.Net.Mime;

namespace TootTallyDifficultyCalculator2._0
{
    public class ChartPerformances
    {

        public const float CHEESABLE_THRESHOLD = 34.375f;


        public List<DataVector>[] aimPerfMatrix;
        public DataVectorAnalytics[] aimAnalyticsArray;

        public List<DataVector>[] tapPerfMatrix;
        public DataVectorAnalytics[] tapAnalyticsArray;

        public float[] aimRatingArray;
        public float[] tapRatingArray;
        private TMBChart _chart;
        private readonly int NOTE_COUNT;

        public ChartPerformances(TMBChart chart)
        {
            aimPerfMatrix = new List<DataVector>[7];
            tapPerfMatrix = new List<DataVector>[7];
            aimRatingArray = new float[7];
            tapRatingArray = new float[7];
            aimAnalyticsArray = new DataVectorAnalytics[7];
            tapAnalyticsArray = new DataVectorAnalytics[7];

            var length = chart.notes.Length;
            for (int i = 0; i < chart.GAME_SPEED.Length; i++)
            {
                aimPerfMatrix[i] = new List<DataVector>(length);
                tapPerfMatrix[i] = new List<DataVector>(length);
            }
            _chart = chart;
            NOTE_COUNT = _chart.notesDict[0].Count;
        }

        public void CalculatePerformances(int speedIndex)
        {
            var noteList = _chart.notesDict[speedIndex];
            var aimEndurance = 0f;
            var tapEndurance = 0f;

            for (int i = 0; i < NOTE_COUNT; i++) //Main Forward Loop
            {
                var currentNote = noteList[i];
                int noteCount = 0;
                float weightSum = 0f;
                var aimStrain = 0f;
                var tapStrain = 0f;
                for (int j = i - 1; j > 0 && /*j > i - 32 &&*/ MathF.Abs(currentNote.position - noteList[j].position) <= 4f; j--)
                {
                    var prevNote = noteList[j];
                    var nextNote = noteList[j + 1];
                    var weight = FastPow(.85f, noteCount);
                    noteCount++;
                    if (weight < 0.0001f) continue;

                    weightSum += weight;

                    var deltaTime = nextNote.position - (prevNote.position + prevNote.length);
                    var lengthSum = prevNote.length;
                    var deltaSlide = MathF.Abs(prevNote.pitchDelta);

                    while (IsSlider(deltaTime))
                    {
                        if (j-- <= 0)
                            break;
                        prevNote = noteList[j];
                        nextNote = noteList[j + 1];
                        deltaTime = nextNote.position - (prevNote.position + prevNote.length);

                        lengthSum += deltaTime == 0 ? MathF.Sqrt(prevNote.length) : prevNote.length;
                        deltaSlide += MathF.Abs(prevNote.pitchDelta);
                    }

                    if (deltaSlide != 0)
                    {
                        //Acc Calc
                        aimStrain += ComputeStrain(CalcAccStrain(lengthSum, deltaSlide, weight)) / MainForm.Instance.GetAccNum();
                        aimEndurance += CalcAccEndurance(lengthSum, deltaSlide, weight);
                    }

                    //Aim Calc
                    deltaTime += lengthSum * .4f;
                    var aimDistance = MathF.Abs(nextNote.pitchStart - prevNote.pitchEnd);
                    var noteMoved = aimDistance != 0 || deltaSlide != 0;

                    if (noteMoved)
                    {
                        aimStrain += ComputeStrain(CalcAimStrain(aimDistance, weight, deltaTime)) / MainForm.Instance.GetAimNum();
                        aimEndurance += CalcAimEndurance(aimDistance, weight, deltaTime);
                    }

                    //Tap Calc
                    var tapDelta = nextNote.position - prevNote.position;

                    tapStrain += ComputeStrain(CalcTapStrain(tapDelta, weight)) / MainForm.Instance.GetTapNum();
                    tapEndurance += CalcTapEndurance(tapDelta, weight);
                }

                var enduranceDecay = MainForm.Instance.GetEndDrain();
                if (i > 0)
                {
                    var aimThreshold = MathF.Pow(aimStrain, 1.6f) * 5f;
                    var tapThreshold = MathF.Pow(tapStrain, 1.6f) * 5f;
                    if (aimEndurance >= aimThreshold)
                        ComputeEnduranceDecay(ref aimEndurance, enduranceDecay, (aimEndurance - aimThreshold) / 50f);
                    if (tapEndurance >= tapThreshold)
                        ComputeEnduranceDecay(ref tapEndurance, enduranceDecay, (tapEndurance - tapThreshold) / 50f);
                }

                aimPerfMatrix[speedIndex].Add(new DataVector(currentNote.position, aimStrain, aimEndurance, noteCount + weightSum));
                tapPerfMatrix[speedIndex].Add(new DataVector(currentNote.position, tapStrain, tapEndurance, noteCount + weightSum));
            }
        }
        public static bool IsSlider(float deltaTime) => !(MathF.Round(deltaTime, 3) > 0);

        public static void ComputeEnduranceDecay(ref float endurance, float enduranceDecay, float distanceFromLastNote)
        {
            endurance /= 1 + MainForm.Instance.GetEndDrainExtra() * distanceFromLastNote;
        }

        //https://www.desmos.com/calculator/e4kskdn8mu
        public static float ComputeStrain(float strain) => a * MathF.Pow(strain + 1, -.012f * MathF.E) - a - (4f * strain) / a;
        private const float a = -50f;
        #region AIM
        public static float CalcAimStrain(float distance, float weight, float deltaTime)
        {
            var speed = distance / MathF.Pow(deltaTime, 1.25f);
            return speed * weight;
        }

        public static float CalcAimEndurance(float distance, float weight, float deltaTime)
        {
            var speed = (distance / MathF.Pow(deltaTime, 1.02f)) / (MainForm.Instance.GetAimEndNote() * MainForm.Instance.GetAimEndMult());
            return speed * weight;
        }
        #endregion

        #region TAP
        public static float CalcTapStrain(float tapDelta, float weight)
        {
            return (11f / MathF.Pow(tapDelta, 1.25f)) * weight;
        }

        public static float CalcTapEndurance(float tapDelta, float weight)
        {
            return (1.1f / MathF.Pow(tapDelta, 1.02f)) / (MainForm.Instance.GetTapEndMult() * MainForm.Instance.GetAimEndMult()) * weight;
        }
        #endregion

        #region ACC
        public static float CalcAccStrain(float lengthSum, float slideDelta, float weight)
        {
            var speed = slideDelta / MathF.Pow(lengthSum, 1.25f);
            return speed * weight;
        }

        public static float CalcAccEndurance(float lengthSum, float slideDelta, float weight)
        {
            var speed = (slideDelta / MathF.Pow(lengthSum, 1.02f)) / (MainForm.Instance.GetAimEndSlider() * MainForm.Instance.GetAimEndMult());
            return speed * weight;
        }
        #endregion

        public void CalculateAnalytics(int gamespeed)
        {
            aimAnalyticsArray[gamespeed] = new DataVectorAnalytics(aimPerfMatrix[gamespeed]);
            tapAnalyticsArray[gamespeed] = new DataVectorAnalytics(tapPerfMatrix[gamespeed]);

        }

        public void CalculateRatings(int gamespeed)
        {
            aimRatingArray[gamespeed] = aimAnalyticsArray[gamespeed].perfWeightedAverage + 0.01f;
            tapRatingArray[gamespeed] = tapAnalyticsArray[gamespeed].perfWeightedAverage + 0.01f;
        }

        public float GetDynamicAimRating(float percent, float speed) => GetDynamicSkillRating(percent, speed, aimPerfMatrix);
        public float GetDynamicTapRating(float percent, float speed) => GetDynamicSkillRating(percent, speed, tapPerfMatrix);

        private float GetDynamicSkillRating(float percent, float speed, List<DataVector>[] skillRatingMatrix)
        {
            var index = (int)((speed - 0.5f) / .25f);

            if (skillRatingMatrix[index].Count <= 1 || percent <= 0)
                return 0;
            else if (speed % .25f == 0)
                return CalcSkillRating(percent, skillRatingMatrix[index]);

            var r1 = CalcSkillRating(percent, skillRatingMatrix[index]);
            var r2 = CalcSkillRating(percent, skillRatingMatrix[index + 1]);

            var minSpeed = _chart.GAME_SPEED[index];
            var maxSpeed = _chart.GAME_SPEED[index + 1];
            var by = (speed - minSpeed) / (maxSpeed - minSpeed);
            return Lerp(r1, r2, by);
        }

        private float CalcSkillRating(float percent, List<DataVector> skillRatingArray)
        {
            if (skillRatingArray.Count <= 1) return .01f;

            int maxRange;
            var mAcc = Math.Clamp(MainForm.Instance.GetMacc(), .01, 1);
            var map = Math.Clamp(MainForm.Instance.GetMap(), .01, 1);
            if (percent <= mAcc)
                maxRange = (int)Math.Clamp(skillRatingArray.Count * (percent * (map / mAcc)), 1, skillRatingArray.Count);
            else
                maxRange = (int)Math.Clamp(skillRatingArray.Count * ((percent - mAcc) * ((1f - map) / (1f - mAcc)) + map), 1, skillRatingArray.Count);

            List<DataVector> array = skillRatingArray.OrderBy(x => x.performance).ToList().GetRange(0, maxRange);
            var analytics = new DataVectorAnalytics(array);
            return analytics.perfWeightedAverage + .01f;
        }

        public const float AIM_WEIGHT = 1.2f;
        public const float TAP_WEIGHT = 1.15f;

        public static readonly float[] HDWeights = { .11f, .1f };
        public static readonly float[] FLWeights = { .18f, .06f };

        public float GetDynamicDiffRating(float percent, float gamespeed, string[] modifiers = null)
        {
            var aimRating = GetDynamicAimRating(percent, gamespeed);
            var tapRating = GetDynamicTapRating(percent, gamespeed);

            if (aimRating == 0 && tapRating == 0) return 0f;

            if (modifiers != null)
            {
                var aimPow = 1f;
                var tapPow = 1f;

                if (modifiers.Contains("HD"))
                {
                    aimPow += HDWeights[0];
                    tapPow += HDWeights[1];
                }
                if (modifiers.Contains("FL"))
                {
                    aimPow += FLWeights[0];
                    tapPow += FLWeights[1];
                }

                aimRating = MathF.Pow(aimRating + 1f, aimPow) - 1f;
                tapRating = MathF.Pow(tapRating + 1f, tapPow) - 1f;
            }

            var totalRating = aimRating + tapRating;
            var aimPerc = aimRating / totalRating;
            var tapPerc = tapRating / totalRating;
            var aimWeight = (aimPerc + MainForm.Instance.GetBiasMult()) * AIM_WEIGHT;
            var tapWeight = (tapPerc + MainForm.Instance.GetBiasMult()) * TAP_WEIGHT;
            var totalWeight = aimWeight + tapWeight;

            return ((aimRating * aimWeight) + (tapRating * tapWeight)) / totalWeight;
        }

        public static float Lerp(float firstFloat, float secondFloat, float by) //Linear easing
        {
            return firstFloat + (secondFloat - firstFloat) * by;
        }

        public static float FastPow(double num, int exp)
        {
            double result = 1.0;
            while (exp > 0)
            {
                if (exp % 2 == 1)
                    result *= num;
                exp >>= 1;
                num *= num;
            }
            return (float)result;
        }

        public class DataVector
        {
            public float performance;
            public float endurance;
            public float time;
            public float weight;

            public DataVector(float time, float performance, float endurance, float weight)
            {
                this.time = time;
                this.performance = performance;
                this.endurance = endurance;
                this.weight = weight;
            }

        }

        public class DataVectorAnalytics
        {
            public float perfMax = 0f, perfMin = 0f, perfSum = 0f, perfWeightedAverage = 0f;
            public float weightSum = 0f;

            public DataVectorAnalytics(List<DataVector> dataVectorList)
            {
                if (dataVectorList.Count > 0)
                {
                    CalculateWeightSum(dataVectorList);
                    CalculateData(dataVectorList);
                }
            }

            public void CalculateWeightSum(List<DataVector> dataVectorList) => weightSum = dataVectorList.Sum(x => x.weight);

            public void CalculateData(List<DataVector> dataVectorList)
            {
                for (int i = 0; i < dataVectorList.Count; i++)
                {
                    if (dataVectorList[i] == null)
                        continue;

                    if (dataVectorList[i].performance > perfMax)
                        perfMax = dataVectorList[i].performance;
                    else if (dataVectorList[i].performance < perfMin)
                        perfMin = dataVectorList[i].performance;

                    perfSum += (dataVectorList[i].performance + dataVectorList[i].endurance) * (dataVectorList[i].weight / weightSum);
                }
                perfWeightedAverage = perfSum;
            }
        }

        public class SerializableDiffData
        {
            public SerializableDataVector[] data;

            public SerializableDiffData(ChartPerformances performances)
            {
                data = new SerializableDataVector[]
                {
                    new SerializableDataVector() { speed = .5f, aim = performances.aimPerfMatrix[0], tap = performances.tapPerfMatrix[0]},
                    new SerializableDataVector() { speed = .75f,aim = performances.aimPerfMatrix[1], tap = performances.tapPerfMatrix[1]},
                    new SerializableDataVector() { speed = 1f,aim = performances.aimPerfMatrix[2], tap = performances.tapPerfMatrix[2]},
                    new SerializableDataVector() { speed = 1.25f,aim = performances.aimPerfMatrix[3], tap = performances.tapPerfMatrix[3]},
                    new SerializableDataVector() { speed = 1.50f,aim = performances.aimPerfMatrix[4], tap = performances.tapPerfMatrix[4]},
                    new SerializableDataVector() { speed = 1.75f,aim = performances.aimPerfMatrix[5], tap = performances.tapPerfMatrix[5]},
                    new SerializableDataVector() { speed = 2f,aim = performances.aimPerfMatrix[6], tap = performances.tapPerfMatrix[6]},
                };
            }
        }

        public class SerializableDataVector
        {
            public float speed;
            public List<DataVector> aim;
            public List<DataVector> tap;
            public List<DataVector> acc;
        }
    }

}
