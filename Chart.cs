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
        public readonly float[] GAME_SPEED = { .5f, .75f, 1f, 1.25f, 1.5f, 1.75f, 2f };

        public string[][] notes;
        public string[][] bgdata;
        public Dictionary<float, List<Note>> notesDict;
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
        public Dictionary<float, List<DataVector>> aimPerformanceDict;
        public Dictionary<float, DataVectorAnalytics> aimAnalyticsDict;
        public Dictionary<float, List<DataVector>> tapPerformanceDict;
        public Dictionary<float, DataVectorAnalytics> tapAnalyticsDict;
        public Dictionary<float, List<DataVector>> accPerformanceDict;
        public Dictionary<float, DataVectorAnalytics> accAnalyticsDict;
        public TimeSpan calculationTime;

        public void OnDeserialize()
        {
            notesDict = new Dictionary<float, List<Note>>();
            for (int i = 0; i < GAME_SPEED.Length; i++)
            {
                var gamespeed = GAME_SPEED[i];
                int count = 1;
                if (!notesDict.ContainsKey(gamespeed))
                    notesDict[gamespeed] = new List<Note>();
                foreach (string[] n in notes)
                {
                    notesDict[gamespeed].Add(new Note(count, BeatToSeconds(double.Parse(n[0]), float.Parse(tempo) * gamespeed), double.Parse(n[1]), float.Parse(n[2]), float.Parse(n[3]), float.Parse(n[4])));
                    if (notesDict[gamespeed].Last().length == 0)
                        notesDict[gamespeed].Last().length = BeatToSeconds(0.01, float.Parse(tempo) * gamespeed);
                    else
                        notesDict[gamespeed].Last().length = BeatToSeconds(notesDict[gamespeed].Last().length, float.Parse(tempo) * gamespeed);
                    count++;
                }
            }

            for (int i = 0; i < GAME_SPEED.Length; i++)
            {
                foreach (Note n in notesDict[GAME_SPEED[i]])
                    if (n.count < notesDict[GAME_SPEED[i]].Count)
                        n.SetIsSlider(CheckIfIsSlider(GAME_SPEED[i], n.count - 1));
            }


            aimPerformanceDict = new Dictionary<float, List<DataVector>>();
            tapPerformanceDict = new Dictionary<float, List<DataVector>>();
            accPerformanceDict = new Dictionary<float, List<DataVector>>();
            aimAnalyticsDict = new Dictionary<float, DataVectorAnalytics>();
            tapAnalyticsDict = new Dictionary<float, DataVectorAnalytics>();
            accAnalyticsDict = new Dictionary<float, DataVectorAnalytics>();


            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < GAME_SPEED.Length; i++)
                CalcAllDiff(GAME_SPEED[i]);


            for (int i = 0; i < GAME_SPEED.Length; i++)
            {
                var gamespeed = GAME_SPEED[i];
                //prevent crash from A
                if (!aimPerformanceDict.ContainsKey(gamespeed))
                    aimPerformanceDict[gamespeed] = new List<DataVector>() { new DataVector(0, 0) };
                if (!tapPerformanceDict.ContainsKey(gamespeed))
                    tapPerformanceDict[gamespeed] = new List<DataVector>() { new DataVector(0, 0) };
                if (!accPerformanceDict.ContainsKey(gamespeed))
                    accPerformanceDict[gamespeed] = new List<DataVector>() { new DataVector(0, 0) };

                if (!aimAnalyticsDict.ContainsKey(gamespeed))
                    aimAnalyticsDict[gamespeed] = new DataVectorAnalytics(aimPerformanceDict[gamespeed]);
                if (!tapAnalyticsDict.ContainsKey(gamespeed))
                    tapAnalyticsDict[gamespeed] = new DataVectorAnalytics(tapPerformanceDict[gamespeed]);
                if (!accAnalyticsDict.ContainsKey(gamespeed))
                    accAnalyticsDict[gamespeed] = new DataVectorAnalytics(accPerformanceDict[gamespeed]);



            }
            stopwatch.Stop();
            calculationTime = stopwatch.Elapsed;

            var breakDistThreshold = 3.5f / (float.Parse(tempo) / 120f);

            List<NoteCluster> clusters = new List<NoteCluster>();
            List<Note> currentNoteList = new List<Note>();

            Stopwatch stopwatch1 = new Stopwatch();
            stopwatch1.Start();
            for (int i = 1; i < notesDict[1].Count; i++)
            {
                Note previousNote = notesDict[1][i - 1];
                Note nextNote = notesDict[1][i];
                currentNoteList.Add(previousNote);
                if (nextNote.position - (previousNote.position + previousNote.length) > breakDistThreshold || i == notesDict[1].Count - 1)
                {
                    if (i == notesDict[1].Count - 1)
                        currentNoteList.Add(nextNote);
                    NoteCluster cluster = new NoteCluster();
                    cluster.noteList.AddRange(currentNoteList);
                    clusters.Add(cluster);
                    currentNoteList.Clear();
                }
            }
            stopwatch1.Stop();
            Trace.WriteLine($"{shortName} process time was {stopwatch1.Elapsed.TotalMilliseconds}ms");
            Trace.WriteLine($"{shortName} cluster count: {clusters.Count} with {breakDistThreshold} distance threshold");
            for (int i = 0; i < clusters.Count; i++)
                Trace.WriteLine($"cluster #{i} has {clusters[i].noteList.Count} notes and last {clusters[i].GetClusterSize()}s.");
            Trace.WriteLine($"-----------------------------------------------------");
        }

        public void CalcAllDiff(float gamespeed)
        {
            var MAX_TIME = BeatToSeconds2(0.05, float.Parse(tempo) * gamespeed);
            var MIN_TIMEDELTA = 1d / 120d;
            var AVERAGE_NOTE_LENGTH = notesDict[gamespeed].Average(n => n.length);

            List<float> weights = new List<float>();//Pre calc weights
            for (int i = 0; i < 26; i++)
                weights.Add(MathF.Pow(0.945f, i));


            for (int i = 0; i < notesDict[gamespeed].Count - 1; i++) //Main Forward Loop
            {
                Note currentNote = notesDict[gamespeed][i];
                Note previousNote = currentNote;
                var comboMultiplier = 1f;
                var directionMultiplier = 1f;
                var lengthSum = 0d;
                Direction currentDirection = Direction.Null, previousDirection = Direction.Null;

                for (int j = i + 1; j < notesDict[gamespeed].Count && j < i + 10; j++)
                {
                    //Combo Calc
                    lengthSum += notesDict[gamespeed][j].length;
                }


                //Second Forward Loop up to 26 notes and notes are at max 4 seconds appart
                var speedStrain = 0d;
                var tapStrain = 0d;
                var accStrain = 0d;

                for (int j = i + 1; j < notesDict[gamespeed].Count && j < i + 26 && notesDict[gamespeed][j].position - (currentNote.position + currentNote.length) <= 4; j++)
                {
                    Note nextNote = notesDict[gamespeed][j];
                    var weight = weights[j - i - 1];

                    //Aim Calc
                    speedStrain += CalcAimStrain(nextNote, previousNote, ref currentDirection, ref previousDirection, weight, ref directionMultiplier, MAX_TIME);

                    //Tap Calc
                    tapStrain += CalcTapStrain(nextNote, previousNote, weight, MIN_TIMEDELTA);

                    //Acc Calc
                    accStrain += CalcAccStrain(nextNote, previousNote, weight, comboMultiplier, AVERAGE_NOTE_LENGTH);


                    previousNote = nextNote;

                }
                if (!aimPerformanceDict.ContainsKey(gamespeed))
                {
                    aimPerformanceDict[gamespeed] = new List<DataVector>();
                    tapPerformanceDict[gamespeed] = new List<DataVector>();
                    accPerformanceDict[gamespeed] = new List<DataVector>();
                }

                aimPerformanceDict[gamespeed].Add(new DataVector((float)currentNote.position, (float)speedStrain));
                tapPerformanceDict[gamespeed].Add(new DataVector((float)currentNote.position, (float)tapStrain));
                accPerformanceDict[gamespeed].Add(new DataVector((float)currentNote.position, (float)accStrain));
            }
        }

        public static double CalcAimStrain(Note nextNote, Note previousNote, ref Direction currentDirection, ref Direction previousDirection, float weight, ref float directionMultiplier, double MAX_TIME)
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

            return speed * weight * directionMultiplier;
        }

        public static double CalcTapStrain(Note nextNote, Note previousNote, float weight, double MIN_TIMEDELTA)
        {
            var tapStrain = 0d;
            if (nextNote.position - (previousNote.position + previousNote.length) > 0)
            {
                var timeDelta = Math.Max(nextNote.position - previousNote.position, MIN_TIMEDELTA);
                var strain = 25f / Math.Pow(timeDelta, 1.2f);
                tapStrain = strain * weight;
            }
            return tapStrain;
        }

        public static double CalcAccStrain(Note nextNote, Note previousNote, float weight, float comboMultiplier, double AVERAGE_NOTE_LENGTH)
        {
            var accStrain = 0d;

            if (nextNote.position - (previousNote.position + previousNote.length) > 0)
            {
                var distance = MathF.Abs(nextNote.pitchStart - previousNote.pitchEnd);
                var accFactor = distance / (nextNote.position - (previousNote.position + previousNote.length));
                var strain = accFactor / AVERAGE_NOTE_LENGTH;
                accStrain = strain * weight * comboMultiplier;
            }
            else if (nextNote.pitchDelta != 0)
            {
                var sliderSpeed = Math.Abs(nextNote.pitchDelta) / nextNote.length;
                var strain = sliderSpeed / AVERAGE_NOTE_LENGTH;
                accStrain = strain * weight * comboMultiplier;
            }


            return accStrain;
        }

        #region Note

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

        public bool CheckIfIsSlider(float gamespeed, int index) => (notesDict[gamespeed][index].position + notesDict[gamespeed][index].length >= notesDict[gamespeed][index + 1].position) || (notesDict[gamespeed][index].pitchDelta != 0);

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

            data.Add("Aim Rating: " + aimPerformanceDict[1].Average(x => x.performance));
            data.Add("Tap Rating: " + tapPerformanceDict[1].Average(x => x.performance));
            data.Add("Acc Rating: " + accPerformanceDict[1].Average(x => x.performance));

            for (int i = 0; i < notesDict[1].Count; i++)
            {
                var n = notesDict[1][i];
                data.Add("nu: " + n.count.ToString("0.00") + " | " +
                    "po: " + n.position.ToString("0.00") + " | " +
                    "le: " + n.length.ToString("0.00") + " | " +
                    "ps: " + n.pitchStart.ToString("0.00") + " | " +
                    "pd: " + n.pitchDelta.ToString("0.00") + " | " +
                    "pe: " + n.pitchEnd.ToString("0.00") +
                    (n.isSlider ? " | Slider" : "") + " | " +
                    (i < aimPerformanceDict.Count ? $"a: {aimPerformanceDict[1][i].performance} t: {tapPerformanceDict[1][i].performance} a: {accPerformanceDict[1][i].performance}" : ""));
            }

            return data;
        }

        public float GetTempoMultiplier() => (float.Parse(tempo) / 100f);
        public static double BeatToSeconds(double time, float bpm)
        {
            return time / bpm * 60f;
        }

        public static double BeatToSeconds2(double beat, float bpm) => (60f / bpm) * beat;

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

        public class DataVectorAnalytics
        {
            public float perfAverage, perfMax, perfMin, perfSum;

            public DataVectorAnalytics(List<DataVector> dataVectorList)
            {
                perfAverage = dataVectorList.Average(x => x.performance);
                perfMax = dataVectorList.Max(x => x.performance);
                perfMin = dataVectorList.Min(x => x.performance);
                perfSum = dataVectorList.Sum(x => x.performance);
            }
        }
    }
}
