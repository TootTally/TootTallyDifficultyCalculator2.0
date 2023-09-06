using System.Runtime.InteropServices;
using static TootTallyDifficultyCalculator2._0.Chart;
using static TootTallyDifficultyCalculator2._0.ChartPerformances;

namespace TootTallyDifficultyCalculator2._0
{
    public class NoteCluster
    {
        public List<Note> noteList;

        public List<DataVector> aimPerformanceList;
        public List<DataVector> tapPerformanceList;
        public List<DataVector> accPerformanceList;
        public DataVectorAnalytics aimAnalytics;
        public DataVectorAnalytics tapAnalytics;
        public DataVectorAnalytics accAnalytics;
        private Chart _chart;

        public NoteCluster(Chart chart)
        {
            noteList = new List<Note>();
            aimPerformanceList = new List<DataVector>();
            tapPerformanceList = new List<DataVector>();
            accPerformanceList = new List<DataVector>();
            _chart = chart;
        }

        public void CalculateDifficulty(float speed)
        {
            var MAX_TIME = BeatToSeconds2(0.05f, _chart.tempo * speed);
            var AVERAGE_NOTE_LENGTH = noteList.Average(n => n.length);
            var TOTAL_NOTE_LENGTH = noteList.Sum(n => n.length);
            var aimEndurance = 0.05d;
            var tapEndurance = 0.1d;
            var endurance_decay = 1.0015d;

            var spanList = CollectionsMarshal.AsSpan(noteList);
            for (int i = 0; i < spanList.Length - 1; i++) //Main Forward Loop
            {
                var currentNote = spanList[i];
                var previousNote = currentNote;
                var directionMultiplier = 1d;
                var lengthSum = 0d;
                Direction currentDirection = Direction.Null, previousDirection = Direction.Null;

                //Second Forward Loop up to 26 notes and notes are at max 4 seconds appart
                var aimStrain = 0d;
                var tapStrain = 0d;
                var accStrain = 0d;

                for (int j = i + 1; j < spanList.Length && j < i + 26 && spanList[j].position - (currentNote.position + currentNote.length) <= 4; j++)
                {
                    var nextNote = spanList[j];
                    var weight = weights[j - i - 1];
                    var endDecayMult = Math.Pow(endurance_decay, weight);
                    if (aimEndurance > 1f)
                        aimEndurance /= endDecayMult;
                    if (tapEndurance > 1f)
                        tapEndurance /= endDecayMult;

                    //Aim Calc
                    aimStrain += Math.Sqrt(CalcAimStrain(nextNote, previousNote, ref currentDirection, ref previousDirection, weight, ref directionMultiplier, aimEndurance, MAX_TIME)) / 45f;
                    aimEndurance += CalcAimEndurance(nextNote, previousNote, weight, directionMultiplier, MAX_TIME);

                    //Tap Calc
                    tapStrain += Math.Sqrt(CalcTapStrain(nextNote, previousNote, weight, tapEndurance)) / 74f;
                    tapEndurance += CalcTapEndurance(nextNote, previousNote, weight);

                    //Acc Calc
                    accStrain += Math.Sqrt(CalcAccStrain(nextNote, previousNote, weight, directionMultiplier)) / 22f; // I can't figure that out yet

                    previousNote = nextNote;

                }
                var lenWeight = Math.Sqrt(currentNote.length) / TOTAL_NOTE_LENGTH;
                if (aimStrain != 0)
                    aimPerformanceList.Add(new DataVector(currentNote.position, aimStrain, lenWeight));
                if (tapStrain != 0)
                    tapPerformanceList.Add(new DataVector(currentNote.position, tapStrain, lenWeight));
                if (accStrain != 0)
                    accPerformanceList.Add(new DataVector(currentNote.position, accStrain, lenWeight));
            }
        }

        private const double MIN_TIMEDELTA = 1d / 120d;
        public static double CalcAimStrain(Note nextNote, Note previousNote, ref Direction currentDirection, ref Direction previousDirection, double weight, ref double directionMultiplier, double endurance, double MAX_TIME)
        {
            double speed = 0d;

            //Calc the space between two notes if they aren't connected sliders
            if (CheckIfSlider(nextNote, previousNote))
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
                var distance = MathF.Abs(nextNote.pitchStart - previousNote.pitchEnd) * 0.45f;
                var t = nextNote.position - (previousNote.position + previousNote.length);
                speed = distance / Math.Max(t, MAX_TIME);

                //Add directionalMultiplier bonus
                if (CheckDirectionChange(previousDirection, currentDirection))
                    directionMultiplier *= 1.02f;

                previousDirection = currentDirection; //update direction before looking at slider
            }

