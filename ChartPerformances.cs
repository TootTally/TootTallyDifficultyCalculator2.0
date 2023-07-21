using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static TootTallyDifficultyCalculator2._0.Chart;

namespace TootTallyDifficultyCalculator2._0
{
    public class ChartPerformances
    {
        public Dictionary<float, List<DataVector>> aimPerfDict;
        public Dictionary<float, DataVectorAnalytics> aimAnalyticsDict;
        public Dictionary<float, List<DataVector>> aimEndPerfDict;
        public Dictionary<float, DataVectorAnalytics> aimEndAnalyticsDict;

        public Dictionary<float, List<DataVector>> tapPerfDict;
        public Dictionary<float, DataVectorAnalytics> tapAnalyticsDict;
        public Dictionary<float, List<DataVector>> tapEndPerfDict;
        public Dictionary<float, DataVectorAnalytics> tapEndAnalyticsDict;

        public Dictionary<float, List<DataVector>> accPerfDict;
        public Dictionary<float, DataVectorAnalytics> accAnalyticsDict;

        public Dictionary<float, float> aimRatingDict;
        public Dictionary<float, float> tapRatingDict;
        public Dictionary<float, float> accRatingDict;
        public Dictionary<float, float> starRatingDict;

        private Chart _chart;

        public ChartPerformances(Chart chart)
        {
            aimPerfDict = new Dictionary<float, List<DataVector>>();
            aimEndPerfDict = new Dictionary<float, List<DataVector>>();
            tapPerfDict = new Dictionary<float, List<DataVector>>();
            tapEndPerfDict = new Dictionary<float, List<DataVector>>();
            accPerfDict = new Dictionary<float, List<DataVector>>();
            aimRatingDict = new Dictionary<float, float>();
            tapRatingDict = new Dictionary<float, float>();
            accRatingDict = new Dictionary<float, float>();
            starRatingDict = new Dictionary<float, float>();
            aimAnalyticsDict = new Dictionary<float, DataVectorAnalytics>();
            aimEndAnalyticsDict = new Dictionary<float, DataVectorAnalytics>();
            tapAnalyticsDict = new Dictionary<float, DataVectorAnalytics>();
            tapEndAnalyticsDict = new Dictionary<float, DataVectorAnalytics>();
            accAnalyticsDict = new Dictionary<float, DataVectorAnalytics>();


            for (int i = 0; i < chart.GAME_SPEED.Length; i++)
            {
                var gamespeed = chart.GAME_SPEED[i];
                aimPerfDict[gamespeed] = new List<DataVector>();
                aimEndPerfDict[gamespeed] = new List<DataVector>();
                tapPerfDict[gamespeed] = new List<DataVector>();
                tapEndPerfDict[gamespeed] = new List<DataVector>();
                accPerfDict[gamespeed] = new List<DataVector>();
            }
            _chart = chart;
            PrecalculateVariables();
        }

        List<double> weights;

        public void PrecalculateVariables()
        {
            weights = new List<double>();
            for (int i = 0; i < 26; i++)
                weights.Add(FastPow(0.945f, i));
        }

