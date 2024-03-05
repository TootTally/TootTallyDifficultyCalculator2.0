using System;
using System.Net.Mime;

namespace TootTallyDifficultyCalculator2._0
{
    public class ChartPerformances
    {
        public static readonly float[] weights = {1f, 0.85f, 0.7225f, 0.6141f, 0.5220f, 0.4437f, 0.3771f, 0.3205f, 0.2724f, 0.2316f,
                                                    0.1968f, 0.1673f, 0.1422f, 0.1209f, 0.1027f, 0.0873f, 0.0742f, 0.0631f, 0.0536f, 0.0455f,
                                                    0.0387f, 0.0329f, 0.0280f, 0.0238f, 0.0202f, 0.0171f,}; //lol
        public const float CHEESABLE_THRESHOLD = 34.375f;


        public List<DataVector>[] aimPerfMatrix;
        public DataVectorAnalytics[] aimAnalyticsArray;

        public List<DataVector>[] tapPerfMatrix;
        public DataVectorAnalytics[] tapAnalyticsArray;

        public List<DataVector>[] accPerfMatrix;
        public DataVectorAnalytics[] accAnalyticsArray;

        public float[] aimRatingArray;
        public float[] tapRatingArray;
        public float[] accRatingArray;
        private TMBChart _chart;
        private readonly int NOTE_COUNT;

        public ChartPerformances(TMBChart chart)
        {
            aimPerfMatrix = new List<DataVector>[7];
            tapPerfMatrix = new List<DataVector>[7];
            accPerfMatrix = new List<DataVector>[7];
            aimRatingArray = new float[7];
            tapRatingArray = new float[7];
            accRatingArray = new float[7];
            aimAnalyticsArray = new DataVectorAnalytics[7];
            tapAnalyticsArray = new DataVectorAnalytics[7];
            accAnalyticsArray = new DataVectorAnalytics[7];

            var length = chart.notes.Length;
            for (int i = 0; i < chart.GAME_SPEED.Length; i++)
            {
                aimPerfMatrix[i] = new List<DataVector>(length);
                tapPerfMatrix[i] = new List<DataVector>(length);
                accPerfMatrix[i] = new List<DataVector>(length);
            }
            _chart = chart;
            NOTE_COUNT = _chart.notesDict[0].Count;
        }

        public void CalculatePerformances(int speedIndex)
        {
            var noteList = _chart.notesDict[speedIndex];

            for (int i = 0; i < NOTE_COUNT; i++) //Main Forward Loop
            {
                var currentNote = noteList[i];
                var lengthSum = currentNote.length;
                var aimStrain = 0f;
                var tapStrain = 0f;
                var accStrain = 0f;
                var aimEndurance = 0f;
                var tapEndurance = 0f;
                var accEndurance = 0f;
                for (int j = i - 1; j > i - 10 && j > 0; j--)
                    lengthSum += noteList[j].length * weights[i - j];
                var enduranceDecay = MainForm.Instance.GetEndDrain();
                if (i > 0)
                {
                    var distanceFromLastNote = MathF.Sqrt(currentNote.position - noteList[i - 1].position);
                    /*ComputeEnduranceDecay(ref aimEndurance, enduranceDecay, distanceFromLastNote);
                    ComputeEnduranceDecay(ref tapEndurance, enduranceDecay, distanceFromLastNote);
                    ComputeEnduranceDecay(ref accEndurance, enduranceDecay, distanceFromLastNote);*/
                }

                for (int j = i - 1; j > 0 && j > i - 26 && MathF.Abs(currentNote.position - noteList[j].position) <= 8f; j--)
                {
                    var prevNote = noteList[j];
                    var nextNote = noteList[j + 1];
                    var weight = weights[i - j - 1];

                    var deltaTime = nextNote.position - (prevNote.position + prevNote.length);
                    var deltaSlide = MathF.Abs(prevNote.pitchDelta);

                    if (!IsSlider(deltaTime))
                    {
                        //Aim Calc
                        deltaTime += prevNote.length * .4f;
                        var aimDistance = MathF.Abs(nextNote.pitchStart - prevNote.pitchEnd);
                        var noteMoved = aimDistance != 0 || deltaSlide != 0;
                        if (noteMoved)
                        {
                            aimStrain += CalcAimStrain(aimDistance, weight, deltaTime, deltaSlide);
                            //aimEndurance += CalcAimStrain(aimDistance, weight, deltaTime, deltaSlide);
                        }

                        //Tap Calc
                        var tapDelta = nextNote.position - prevNote.position;
                        tapStrain += CalcTapStrain(tapDelta, weight);
                        //tapEndurance += CalcTapStrain(tapDelta, weight);
                    }

                    if (deltaSlide != 0)
                    {
                        //Acc Calc
                        accStrain += CalcAccStrain(prevNote, deltaSlide, weight);
                        //accEndurance += CalcAccStrain(prevNote, deltaSlide, weight);
                    }

                }

                aimPerfMatrix[speedIndex].Add(new DataVector(currentNote.position, aimStrain, aimEndurance, lengthSum));
                tapPerfMatrix[speedIndex].Add(new DataVector(currentNote.position, tapStrain, tapEndurance, lengthSum));
                accPerfMatrix[speedIndex].Add(new DataVector(currentNote.position, accStrain, accEndurance, lengthSum));
            }
        }
        public static bool IsSlider(float deltaTime) => !(MathF.Round(deltaTime, 3) > 0);

