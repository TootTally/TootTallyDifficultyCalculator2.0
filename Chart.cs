using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TootTallyDifficultyCalculator2._0
{
    public class Chart
    {
        public const float SLIDER_BREAK_CONST = 34.375f;


        public string[][] notes;
        public string[][] bgdata;
        public List<Note> notesList;
        public List<string> note_color_start;
        public List<string> note_color_end;
        public string endpoint;
        public string savednotespacing;
        public string tempo;
        public string timesig;
        public string trackRef;
        public string name;
        public string shortName;
        public string author;
        public string genre;
        public string description;
        public string difficulty;
        public string year;
        public List<Lyrics> lyrics;
        public List<DataVector> aimPerformanceList;
        public List<DataVector> tapPerformanceList;
        public List<DataVector> comboPerformanceList;
        public List<DataVector> accPerformanceList;
        public TimeSpan calculationTime;

        public void OnDeserialize()
        {
            notesList = new List<Note>();
            int count = 1;
            foreach (string[] n in notes)
            {
                notesList.Add(new Note(count, beatToSeconds(double.Parse(n[0]), float.Parse(tempo)), double.Parse(n[1]), float.Parse(n[2]), float.Parse(n[3]), float.Parse(n[4])));
                if (notesList.Last().length == 0)
                    notesList.Last().length = beatToSeconds(0.01, float.Parse(tempo));
                else
                    notesList.Last().length = beatToSeconds(notesList.Last().length, float.Parse(tempo));
                count++;
            }
            foreach (Note n in notesList)
                if (n.count < notesList.Count)
                    n.SetIsSlider(CheckIfIsSlider(n.count - 1));

            aimPerformanceList = new List<DataVector>();
            tapPerformanceList = new List<DataVector>();
            comboPerformanceList = new List<DataVector>();
            accPerformanceList = new List<DataVector>();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            CalcAllDiff();
            stopwatch.Stop();
            calculationTime = stopwatch.Elapsed;
            //prevent crash from A
            if (aimPerformanceList.Count <= 0)
                aimPerformanceList.Add(new DataVector(0, 0));
            if (tapPerformanceList.Count <= 0)
                tapPerformanceList.Add(new DataVector(0, 0));
            if (accPerformanceList.Count <= 0)
                accPerformanceList.Add(new DataVector(0, 0));

        }

        public void CalcAllDiff()
        {
            var MAX_TIME = beatToSeconds2(0.05, float.Parse(tempo));
            var MIN_TIMEDELTA = 1d / 120d;

            List<float> weights = new List<float>();//Pre calc weights
            for (int i = 0; i < 26; i++)
                weights.Add(MathF.Pow(0.945f, i));


            for (int i = 0; i < notesList.Count - 1; i++) //Main Forward Loop
            {
                Note currentNote = notesList[i];
                Note previousNote = currentNote;
                var comboMultiplier = 1f;
                var directionMultiplier = 1f;
                var lengthSum = 0d;
                Direction currentDirection = Direction.Null, previousDirection = Direction.Null;

                for (int j = i + 1; j < notesList.Count && j < i + 10; j++)
                {
                    //Combo Calc
                    lengthSum += notesList[j].length;
                }


                //Second Forward Loop up to 26 notes and notes are at max 4 seconds appart
                var speedStrain = 0d;
                var tapStrain = 0d;
                var accStrain = 0d;

                for (int j = i + 1; j < notesList.Count && j < i + 26 && notesList[j].position - (currentNote.position + currentNote.length) <= 4; j++)
                {
                    Note nextNote = notesList[j];
                    var weight = weights[j - i - 1];

                    //Aim Calc
                    speedStrain += CalcAimStrain(nextNote, previousNote, ref currentDirection, ref previousDirection, weight, comboMultiplier, ref directionMultiplier, MAX_TIME);

                    //Tap Calc
                    tapStrain += CalcTapStrain(nextNote, previousNote, weight, comboMultiplier, MIN_TIMEDELTA);

                    //Acc Calc
                    accStrain += CalcAccStrain(nextNote, previousNote);


                    previousNote = nextNote;

                }
                aimPerformanceList.Add(new DataVector((float)currentNote.position, (float)speedStrain));
                tapPerformanceList.Add(new DataVector((float)currentNote.position, (float)tapStrain));
                accPerformanceList.Add(new DataVector((float)currentNote.position, (float)accStrain));
            }
        }

        public static double CalcAimStrain(Note nextNote, Note previousNote, ref Direction currentDirection, ref Direction previousDirection, float weight, float comboMultiplier, ref float directionMultiplier, double MAX_TIME)
        {
            double speed;
            //Calc the space between two notes
            if (nextNote.pitchStart - previousNote.pitchEnd != 0)
                if (previousNote.pitchStart - nextNote.pitchEnd > 0)
                    currentDirection = Direction.Up;
                else
                    currentDirection = Direction.Down;
            else
                currentDirection = Direction.Null;

            var distance = MathF.Abs(nextNote.pitchStart - previousNote.pitchEnd);
            var currentNoteStart = nextNote.position;
            var previousNoteStart = previousNote.position;
            var previousNoteLength = previousNote.length;
            var t = currentNoteStart - (previousNoteStart + previousNoteLength);
            speed = distance / Math.Max(t, MAX_TIME);
            //Add directionalMultiplier bonus here

            previousDirection = currentDirection; //update direction before looking at slider


            if (nextNote.isSlider) //Calc extra speed if its a slider
            {
                speed += MathF.Abs(nextNote.pitchDelta) / nextNote.length; //This is equal to 0 if its not a slider
                currentDirection = nextNote.pitchDelta > 0 ? Direction.Up : nextNote.pitchDelta < 0 ? Direction.Down : Direction.Null; //Set direction for slider
                                                                                                                                       //Add directionalMultiplier bonus here


                previousDirection = currentDirection; //update direction from slider
            }

            return speed * weight * comboMultiplier * directionMultiplier;
        }

        public static double CalcTapStrain(Note nextNote, Note previousNote, float weight, float comboMultiplier, double MIN_TIMEDELTA)
        {
            var tapStrain = 0d;
            if (!nextNote.isSlider)
            {
                var timeDelta = Math.Max(nextNote.position - (previousNote.position + previousNote.length), MIN_TIMEDELTA);
                var strain = 25f / Math.Pow(timeDelta, 1.2f);
                tapStrain = strain * weight * comboMultiplier;
            }
            return tapStrain;
        }

        public static double CalcAccStrain(Note nextNote, Note previousNote)
        {
            var accStrain = 0d;

            return accStrain;
        }

        #region Note
        public class Note
        {
            public int count;
            public float pitchStart, pitchDelta, pitchEnd;
            public double position, length;
            public bool isSlider = false;

            public Note(int count, double position, double length, float pitchStart, float pitchDelta, float pitchEnd)
            {
                this.count = count;
                this.position = position;
                this.length = length;
                this.pitchStart = pitchStart;
                this.pitchDelta = pitchDelta;
                this.pitchEnd = pitchEnd;
            }

            public void SetIsSlider(bool isSlider) => this.isSlider = isSlider;

        }

        public enum Direction
        {
            Null,
            Up,
            Down

        }
        #endregion

        public class Lyrics
        {
            public string bar;
            public string text;
        }

        public string CheckIfSamePosition()
        {
            string message = "no duplicate found";
            bool foundDupe = false;
            for (int i = 1; i < notesList.Count - 1; i++)
            {
                if (notesList[i - 1].position == notesList[i].position || notesList[i + 1].position == notesList[i].position || notesList[i - 1].position == notesList[i + 1].position)
                {
                    if (!foundDupe)
                    {
                        message = "found duplicate(s) at ";
                        foundDupe = true;
                    }
                    message += notesList[i].count + ", ";
                }
            }

            return message;
        }

        public bool CheckIfIsSlider(int index) => (notesList[index].position + notesList[index].length >= notesList[index + 1].position) || (notesList[index].pitchDelta != 0);

        public List<string> ToDisplayData()
        {
            List<string> data = new List<string>
            {
                "Diff calculated in " + calculationTime.TotalMilliseconds.ToString("0.0000") + "ms",
                "___MAP DATA___",
                "Name: " + name,
                "shortName: " + shortName,
                "trackRef: " + trackRef,
                "year: " + year,
                "author: " + author,
                "genre: " + genre,
                "description: " + description,
                "difficulty: " + difficulty,
                "savednotespacing: " + savednotespacing,
                "endpoint: " + endpoint,
                "timesig: " + timesig,
                "tempo: " + tempo
            };
            string lyricsString = "Lyrics: ";
            if (lyrics != null && lyrics.Count > 0)
                lyrics.ForEach(s => lyricsString += s);
            else
                lyricsString += "No Lyrics found";
            data.Add(lyricsString);

            data.Add("Aim Rating: " + aimPerformanceList.Average(x => x.performance));
            data.Add("Tap Rating: " + tapPerformanceList.Average(x => x.performance));
            data.Add("Acc Rating: " + accPerformanceList.Average(x => x.performance));

            for (int i = 0; i < notesList.Count; i++)
            {
                var n = notesList[i];
                data.Add("nu: " + n.count.ToString("0.00") + " | " +
                    "po: " + n.position.ToString("0.00") + " | " +
                    "le: " + n.length.ToString("0.00") + " | " +
                    "ps: " + n.pitchStart.ToString("0.00") + " | " +
                    "pd: " + n.pitchDelta.ToString("0.00") + " | " +
                    "pe: " + n.pitchEnd.ToString("0.00") +
                    (n.isSlider ? " | Slider" : "") + " | " + 
                    (i < aimPerformanceList.Count?$"a: {aimPerformanceList[i].performance} t: {tapPerformanceList[i].performance} a: {accPerformanceList[i].performance}" : ""));
            }

            return data;
        }

        public float GetTempoMultiplier() => (float.Parse(tempo) / 100);
        public double beatToSeconds(double time, float bpm)
        {
            return time / bpm * 60f;
        }

        public double beatToSeconds2(double beat, float bpm) => (60f / bpm) * beat;

        public class DataVector
        {
            public float performance;
            public float time;
            public DataVector(float time, float performance)
            {
                this.time = time;
                this.performance = performance;
            }
            
           
        }

    }
}
