using System;
using System.Diagnostics;
using System.Formats.Asn1;
using System.Runtime.InteropServices;
using static TootTallyDifficultyCalculator2._0.Chart;

namespace TootTallyDifficultyCalculator2._0
{
    public class ChartPerformances
    {
        public static readonly float[] weights = {1f, 0.85f, 0.7225f, 0.6141f, 0.5220f, 0.4437f, 0.3771f, 0.3205f, 0.2724f, 0.2316f,
                                                    0.1968f, 0.1673f, 0.1422f, 0.1209f, 0.1027f, 0.0873f, 0.0742f, 0.0631f, 0.0536f, 0.0455f,
                                                    0.0387f, 0.0329f, 0.0280f, 0.0238f, 0.0202f, 0.0171f,}; //lol

        public DataVector[][] aimPerfMatrix;
        public DataVectorAnalytics[] aimAnalyticsArray;

        public DataVector[][] tapPerfMatrix;
        public DataVectorAnalytics[] tapAnalyticsArray;

        public DataVector[][] accPerfMatrix;
        public DataVectorAnalytics[] accAnalyticsArray;

        public float[] aimRatingArray;
        public float[] tapRatingArray;
        public float[] accRatingArray;
        private Chart _chart;
        private readonly int NOTE_COUNT;

        public ChartPerformances(Chart chart)
        {
            aimPerfMatrix = new DataVector[7][];
            tapPerfMatrix = new DataVector[7][];
            accPerfMatrix = new DataVector[7][];
            aimRatingArray = new float[7];
            tapRatingArray = new float[7];
            accRatingArray = new float[7];
            aimAnalyticsArray = new DataVectorAnalytics[7];
            tapAnalyticsArray = new DataVectorAnalytics[7];
            accAnalyticsArray = new DataVectorAnalytics[7];

            var length = chart.notes.Length;
            for (int i = 0; i < chart.GAME_SPEED.Length; i++)
            {
                aimPerfMatrix[i] = new DataVector[length];
                tapPerfMatrix[i] = new DataVector[length];
                accPerfMatrix[i] = new DataVector[length];
            }
            _chart = chart;
            NOTE_COUNT = _chart.notesDict[0].Count;
        }

        public void CalculatePerformances(int speedIndex)
        {
            var noteList = _chart.notesDict[speedIndex];
            var aimEndurance = 0f;
            var tapEndurance = 0f;
            var accEndurance = 0f;
            var endurance_decay = MainForm.Instance.GetEndDrain();
            var aim_div = MainForm.Instance.GetAimNum();
            var tap_div = MainForm.Instance.GetTapNum();
            var acc_div = MainForm.Instance.GetAccNum();
            var aim_end_note = MainForm.Instance.GetAimEndNote();
            var aim_end_mult = MainForm.Instance.GetAimEndMult();
            var tap_end = MainForm.Instance.GetTapEndMult();
            var acc_end = MainForm.Instance.GetAimEndSlider();

            var firstNotePosition = noteList[0].position;
            for (int i = 0; i < NOTE_COUNT; i++) //Main Forward Loop
            {
                var currentNote = noteList[i];
                var lengthSum = 0f;

                for (int j = i; j > i - 10 && j > 0; j--)
                    lengthSum += noteList[j].length * weights[i - j];

                //Second Forward Loop up to 26 notes and notes are at max 4 seconds appart
                var aimStrain = 0f;
                var tapStrain = 0f;
                var accStrain = 0f;

                var increaseDecay = (currentNote.position - firstNotePosition) * _chart.GAME_SPEED[speedIndex] < 30f;
                ComputeEnduranceDecay(ref aimEndurance, endurance_decay, increaseDecay);
                ComputeEnduranceDecay(ref tapEndurance, endurance_decay, increaseDecay);
                ComputeEnduranceDecay(ref accEndurance, endurance_decay, increaseDecay);

                for (int j = i - 1; j > 0 && j > i - 10 && currentNote.position - noteList[j].position <= 8; j--)
                {
                    var nextNote = noteList[j];
                    var previousNote = noteList[j + 1];
                    var MAX_TIME = previousNote.length * 0.6f;
                    var weight = weights[i - j - 1];

                    if (!IsSlider(previousNote, nextNote))
                    {
                        //Aim Calc
                        aimStrain += MathF.Sqrt(CalcAimStrain(previousNote, nextNote, weight, MAX_TIME)) / aim_div;
                        aimEndurance += CalcAimEndurance(previousNote, nextNote, weight, MAX_TIME, aim_end_note, aim_end_mult);

                        //Tap Calc
                        tapStrain += MathF.Sqrt(CalcTapStrain(previousNote, nextNote, weight)) / tap_div;
                        tapEndurance += CalcTapEndurance(previousNote, nextNote, weight, tap_end);
                    }

                    if (previousNote.pitchDelta != 0)
                    {
                        //Acc Calc
                        accStrain += MathF.Sqrt(CalcAccStrain(previousNote, weight)) / acc_div;
                        accEndurance += CalcAccEndurance(previousNote, weight, acc_end, aim_end_mult);
                    }

                }

                aimPerfMatrix[speedIndex][i] = new DataVector(currentNote.position, aimStrain + CalcNerfedEndurance(aimEndurance), lengthSum);
                tapPerfMatrix[speedIndex][i] = new DataVector(currentNote.position, tapStrain + CalcNerfedEndurance(tapEndurance), lengthSum);
                accPerfMatrix[speedIndex][i] = new DataVector(currentNote.position, accStrain + CalcNerfedEndurance(accEndurance), lengthSum);
            }
        }

