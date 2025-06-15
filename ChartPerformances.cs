using System;
using System.Diagnostics;
using System.Net.Mime;
using System.Runtime.InteropServices;

namespace TootTallyDifficultyCalculator2._0
{
    public class ChartPerformances : IDisposable
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

            for (int i = 0; i < chart.GAME_SPEED.Length; i++)
            {
                aimPerfMatrix[i] = new List<DataVector>(chart.noteCount);
                tapPerfMatrix[i] = new List<DataVector>(chart.noteCount);
            }
            _chart = chart;
            NOTE_COUNT = _chart.notesDict[0].Count;
        }

        public readonly float AIM_DIV = MainForm.Instance.GetAimNum();
        public readonly float TAP_DIV = MainForm.Instance.GetTapNum();
        public readonly float ACC_DIV = MainForm.Instance.GetAccNum();
        public readonly float AIM_END = MainForm.Instance.GetAimEndNote();
        public readonly float TAP_END = MainForm.Instance.GetTapEndMult();
        public readonly float ACC_END = MainForm.Instance.GetAimEndSlider();
        public readonly float MUL_END = MainForm.Instance.GetAimEndMult();
        public readonly float MAX_DIST = 8f;

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
                for (int j = i - 1; j >= 0 && noteCount < 64 && (MathF.Abs(currentNote.position - noteList[j].position) <= MAX_DIST || i - j <= 2); j--)
                {
                    var prevNote = noteList[j];
                    var nextNote = noteList[j + 1];
                    if (prevNote.position >= nextNote.position) break;

                    var weight = MainForm.WEIGHTS[noteCount];
                    noteCount++;
                    weightSum += weight;

                    var lengthSum = prevNote.length;
                    var deltaSlideSum = MathF.Abs(prevNote.pitchDelta);
                    if (deltaSlideSum <= CHEESABLE_THRESHOLD)
                        deltaSlideSum *= .15f;
                    while (prevNote.isSlider) //Merge all sliders into one note
                    {
                        if (j-- <= 0)
                            break;
                        prevNote = noteList[j];
                        nextNote = noteList[j + 1];

                        if (prevNote.pitchDelta == 0)
                            lengthSum += prevNote.length * .85f;
                        else
                        {
                            var deltaSlide = MathF.Abs(prevNote.pitchDelta);
                            lengthSum += prevNote.length;
                            if (deltaSlide <= CHEESABLE_THRESHOLD)
                                deltaSlide *= .15f;
                            deltaSlideSum += deltaSlide /* MathF.Sqrt(sliderCount)*/;
                        }
                    }
                    var deltaTime = nextNote.position - prevNote.position;
                    if (deltaSlideSum != 0)
                    {
                        //Acc Calc
                        aimStrain += CalcAccStrain(lengthSum, deltaSlideSum, weight) / ACC_DIV;
                        aimEndurance += CalcAccEndurance(lengthSum, deltaSlideSum, weight);
                    }

                    //Aim Calc
                    var aimDistance = MathF.Abs(nextNote.pitchStart - prevNote.pitchEnd);
                    var noteMoved = aimDistance != 0 || deltaSlideSum != 0;

                    if (noteMoved)
                    {
                        aimStrain += CalcAimStrain(aimDistance, weight, deltaTime);
                        aimEndurance += CalcAimEndurance(aimDistance, weight, deltaTime);
                    }

                    //Tap Calc
                    tapStrain += CalcTapStrain(deltaTime, weight, aimDistance);
                    tapEndurance += CalcTapEndurance(deltaTime, weight, aimDistance);
                }
                aimStrain = ComputeStrain(aimStrain) / AIM_DIV;
                tapStrain = ComputeStrain(tapStrain) / TAP_DIV;
                if (i > 0)
                {
                    var endDivider = 61f - MathF.Min(currentNote.position - noteList[i - 1].position, 5f) * 12f;
                    var aimThreshold = MathF.Sqrt(aimStrain) * 1.5f;//MathF.Pow(aimStrain, 1.08f) * 1.2f;
                    var tapThreshold = MathF.Sqrt(tapStrain) * 5f;//MathF.Pow(tapStrain, 1.08f) * 1.2f;
                    if (aimEndurance >= aimThreshold)
                        ComputeEnduranceDecay(ref aimEndurance, (aimEndurance - aimThreshold) / endDivider);
                    if (tapEndurance >= tapThreshold)
                        ComputeEnduranceDecay(ref tapEndurance, (tapEndurance - tapThreshold) / endDivider);
                }

                aimPerfMatrix[speedIndex].Add(new DataVector(currentNote.position, aimStrain, aimEndurance, weightSum));
                tapPerfMatrix[speedIndex].Add(new DataVector(currentNote.position, tapStrain, tapEndurance, weightSum));
            }
        }
        //public static bool IsSlider(float deltaTime) => !(MathF.Round(deltaTime, 3) > 0);

        public static void ComputeEnduranceDecay(ref float endurance, float distanceFromLastNote)
        {
            endurance /= 1 + (MainForm.Instance.GetEndDrainExtra() * distanceFromLastNote);
        }

        //https://www.desmos.com/calculator/tkunxszosp
        //public static float ComputeStrain(float strain) => a * MathF.Pow(strain + 1, -.04f * MathF.E) - a - (2.5f * strain / a);
        public float ComputeStrain(float strain) => a * MathF.Pow(strain + 1, b * MathF.E) - a - (MathF.Pow(strain,p) / a);
        private readonly float a = -MainForm.Instance.GetStrainA();
        private readonly float b = -MainForm.Instance.GetStrainB();
        private readonly float p = MainForm.Instance.GetStrainP();

        #region AIM
        public float CalcAimStrain(float distance, float weight, float deltaTime)
        {
            var speed = MathF.Sqrt(distance + 50) * .75f / MathF.Pow(deltaTime, 1.38f);
            return speed * weight;
        }

        public float CalcAimEndurance(float distance, float weight, float deltaTime)
        {
            var speed = (MathF.Sqrt(distance + 50) * .25f / MathF.Pow(deltaTime, 1.08f)) / (AIM_END * MUL_END);
            return speed * weight;
        }
        #endregion

        #region TAP
        public static float CalcTapStrain(float tapDelta, float weight, float aimDistance)
        {
            var baseValue = MathF.Min(Lerp(3.25f, 5.5f, aimDistance / CHEESABLE_THRESHOLD), 6f);
            //var baseValue = aimDistance <= CHEESABLE_THRESHOLD ? 8f : 16f;
            return (baseValue / MathF.Pow(tapDelta, 1.38f)) * weight;
        }

        public float CalcTapEndurance(float tapDelta, float weight, float aimDistance)
        {
            var baseValue = MathF.Min(Lerp(.14f, .20f, aimDistance / CHEESABLE_THRESHOLD), .25f);
            //var baseValue = aimDistance <= CHEESABLE_THRESHOLD ? .25f : .65f;
            return (baseValue / MathF.Pow(tapDelta, 1.12f)) / (TAP_END * MUL_END) * weight;
        }
        #endregion

        #region ACC
        public static float CalcAccStrain(float lengthSum, float slideDelta, float weight)
        {
            var speed = slideDelta * 5f / MathF.Pow(lengthSum, 1.16f);
            return speed * weight;
        }

        public float CalcAccEndurance(float lengthSum, float slideDelta, float weight)
        {
            var speed = slideDelta * .75f / MathF.Pow(lengthSum, 1.08f) / (ACC_END * MUL_END);
            return speed * weight;
        }
        #endregion

        public void CalculateAnalytics(int gamespeed, float songLengthMult = 1)
        {
            aimAnalyticsArray[gamespeed] = new DataVectorAnalytics(aimPerfMatrix[gamespeed], songLengthMult);
            tapAnalyticsArray[gamespeed] = new DataVectorAnalytics(tapPerfMatrix[gamespeed], songLengthMult);

        }

        public void CalculateRatings(int gamespeed)
        {
            aimRatingArray[gamespeed] = aimAnalyticsArray[gamespeed].perfWeightedAverage + 0.01f;
            tapRatingArray[gamespeed] = tapAnalyticsArray[gamespeed].perfWeightedAverage + 0.01f;
        }

        public float GetDynamicAimRating(int hitCount, float speed) => GetDynamicSkillRating(hitCount, speed, aimPerfMatrix);
        public float GetDynamicTapRating(int hitCount, float speed) => GetDynamicSkillRating(hitCount, speed, tapPerfMatrix);

        private float GetDynamicSkillRating(int hitCount, float speed, List<DataVector>[] skillRatingMatrix)
        {
            var index = (int)((speed - 0.5f) / .25f);

            if (skillRatingMatrix[index].Count <= 1 || hitCount <= 0)
                return 0;
            else if (speed % .25f == 0)
                return CalcSkillRating(hitCount, skillRatingMatrix[index]);

            var r1 = CalcSkillRating(hitCount, skillRatingMatrix[index]);
            var r2 = CalcSkillRating(hitCount, skillRatingMatrix[index + 1]);

            var minSpeed = _chart.GAME_SPEED[index];
            var maxSpeed = _chart.GAME_SPEED[index + 1];
            var by = (speed - minSpeed) / (maxSpeed - minSpeed);
            return Lerp(r1, r2, by);
        }

        public readonly float MACC = Math.Clamp(MainForm.Instance.GetMacc(), .01f, 1);
        public readonly float MAP = Math.Clamp(MainForm.Instance.GetMap(), .01f, 1);

        private float CalcSkillRating(int hitCount, List<DataVector> skillRatingArray)
        {
            if (skillRatingArray.Count <= 1) return .01f;


            int maxRange;
            float percent = 1f;
            if (hitCount < _chart.noteCount)
                percent = (float)hitCount / _chart.noteCount;

            if (percent <= MACC)
                maxRange = (int)Math.Clamp(skillRatingArray.Count * (percent * (MAP / MACC)), 1, skillRatingArray.Count);
            else
                maxRange = (int)Math.Clamp(skillRatingArray.Count * ((percent - MACC) * ((1f - MAP) / (1f - MACC)) + MAP), 1, skillRatingArray.Count);
            List<DataVector> array = skillRatingArray.OrderBy(x => x.performance + x.endurance).ToList().GetRange(0, maxRange);
            var analytics = new DataVectorAnalytics(array, _chart.songLengthMult);
            return analytics.perfWeightedAverage + .01f;
        }

        public const float AIM_WEIGHT = 1.25f;
        public const float TAP_WEIGHT = 1f;
        public float[] HDWeights = MainForm.Instance.GetHDWeights();
        public float[] FLWeights = MainForm.Instance.GetFLWeights();
        public float[] EZWeights = MainForm.Instance.GetEZWeights();
        public float BIAS = MainForm.Instance.GetBiasMult();

        public float GetDynamicDiffRating(int hitCount, float gamespeed, string[] modifiers = null)
        {
            var aimRating = GetDynamicAimRating(hitCount, gamespeed);
            var tapRating = GetDynamicTapRating(hitCount, gamespeed);

            if (aimRating == 0 && tapRating == 0) return 0f;

            if (modifiers != null)
            {
                var aimPow = 1f;
                var tapPow = 1f;
                var isEZModeOn = modifiers.Contains("EZ");
                var mult = isEZModeOn ? .4f : 1f;
                if (modifiers.Contains("HD"))
                {
                    aimPow += HDWeights[0] * mult;
                    tapPow += HDWeights[1] * mult;
                }
                if (modifiers.Contains("FL"))
                {
                    aimPow += FLWeights[0] * mult;
                    tapPow += FLWeights[1] * mult;
                }
                if (isEZModeOn)
                {
                    aimPow -= EZWeights[0];
                    tapPow -= EZWeights[1];
                }

                if (aimPow <= 0) aimPow = .01f;
                if (tapPow <= 0) tapPow = .01f;

                aimRating *= aimPow;
                tapRating *= tapPow;
            }

            var totalRating = aimRating + tapRating;
            var aimPerc = aimRating / totalRating;
            var tapPerc = tapRating / totalRating;
            var aimWeight = (aimPerc + BIAS) * AIM_WEIGHT;
            var tapWeight = (tapPerc + BIAS) * TAP_WEIGHT;
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

        public void Dispose()
        {
            aimPerfMatrix = null;
            aimAnalyticsArray = null;
            aimRatingArray = null;
            tapPerfMatrix = null;
            tapAnalyticsArray = null;
            tapRatingArray = null;
        }

        public struct DataVector
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

        public struct DataVectorAnalytics
        {
            public float perfMax = 0f, perfMin = 0f, perfWeightedAverage = 0f;
            public float weightSum = 1f;

            public DataVectorAnalytics(List<DataVector> dataVectorList, float songLengthMult)
            {
                if (dataVectorList.Count > 0)
                {
                    CalculateWeightSum(dataVectorList, songLengthMult);
                    CalculateData(dataVectorList);
                }
            }

            public void CalculateWeightSum(List<DataVector> dataVectorList, float songLengthMult)
            {
                for (int i = 0; i < dataVectorList.Count; i++)
                    weightSum += dataVectorList[i].weight;
                weightSum *= songLengthMult;
            }

            public void CalculateData(List<DataVector> dataVectorList)
            {
                for (int i = 0; i < dataVectorList.Count; i++)
                {
                    if (dataVectorList[i].performance > perfMax)
                        perfMax = dataVectorList[i].performance;
                    else if (dataVectorList[i].performance < perfMin)
                        perfMin = dataVectorList[i].performance;

                    perfWeightedAverage += (dataVectorList[i].performance + dataVectorList[i].endurance) * (dataVectorList[i].weight / weightSum);
                }
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