            if (nextNote.pitchDelta != 0) //Calc extra speed if its a slider
            {
                //speed += (MathF.Abs(nextNote.pitchDelta) / nextNote.length) * 0.25f;
                //currentDirection = nextNote.pitchDelta > 0 ? Direction.Up : nextNote.pitchDelta < 0 ? Direction.Down : Direction.Null; //Set direction for slider
                //Add directionalMultiplier bonus for slider
                if (CheckDirectionChange(previousDirection, currentDirection))
                    directionMultiplier *= 1.04f;

                previousDirection = currentDirection; //update direction from slider
            }
            //return the weighted speed with all the multiplier
            return speed * weight * directionMultiplier * endurance;
        }

        public static double CalcTapStrain(Note nextNote, Note previousNote, double weight, double endurance)
        {
            var tapStrain = 0d;
            if (nextNote.position - (previousNote.position + previousNote.length) > MIN_TIMEDELTA)
            {
                var timeDelta = Math.Max(nextNote.position - previousNote.position, MIN_TIMEDELTA);
                var strain = 12f / Math.Pow(timeDelta, 1.55f);
                tapStrain = strain * weight * endurance;
            }
            return tapStrain;
        }

        public static bool CheckDirectionChange(Direction prevDir, Direction currDir) => (prevDir != currDir && prevDir != Direction.Null && currDir != Direction.Null);

        public static double CalcAccStrain(Note nextNote, Note previousNote, double weight, double directionMultiplier)
        {
            var accStrain = 0d;

            if (nextNote.pitchDelta != 0)
            {
                var sliderHeight = (Math.Pow(Math.Abs(nextNote.pitchDelta / SLIDER_BREAK_CONST), 1.15f) * SLIDER_BREAK_CONST) / nextNote.length; //Promote height over speed
                var strain = sliderHeight;
                accStrain = strain * weight * directionMultiplier;
            }

            return accStrain;
        }

        public static double CalcAimEndurance(Note nextNote, Note previousNote, double weight, double directionalMultiplier, double MAX_TIME)
        {
            var endurance = 0d;
            var enduranceAimStrain = 0d;

            if (CheckIfSlider(nextNote, previousNote))
            {
                var distance = Math.Sqrt(MathF.Abs(nextNote.pitchStart - previousNote.pitchEnd)) * nextNote.length;
                var t = nextNote.position - (previousNote.position + previousNote.length);
                enduranceAimStrain = distance / Math.Max(t, MAX_TIME) / 45f;
            }

            /*if (nextNote.pitchDelta != 0) //Calc extra speed if its a slider
                enduranceAimStrain += (MathF.Abs(nextNote.pitchDelta) / (nextNote.length)) / 35f; //This is equal to 0 if its not a slider*/

            endurance += Math.Sqrt(enduranceAimStrain) / 120f;

            return endurance * weight * directionalMultiplier;
        }

        public static bool CheckIfSlider(Note nextNote, Note previousNote) => Math.Round(nextNote.position - (previousNote.position + previousNote.length), 2) > 0;

        public static double CalcTapEndurance(Note nextNote, Note previousNote, double weight)
        {
            var endurance = 0d;

            var enduranceTapStrain = 0d;
            if (nextNote.position - (previousNote.position + previousNote.length) > MIN_TIMEDELTA)
            {
                var timeDelta = nextNote.position - previousNote.position;
                enduranceTapStrain = 0.45f / Math.Pow(timeDelta, 1.65f);
            }

            endurance += Math.Sqrt(enduranceTapStrain) / 175f;

            return endurance * weight;
        }

        public void AddNotes(params Note[] note) => noteList.AddRange(note);

        public double GetClusterSize() => GetClusterEnd() - GetClusterStart();

        public double GetClusterEnd() => noteList.Max(x => x.position + x.length);

        public double GetClusterStart() => noteList.Min(x => x.position);

        public double GetDistanceFromCluster(NoteCluster noteCluster) => noteCluster.GetClusterStart() - GetClusterEnd();
    }
}