        public static float CalcNerfedEndurance(float endurance)
        {
            return endurance > 1 ? MathF.Sqrt(endurance) : endurance;
        }

        public static void ComputeEnduranceDecay(ref float endurance, float enduranceDecay, bool increaseDecay)
        {
            if (endurance > 0f)
            {
                if (increaseDecay)
                    enduranceDecay += (endurance >= 1f ? .3f : .2f);
                endurance /= enduranceDecay;
            }
        }

        public static float CalcAimStrain(Note nextNote, Note previousNote, float weight, float MAX_TIME)
        {
            //Calc the space between two notes if they aren't connected sliders
            var distance = MathF.Pow(MathF.Abs(nextNote.pitchStart - previousNote.pitchEnd), .9f);
            if (nextNote.pitchDelta != 0)
                distance *= .35f;

            float speed = distance / MathF.Max(nextNote.position - (previousNote.position + previousNote.length), MAX_TIME);

            //return the weighted speed with all the multiplier
            return speed * weight;
        }

        public static float CalcTapStrain(Note nextNote, Note previousNote, float weight) =>
            2f / MathF.Pow(nextNote.position - previousNote.position, 1.5f) * weight;


        public static float CalcAccStrain(Note nextNote, float weight)
        {
            var strain = MathF.Abs(nextNote.pitchDelta) / nextNote.length;
            if (nextNote.pitchDelta <= 34.375f)
                strain *= .5f;

            return strain * weight;
        }

        public static float CalcAimEndurance(Note nextNote, Note previousNote, float weight, float MAX_TIME, float aim_end_note, float aim_end_mult)
        {
            var distance = MathF.Abs(nextNote.pitchStart - previousNote.pitchEnd);
            var t = nextNote.position - (previousNote.position + previousNote.length);
            float endurance = distance / MathF.Max(t, MAX_TIME * 3f) / aim_end_note / aim_end_mult;

            return CalcEnduranceWeight(endurance, weight);
        }

        public static bool IsSlider(Note nextNote, Note previousNote) => !(Math.Round(nextNote.position - (previousNote.position + previousNote.length), 3) > 0);

        public static float CalcTapEndurance(Note nextNote, Note previousNote, float weight, float tap_end_mult)
        {

            float timeDelta = nextNote.position - previousNote.position;
            float enduranceTapStrain = 0.45f / MathF.Pow(timeDelta, 1.1f);
            float endurance = enduranceTapStrain / tap_end_mult;

            return CalcEnduranceWeight(endurance, weight);
        }

        public static float CalcAccEndurance(Note nextNote, float weight, float acc_end, float aim_end_mult)
        {
            var endurance = MathF.Abs(nextNote.pitchDelta * .5f) / nextNote.length / (acc_end * aim_end_mult); //This is equal to 0 if its not a slider*/
            if (nextNote.pitchDelta <= 34.375f)
                endurance *= .5f;


            return CalcEnduranceWeight(endurance, weight);
        }