        Note currentNote, previousNote, nextNote;
        double MAX_TIME;
        public void CalculatePerformances(float gamespeed)
        {
            var noteList = _chart.notesDict[gamespeed];
            MAX_TIME = BeatToSeconds2(0.05, float.Parse(_chart.tempo) * gamespeed);
            var MIN_TIMEDELTA = 1d / 240d;
            var AVERAGE_NOTE_LENGTH = noteList.Average(n => n.length);
            var TOTAL_NOTE_LENGTH = noteList.Sum(n => n.length);
            var aimEndurance = 0.3d;
            var tapEndurance = 0.3d;
            var endurance_decay = 1.005d;

            //Trace.WriteLine($"{_chart.name,15} T: " + TOTAL_NOTE_LENGTH);
            //Trace.WriteLine($"{_chart.name,15} A: " + AVERAGE_NOTE_LENGTH);

            for (int i = 0; i < noteList.Count - 1; i++) //Main Forward Loop
            {
                currentNote = noteList[i];
                previousNote = currentNote;
                var comboMultiplier = 0.8f;
                var directionMultiplier = 1f;
                var lengthSum = 0d;
                Direction currentDirection = Direction.Null, previousDirection = Direction.Null;

                var lenCount = 0;
                for (int j = i + 1; j < noteList.Count && j < i + 10; j++)
                {
                    //Combo Calc
                    lengthSum += noteList[j].length;
                    lenCount++;
                }
                comboMultiplier += (float)lengthSum;

                //Second Forward Loop up to 26 notes and notes are at max 4 seconds appart
                var aimStrain = 0d;
                var tapStrain = 0d;
                var accStrain = 0d;

                for (int j = i + 1; j < noteList.Count && j < i + 26 && noteList[j].position - (currentNote.position + currentNote.length) <= 4; j++)
                {
                    nextNote = noteList[j];
                    var weight = weights[j - i - 1];
                    var endDecayMult = Math.Pow(endurance_decay, weight);
                    if (aimEndurance > 1f)
                        aimEndurance /= endDecayMult;
                    if (tapEndurance > 1f)
                        tapEndurance /= endDecayMult;

                    //Aim Calc
                    aimStrain += Math.Sqrt(CalcAimStrain(nextNote, previousNote, ref currentDirection, ref previousDirection, weight, ref directionMultiplier, aimEndurance, MAX_TIME)) / 45f;
                    aimEndurance += CalcAimEndurance(nextNote, previousNote, weight, directionMultiplier);

                    //Tap Calc
                    tapStrain += Math.Sqrt(CalcTapStrain(nextNote, previousNote, weight, comboMultiplier, tapEndurance, MIN_TIMEDELTA)) / 58f;
                    tapEndurance += CalcTapEndurance(nextNote, previousNote, weight);

                    //Acc Calc
                    accStrain += Math.Sqrt(CalcAccStrain(nextNote, previousNote, weight, comboMultiplier, directionMultiplier, AVERAGE_NOTE_LENGTH, TOTAL_NOTE_LENGTH)) / 52f; // I can't figure that out yet

                    previousNote = nextNote;

                }

                aimPerfDict[gamespeed].Add(new DataVector((float)currentNote.position, (float)aimStrain, (float)(currentNote.length / TOTAL_NOTE_LENGTH)));
                aimEndPerfDict[gamespeed].Add(new DataVector((float)currentNote.position, (float)aimEndurance, 1));
                tapPerfDict[gamespeed].Add(new DataVector((float)currentNote.position, (float)tapStrain, (float)(currentNote.length / TOTAL_NOTE_LENGTH)));
                tapEndPerfDict[gamespeed].Add(new DataVector((float)currentNote.position, (float)tapEndurance, 1));
                accPerfDict[gamespeed].Add(new DataVector((float)currentNote.position, (float)accStrain, (float)(currentNote.length / TOTAL_NOTE_LENGTH)));
            }
        }

        public static double CalcAimStrain(Note nextNote, Note previousNote, ref Direction currentDirection, ref Direction previousDirection, double weight, ref float directionMultiplier, double endurance, double MAX_TIME)
        {
            double speed = 0d;

            //Calc the space between two notes if they aren't connected sliders
            if (nextNote.position - (previousNote.position + previousNote.length) > 0)
            {
                //check for the direction of the space
                if (nextNote.pitchStart - previousNote.pitchEnd != 0)
                    if (previousNote.pitchStart - nextNote.pitchEnd > 0)
                        currentDirection = Direction.Up;
                    else
                        currentDirection = Direction.Down;
                else
                    currentDirection = Direction.Null;

                //Calculate the speed in units per seconds, capped at a minimum of 0.05 beats for the time to prevent bad mapping practices
                var distance = MathF.Abs(nextNote.pitchStart - previousNote.pitchEnd) * 0.55f;
                var t = nextNote.position - (previousNote.position + previousNote.length);
                speed = distance / Math.Max(t, MAX_TIME);

                //Add directionalMultiplier bonus
                if (CheckDirectionChange(previousDirection, currentDirection))
                    directionMultiplier *= 1.02f;

                previousDirection = currentDirection; //update direction before looking at slider
            }

            if (nextNote.pitchDelta != 0) //Calc extra speed if its a slider
            {
                speed += MathF.Abs(nextNote.pitchDelta * 6f) / nextNote.length;
                currentDirection = nextNote.pitchDelta > 0 ? Direction.Up : nextNote.pitchDelta < 0 ? Direction.Down : Direction.Null; //Set direction for slider
                //Add directionalMultiplier bonus for slider
                if (CheckDirectionChange(previousDirection, currentDirection))
                    directionMultiplier *= 1.06f;

                previousDirection = currentDirection; //update direction from slider
            }
            //return the weighted speed with all the multiplier
            return speed * weight * directionMultiplier * endurance;
        }