        public static float CalcNerfedEndurance(float endurance)
        {
            return endurance > 1 ? MathF.Sqrt(endurance) : endurance;
        }

        public static void ComputeEnduranceDecay(ref float endurance, float enduranceDecay, float distanceFromLastNote)
        {
            if (endurance > 1f)
                endurance /= enduranceDecay + MathF.Pow(MainForm.Instance.GetEndDrainExtra() * distanceFromLastNote, MathF.E);
        }

        //https://www.desmos.com/calculator/e4kskdn8mu
        public static float ComputeStrain(float a) => -29f * MathF.Pow(a + 1, -.01f * MathF.E) + 29f + (3f * a) / 29f;

        public static float CalcAimStrain(float distance, float weight, float deltaTime, float sliderDelta)
        {
            var speed = distance / deltaTime;
            return speed;
        }

        public static float CalcTapStrain(float tapDelta, float weight)
        {
            return 45f / tapDelta;
        }


        public static float CalcAccStrain(Note prevNote, float slideDelta, float weight)
        {
            var speed = 4f * slideDelta / prevNote.length;
            return speed;
        }

        public static float CalcAimEndurance(float weight, float distance, float deltaTime, float aim_end_note, float aim_end_mult)
        {
            return 0f * weight;
        }


        public static float CalcTapEndurance(float tapDelta, float weight, float tap_end_mult)
        {
            return 0f * weight;
        }

        public static float CalcAccEndurance(Note prevNote, float slideDelta, float weight, float acc_end, float aim_end_mult)
        {
            return 0f * weight;
        }

        public void CalculateAnalytics(int gamespeed)
        {
            aimAnalyticsArray[gamespeed] = new DataVectorAnalytics(aimPerfMatrix[gamespeed]);
            tapAnalyticsArray[gamespeed] = new DataVectorAnalytics(tapPerfMatrix[gamespeed]);
            accAnalyticsArray[gamespeed] = new DataVectorAnalytics(accPerfMatrix[gamespeed]);
        }

        public void CalculateRatings(int gamespeed)
        {
            aimRatingArray[gamespeed] = aimAnalyticsArray[gamespeed].perfWeightedAverage + 0.01f;
            tapRatingArray[gamespeed] = tapAnalyticsArray[gamespeed].perfWeightedAverage + 0.01f;
            accRatingArray[gamespeed] = accAnalyticsArray[gamespeed].perfWeightedAverage + 0.01f;
        }