        public static float CalcEnduranceWeight(float endurance, float weight)
        {
            return endurance * weight;
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

        public float GetAimRating(float speed) => GetSkillRating(speed, aimRatingArray);

        public float GetTapRating(float speed) => GetSkillRating(speed, tapRatingArray);

        public float GetAccRating(float speed) => GetSkillRating(speed, accRatingArray);

        private float GetSkillRating(float speed, float[] skillRatingArray)
        {
            var index = (int)((speed - 0.5f) / .25f);
            if (speed % .25f == 0)
                return skillRatingArray[index];

            var minSpeed = _chart.GAME_SPEED[index];
            var maxSpeed = _chart.GAME_SPEED[index + 1];
            var by = (speed - minSpeed) / (maxSpeed - minSpeed);
            return Lerp(skillRatingArray[index], skillRatingArray[index + 1], by);
        }
        public float GetDynamicAimRating(float percent, float speed) => GetDynamicSkillRating(percent, aimPerfMatrix[(int)((speed - 0.5f) / .25f)]);
        public float GetDynamicTapRating(float percent, float speed) => GetDynamicSkillRating(percent, tapPerfMatrix[(int)((speed - 0.5f) / .25f)]);
        public float GetDynamicAccRating(float percent, float speed) => GetDynamicSkillRating(percent, accPerfMatrix[(int)((speed - 0.5f) / .25f)]);

        private float GetDynamicSkillRating(float percent, DataVector[] skillRatingArray)
        {
            if (skillRatingArray.Length <= 1 || percent <= 0) return 0;

            var maxRange = (int)Math.Clamp(skillRatingArray.Length * percent, 1, skillRatingArray.Length);
            DataVector[] array = skillRatingArray.OrderBy(x => x.performance).ToList().GetRange(0, maxRange).ToArray();
            var analytics = new DataVectorAnalytics(array);
            return analytics.perfWeightedAverage + .01f;
        }

        public const float AIM_WEIGHT = 1.2f;
        public const float TAP_WEIGHT = 1.15f;
        public const float ACC_WEIGHT = 1.1f;

        public static readonly float[] HDWeights = { .12f, .08f, .32f };
        public static readonly float[] FLWeights = { .35f, .12f, .08f };

        public float GetDiffRating(float gamespeed, string[] modifiers = null)
        {
            var aimRating = GetAimRating(gamespeed);
            var tapRating = GetTapRating(gamespeed);
            var accRating = GetAccRating(gamespeed);


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

                aimRating = MathF.Pow(aimRating, aimPow);
                tapRating = MathF.Pow(tapRating, tapPow);
                accRating = MathF.Pow(accRating, accPow);
            }

            if (aimRating != 0 && tapRating != 0 && accRating != 0)
            {
                var totalRating = aimRating + tapRating + accRating;
                var aimPerc = aimRating / totalRating;
                var tapPerc = tapRating / totalRating;
                var accPerc = accRating / totalRating;
                var aimWeight = (aimPerc + MainForm.Instance.GetBiasMult()) * AIM_WEIGHT;
                var tapWeight = (tapPerc + MainForm.Instance.GetBiasMult()) * TAP_WEIGHT;
                var accWeight = (accPerc + MainForm.Instance.GetBiasMult()) * ACC_WEIGHT;
                var totalWeight = aimWeight + tapWeight + accWeight;
                return ((aimRating * aimWeight) + (tapRating * tapWeight) + (accRating * accWeight)) / totalWeight;
            }
            else
                return 0f;
        }

        public float GetDynamicDiffRating(float percent, float gamespeed, string[] modifiers = null)
        {
            var aimRating = GetDynamicAimRating(percent, gamespeed);
            var tapRating = GetDynamicTapRating(percent, gamespeed);
            var accRating = GetDynamicAccRating(percent, gamespeed);


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

                aimRating = MathF.Pow(aimRating, aimPow);
                tapRating = MathF.Pow(tapRating, tapPow);
                accRating = MathF.Pow(accRating, accPow);
            }

            if (aimRating != 0 && tapRating != 0 && accRating != 0)
            {
                var totalRating = aimRating + tapRating + accRating;
                var aimPerc = aimRating / totalRating;
                var tapPerc = tapRating / totalRating;
                var accPerc = accRating / totalRating;
                var aimWeight = (aimPerc + MainForm.Instance.GetBiasMult()) * AIM_WEIGHT;
                var tapWeight = (tapPerc + MainForm.Instance.GetBiasMult()) * TAP_WEIGHT;
                var accWeight = (accPerc + MainForm.Instance.GetBiasMult()) * ACC_WEIGHT;
                var totalWeight = aimWeight + tapWeight + accWeight;
                return ((aimRating * aimWeight) + (tapRating * tapWeight) + (accRating * accWeight)) / totalWeight;
            }
            else
                return 0f;
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
            public float time;
            public float weight;

            public DataVector(float time, float performance, float weight)
            {
                this.time = time;
                this.performance = performance;
                this.weight = weight;
            }

        }

        public class DataVectorAnalytics
        {
            public float perfMax, perfMin, perfSum, perfWeightedAverage;

            public DataVectorAnalytics(DataVector[] dataVectorList)
            {
                CalculateData(dataVectorList);
            }
            public void CalculateData(DataVector[] dataVectorList)
            {
                perfSum = 0f;
                var weightSum = 0f;
                for (int i = 0; i < dataVectorList.Length; i++)
                {
                    if (dataVectorList[i] == null)
                        continue;

                    if (dataVectorList[i].performance > perfMax)
                        perfMax = dataVectorList[i].performance;
                    else if (dataVectorList[i].performance < perfMin)
                        perfMin = dataVectorList[i].performance;

                    perfSum += dataVectorList[i].performance * dataVectorList[i].weight;
                    weightSum += dataVectorList[i].weight;
                }
                if (weightSum == 0f)
                    perfWeightedAverage = 0;
                else
                    perfWeightedAverage = perfSum / weightSum;
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
            public DataVector[] aim;
            public DataVector[] tap;
            public DataVector[] acc;
        }

    }

}