        public double CalcTapStrain(Note nextNote, Note previousNote, double weight, float comboMultiplier, double endurance, double MIN_TIMEDELTA)
        {
            var tapStrain = 0d;
            if (nextNote.position - (previousNote.position + previousNote.length) > 0)
            {
                var timeDelta = Math.Max(nextNote.position - previousNote.position, MIN_TIMEDELTA);
                var strain = 10.5f / Math.Pow(timeDelta, 1.75f);
                tapStrain = strain * weight * comboMultiplier * endurance;
            }
            return tapStrain;
        }

        public static bool CheckDirectionChange(Direction prevDir, Direction currDir) => (prevDir != currDir && prevDir != Direction.Null && currDir != Direction.Null);

        public double CalcAccStrain(Note nextNote, Note previousNote, double weight, float comboMultiplier, float directionMultiplier, double AVERAGE_NOTE_LENGTH, double TOTAL_NOTE_LENGTH)
        {
            var accStrain = 0d;
            var lengthmult = Math.Sqrt((nextNote.length*1.01f) / AVERAGE_NOTE_LENGTH);

            if (nextNote.position - (previousNote.position + previousNote.length) > 0)
            {
                var noteHeight = MathF.Abs(nextNote.pitchStart - previousNote.pitchEnd) / (nextNote.position - previousNote.position);
                var strain = noteHeight * lengthmult;
                accStrain = strain * weight * directionMultiplier * comboMultiplier;
            }
            if (nextNote.pitchDelta != 0)
            {
                var sliderHeight = Math.Abs(nextNote.pitchDelta * 1.25f) / nextNote.length; //Promote height over speed
                var strain = sliderHeight * lengthmult;
                accStrain = strain * weight * directionMultiplier * comboMultiplier;
            }

            return accStrain;
        }

        public static double CalcAimEndurance(Note nextNote, Note previousNote, double weight, float directionalMultiplier)
        {
            var endurance = 0d;
            var enduranceAimStrain = 0d;

            if (nextNote.position - (previousNote.position + previousNote.length) > 0)
            {
                var distance = MathF.Abs(nextNote.pitchStart - previousNote.pitchEnd) * (nextNote.length / 10f);
                var t = nextNote.position - (previousNote.position + previousNote.length);
                enduranceAimStrain = distance / t;
            }

            if (nextNote.pitchDelta != 0) //Calc extra speed if its a slider
                enduranceAimStrain += (MathF.Abs(nextNote.pitchDelta) / nextNote.length) / 135f; //This is equal to 0 if its not a slider

            endurance += Math.Sqrt(enduranceAimStrain) / 240f;

            return endurance * weight * directionalMultiplier;
        }

        public static double CalcTapEndurance(Note nextNote, Note previousNote, double weight)
        {
            var endurance = 0d;

            var enduranceTapStrain = 0d;
            if (nextNote.position - (previousNote.position + previousNote.length) > 0)
            {
                var timeDelta = nextNote.position - previousNote.position;
                enduranceTapStrain = 0.65f / Math.Pow(timeDelta, 1.85f);
            }

            endurance += Math.Sqrt(enduranceTapStrain) / 350f;

            return endurance * weight;
        }

        public void CalculateAnalytics(float gamespeed)
        {
            if (aimPerfDict[gamespeed].Count <= 0)
                aimPerfDict[gamespeed].Add(new DataVector(0, 0, 0));
            if (aimEndPerfDict[gamespeed].Count <= 0)
                aimEndPerfDict[gamespeed].Add(new DataVector(0, 0, 0));
            if (tapPerfDict[gamespeed].Count <= 0)
                tapPerfDict[gamespeed].Add(new DataVector(0, 0, 0));
            if (tapEndPerfDict[gamespeed].Count <= 0)
                tapEndPerfDict[gamespeed].Add(new DataVector(0, 0, 0));
            if (accPerfDict[gamespeed].Count <= 0)
                accPerfDict[gamespeed].Add(new DataVector(0, 0, 0));


            aimAnalyticsDict[gamespeed] = new DataVectorAnalytics(aimPerfDict[gamespeed]);
            aimEndAnalyticsDict[gamespeed] = new DataVectorAnalytics(aimEndPerfDict[gamespeed]);
            tapAnalyticsDict[gamespeed] = new DataVectorAnalytics(tapPerfDict[gamespeed]);
            tapEndAnalyticsDict[gamespeed] = new DataVectorAnalytics(tapEndPerfDict[gamespeed]);
            accAnalyticsDict[gamespeed] = new DataVectorAnalytics(accPerfDict[gamespeed]);
        }