        public float GetDynamicAimRating(float percent, float speed) => GetDynamicSkillRating(percent, speed, aimPerfMatrix);
        public float GetDynamicTapRating(float percent, float speed) => GetDynamicSkillRating(percent, speed, tapPerfMatrix);
        public float GetDynamicAccRating(float percent, float speed) => GetDynamicSkillRating(percent, speed, accPerfMatrix);

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
        public const float ACC_WEIGHT = 1.1f;

        public static readonly float[] HDWeights = { .1f, .09f, .12f };
        public static readonly float[] FLWeights = { .18f, .05f, .09f };

        public float GetDynamicDiffRating(float percent, float gamespeed, string[] modifiers = null)
        {
            var aimRating = GetDynamicAimRating(percent, gamespeed);
            var tapRating = GetDynamicTapRating(percent, gamespeed);
            var accRating = GetDynamicAccRating(percent, gamespeed);

            if (aimRating == 0 && tapRating == 0 && accRating == 0) return 0f;

            if (modifiers != null)
            {
                var aimPow = 1f;
                var tapPow = 1f;
                var accPow = 1f;
                if (modifiers.Contains("HD"))
                {
                    aimPow += HDWeights[0];
                    tapPow += HDWeights[1];
                    accPow += HDWeights[2];
                }
                if (modifiers.Contains("FL"))
                {
                    aimPow += FLWeights[0];
                    tapPow += FLWeights[1];
                    accPow += FLWeights[2];
                }

                aimRating = MathF.Pow(aimRating + 1f, aimPow) - 1f;
                tapRating = MathF.Pow(tapRating + 1f, tapPow) - 1f;
                accRating = MathF.Pow(accRating + 1f, accPow) - 1f;
            }

            return (aimRating + tapRating + accRating) / 3f;
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
            public float perfMax, perfMin, perfSum, perfWeightedAverage;
            public float weightSum;

            public DataVectorAnalytics(List<DataVector> dataVectorList)
            {
                CalculateWeightSum(dataVectorList);
                CalculateData(dataVectorList);
            }

            public void CalculateWeightSum(List<DataVector> dataVectorList) => weightSum = dataVectorList.Sum(x => x.weight);

            public void CalculateData(List<DataVector> dataVectorList)
            {
                perfSum = 0f;
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
                if (weightSum == 0f)
                    perfWeightedAverage = 0;
                else
                    perfWeightedAverage = perfSum / dataVectorList.Count;
            }
        }

        public class SerializableDiffData
        {
            public SerializableDataVector[] data;

            public SerializableDiffData(ChartPerformances performances)
            {
                data = new SerializableDataVector[]
                {
                    new SerializableDataVector() { speed = .5f, aim = performances.aimPerfMatrix[0], tap = performances.tapPerfMatrix[0], acc = performances.accPerfMatrix[0]},
                    new SerializableDataVector() { speed = .75f,aim = performances.aimPerfMatrix[1], tap = performances.tapPerfMatrix[1], acc = performances.accPerfMatrix[1]},
                    new SerializableDataVector() { speed = 1f,aim = performances.aimPerfMatrix[2], tap = performances.tapPerfMatrix[2], acc = performances.accPerfMatrix[2]},
                    new SerializableDataVector() { speed = 1.25f,aim = performances.aimPerfMatrix[3], tap = performances.tapPerfMatrix[3], acc = performances.accPerfMatrix[3]},
                    new SerializableDataVector() { speed = 1.50f,aim = performances.aimPerfMatrix[4], tap = performances.tapPerfMatrix[4], acc = performances.accPerfMatrix[4]},
                    new SerializableDataVector() { speed = 1.75f,aim = performances.aimPerfMatrix[5], tap = performances.tapPerfMatrix[5], acc = performances.accPerfMatrix[5]},
                    new SerializableDataVector() { speed = 2f,aim = performances.aimPerfMatrix[6], tap = performances.tapPerfMatrix[6], acc = performances.accPerfMatrix[6]},
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