        public void CalculateRatings(float gamespeed)
        {
            var aimRating = aimRatingDict[gamespeed] = aimAnalyticsDict[gamespeed].perfWeightedAverage + 0.01f;
            var tapRating = tapRatingDict[gamespeed] = tapAnalyticsDict[gamespeed].perfWeightedAverage + 0.01f;
            var accRating = accRatingDict[gamespeed] = accAnalyticsDict[gamespeed].perfWeightedAverage + 0.01f;

            if (aimRating != 0 && tapRating != 0 && accRating != 0)
            {
                var totalRating = aimRating + tapRating + accRating;
                var aimPerc = aimRating / totalRating;
                var tapPerc = tapRating / totalRating;
                var accPerc = accRating / totalRating;
                var aimWeight = (aimPerc + 0.25f) * 1.3f;
                var tapWeight = (tapPerc + 0.25f) * 1.05f;
                var accWeight = (accPerc + 0.25f) * 1.1f;
                var totalWeight = aimWeight + tapWeight + accWeight;
                starRatingDict[gamespeed] = ((aimRating * aimWeight) + (tapRating * tapWeight) + (accRating * accWeight)) / totalWeight;
            }
            else
                starRatingDict[gamespeed] = 0f;
        }

        public float GetDiffRating(float speed)
        {
            if (speed % .25f == 0)
                return starRatingDict[speed];

            var index = (int)((speed - 0.5f) / .25f);
            var minSpeed = _chart.GAME_SPEED[index];
            var maxSpeed = _chart.GAME_SPEED[index + 1];
            var by = (speed - minSpeed) / (maxSpeed - minSpeed);
            return Lerp(starRatingDict[minSpeed], starRatingDict[maxSpeed], by);
        }

        public static float Lerp(float firstFloat, float secondFloat, float by) //Linear easing
        {
            return firstFloat + (secondFloat - firstFloat) * by;
        }

        public static double FastPow(double num, int exp)
        {
            double result = 1.0;
            while (exp > 0)
            {
                if (exp % 2 == 1)
                    result *= num;
                exp >>= 1;
                num *= num;
            }
            return result;
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
            public float perfAverage, perfMax, perfMin, perfSum, perfWeightedAverage;

            public DataVectorAnalytics(List<DataVector> dataVectorList)
            {
                perfAverage = dataVectorList.Average(x => x.performance);
                perfWeightedAverage = CalculateWeightedAverage(dataVectorList);
                perfMax = dataVectorList.Max(x => x.performance);
                perfMin = dataVectorList.Min(x => x.performance);
                perfSum = dataVectorList.Sum(x => x.performance);
            }

            public float CalculateWeightedAverage(List<DataVector> dataVectorList) => dataVectorList.Sum(x => x.performance * x.weight) / dataVectorList.Sum(x => x.weight);
        } 

        public class SerializableDiffData
        {
            public SerializableDataVector speed050;
            public SerializableDataVector speed075;
            public SerializableDataVector speed100;
            public SerializableDataVector speed125;
            public SerializableDataVector speed150;
            public SerializableDataVector speed175;
            public SerializableDataVector speed200;

            public SerializableDiffData(ChartPerformances performances)
            {
                speed050 = new SerializableDataVector() { aim = performances.aimPerfDict[0.5f], tap = performances.tapPerfDict[0.5f], acc = performances.accPerfDict[0.5f] };
                speed075 = new SerializableDataVector() { aim = performances.aimPerfDict[0.75f], tap = performances.tapPerfDict[0.75f], acc = performances.accPerfDict[0.75f] };
                speed100 = new SerializableDataVector() { aim = performances.aimPerfDict[1f], tap = performances.tapPerfDict[1f], acc = performances.accPerfDict[1f] };
                speed125 = new SerializableDataVector() { aim = performances.aimPerfDict[1.25f], tap = performances.tapPerfDict[1.25f], acc = performances.accPerfDict[1.25f] };
                speed150 = new SerializableDataVector() { aim = performances.aimPerfDict[1.5f], tap = performances.tapPerfDict[1.5f], acc = performances.accPerfDict[1.5f] };
                speed175 = new SerializableDataVector() { aim = performances.aimPerfDict[1.75f], tap = performances.tapPerfDict[1.75f], acc = performances.accPerfDict[1.75f] };
                speed200 = new SerializableDataVector() { aim = performances.aimPerfDict[2f], tap = performances.tapPerfDict[2f], acc = performances.accPerfDict[2f] };
            }
        }

        public class SerializableDataVector
        {
            public List<DataVector> aim;
            public List<DataVector> tap;
            public List<DataVector> acc;
        }
    }

}
