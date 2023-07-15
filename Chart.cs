﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
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
        public string songHash;
        public Leaderboard leaderboard;

        public List<Lyrics> lyrics;
        public Dictionary<float, List<DataVector>> aimPerformanceDict;
        public Dictionary<float, DataVectorAnalytics> aimAnalyticsDict;
        public Dictionary<float, List<DataVector>> tapPerformanceDict;
        public Dictionary<float, DataVectorAnalytics> tapAnalyticsDict;
        public Dictionary<float, List<DataVector>> accPerformanceDict;
        public Dictionary<float, DataVectorAnalytics> accAnalyticsDict;
        public Dictionary<float, float> aimRatingDict;
        public Dictionary<float, float> tapRatingDict;
        public Dictionary<float, float> accRatingDict;
        public Dictionary<float, float> starRatingDict;
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
                    notesDict[gamespeed].Add(new Note(count, BeatToSeconds(double.Parse(n[0]), float.Parse(tempo) * gamespeed), BeatToSeconds(double.Parse(n[1]), float.Parse(tempo) * gamespeed), float.Parse(n[2]), float.Parse(n[3]), float.Parse(n[4])));
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
            aimRatingDict = new Dictionary<float, float>();
            tapRatingDict = new Dictionary<float, float>();
            accRatingDict = new Dictionary<float, float>();
            starRatingDict = new Dictionary<float, float>();
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

                var aimRating = aimRatingDict[gamespeed] = aimPerformanceDict[gamespeed].Average(x => x.performance);
                var tapRating = tapRatingDict[gamespeed] = tapPerformanceDict[gamespeed].Average(x => x.performance);
                var accRating = accRatingDict[gamespeed] = accPerformanceDict[gamespeed].Average(x => x.performance);

                if (aimRating != 0 && tapRating != 0 && accRating != 0)
                {
                    var totalRating = aimRating + tapRating + accRating;
                    var aimPerc = aimRating / totalRating;
                    var tapPerc = tapRating / totalRating;
                    var accPerc = accRating / totalRating;
                    var aimWeight = (aimPerc + 0.25f) * 1.25f;
                    var tapWeight = (tapPerc + 0.25f);
                    var accWeight = (accPerc + 0.25f);
                    var totalWeight = aimWeight + tapWeight + accWeight;
                    starRatingDict[gamespeed] = ((aimRating * aimWeight) + (tapRating * tapWeight) + (accRating * accWeight)) / totalWeight;
                }
                else
                    starRatingDict[gamespeed] = 0f;
            }
            stopwatch.Stop();
            calculationTime = stopwatch.Elapsed;

            /*
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
                    cluster.AddNotes(currentNoteList.ToArray());
                    clusters.Add(cluster);
                    currentNoteList.Clear();
                }
            }
            stopwatch1.Stop();
            Trace.WriteLine($"{shortName} process time was {stopwatch1.Elapsed.TotalMilliseconds}ms");
            Trace.WriteLine($"{shortName} cluster count: {clusters.Count} with {breakDistThreshold} distance threshold");
            for (int i = 0; i < clusters.Count; i++)
            {
                if (i > 0)
                    Trace.WriteLine($"break length between {i - 1} and {i}: {clusters[i - 1].GetDistanceFromCluster(clusters[i])}");
                Trace.WriteLine($"cluster #{i} has {clusters[i].noteList.Count} notes and last {clusters[i].GetClusterSize()}s.");
            }
            Trace.WriteLine($"-----------------------------------------------------");
            */
        }

        public void GetLeaderboardFromAPI()
        {
            TootTallyAPIServices.GetChartData(songHash, (leaderboard) => this.leaderboard = leaderboard);
        }

        public float GetDiffRating(float speed)
        {
            if (speed % .25f == 0)
                return starRatingDict[speed];

            var index = (int)((speed - 0.5f) / .25f);
            var minSpeed = GAME_SPEED[index];
            var maxSpeed = GAME_SPEED[index + 1];
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

        public void CalcAllDiff(float gamespeed)
        {
            var MAX_TIME = BeatToSeconds2(0.05, float.Parse(tempo) * gamespeed);
            var MIN_TIMEDELTA = 1d / 240d;
            var AVERAGE_NOTE_LENGTH = notesDict[gamespeed].Average(n => n.length);
            var aimEndurance = 0.4d;
            var tapEndurance = 0.4d;
            var endurance_decay = 1.005d;

            List<double> weights = new List<double>();//Pre calc weights
            for (int i = 0; i < 26; i++)
                weights.Add(FastPow(0.945f, i));


            for (int i = 0; i < notesDict[gamespeed].Count - 1; i++) //Main Forward Loop
            {
                Note currentNote = notesDict[gamespeed][i];
                Note previousNote = currentNote;
                var comboMultiplier = 0.8f;
                var directionMultiplier = 1f;
                var lengthSum = 0d;
                Direction currentDirection = Direction.Null, previousDirection = Direction.Null;

                var lenCount = 0;
                for (int j = i + 1; j < notesDict[gamespeed].Count && j < i + 10; j++)
                {
                    //Combo Calc
                    lengthSum += notesDict[gamespeed][j].length;
                    lenCount++;
                }
                comboMultiplier += (float)lengthSum;
                //Trace.WriteLine($"{name} - CM: " + comboMultiplier);

                //Second Forward Loop up to 26 notes and notes are at max 4 seconds appart
                var aimStrain = 0d;
                var tapStrain = 0d;
                var accStrain = 0d;


                for (int j = i + 1; j < notesDict[gamespeed].Count && j < i + 26 && notesDict[gamespeed][j].position - (currentNote.position + currentNote.length) <= 4; j++)
                {
                    Note nextNote = notesDict[gamespeed][j];
                    var weight = weights[j - i - 1];
                    var endDecayMult = Math.Pow(endurance_decay, weight);
                    if (aimEndurance > 1f)
                        aimEndurance /= endDecayMult;
                    if (tapEndurance > 1f)
                        tapEndurance /= endDecayMult;

                    //Aim Calc
                    aimStrain += Math.Sqrt(CalcAimStrain(nextNote, previousNote, ref currentDirection, ref previousDirection, weight, ref directionMultiplier, aimEndurance, MAX_TIME) * 12f) / 175f;
                    aimEndurance += CalcAimEndurance(nextNote, previousNote, weight, directionMultiplier);

                    //Tap Calc
                    tapStrain += Math.Sqrt(CalcTapStrain(nextNote, previousNote, weight, comboMultiplier, aimEndurance, MIN_TIMEDELTA) * 30f) / 90f;
                    tapEndurance += CalcTapEndurance(nextNote, previousNote, weight);

                    //Acc Calc
                    accStrain += Math.Sqrt(CalcAccStrain(nextNote, previousNote, weight, comboMultiplier, directionMultiplier, AVERAGE_NOTE_LENGTH) * 7f) / 175f; // I can't figure that out yet


                    previousNote = nextNote;

                }
                if (!aimPerformanceDict.ContainsKey(gamespeed))
                {
                    aimPerformanceDict[gamespeed] = new List<DataVector>();
                    tapPerformanceDict[gamespeed] = new List<DataVector>();
                    accPerformanceDict[gamespeed] = new List<DataVector>();
                }

                aimPerformanceDict[gamespeed].Add(new DataVector((float)currentNote.position, (float)aimStrain));
                tapPerformanceDict[gamespeed].Add(new DataVector((float)currentNote.position, (float)tapStrain));
                accPerformanceDict[gamespeed].Add(new DataVector((float)currentNote.position, (float)accStrain));
            }
        }

        public static double CalcAimStrain(Note nextNote, Note previousNote, ref Direction currentDirection, ref Direction previousDirection, double weight, ref float directionMultiplier, double endurance, double MAX_TIME)
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

            var distance = MathF.Abs(nextNote.pitchStart - previousNote.pitchEnd) * 1.5f;
            var t = nextNote.position - (previousNote.position + previousNote.length);
            speed = distance / Math.Max(t, MAX_TIME);
            //Add directionalMultiplier bonus here
            if (previousDirection != currentDirection && currentDirection != Direction.Null)
                directionMultiplier *= 1.02f;

            previousDirection = currentDirection; //update direction before looking at slider


            if (nextNote.pitchDelta != 0) //Calc extra speed if its a slider
            {
                speed += MathF.Abs(nextNote.pitchDelta) / nextNote.length; //This is equal to 0 if its not a slider
                currentDirection = nextNote.pitchDelta > 0 ? Direction.Up : nextNote.pitchDelta < 0 ? Direction.Down : Direction.Null; //Set direction for slider
                if (previousDirection != currentDirection && currentDirection != Direction.Null)
                    directionMultiplier *= 1.08f;

                previousDirection = currentDirection; //update direction from slider
            }
            return speed * weight * directionMultiplier * endurance;
        }

        public static double CalcTapStrain(Note nextNote, Note previousNote, double weight, float comboMultiplier, double endurance, double MIN_TIMEDELTA)
        {
            var tapStrain = 0d;
            if (nextNote.pitchDelta == 0 && nextNote.position - (previousNote.position + previousNote.length) > 0)
            {
                var timeDelta = Math.Max(nextNote.position - previousNote.position, MIN_TIMEDELTA);
                var strain = 8f / Math.Pow(timeDelta, 1.35f);
                tapStrain = strain * weight * comboMultiplier * endurance;
            }
            return tapStrain;
        }

        public static bool CheckDirectionChange(Direction prevDir, Direction currDir) => (prevDir != currDir && prevDir != Direction.Null && currDir != Direction.Null);

        public static double CalcAccStrain(Note nextNote, Note previousNote, double weight, float comboMultiplier, float directionMultiplier, double AVERAGE_NOTE_LENGTH)
        {
            var accStrain = 0d;

            if (nextNote.pitchDelta == 0 && nextNote.position - (previousNote.position + previousNote.length) > 0)
            {
                var distance = MathF.Abs(nextNote.pitchStart - previousNote.pitchEnd);
                var accFactor = distance / Math.Sqrt(0.12f * (nextNote.length / AVERAGE_NOTE_LENGTH));
                var strain = accFactor;
                accStrain = strain * weight * directionMultiplier * comboMultiplier;
            }
            else if (nextNote.pitchDelta != 0)
            {
                var sliderSpeed = Math.Abs(nextNote.pitchDelta) / nextNote.length; //Promote height over speed
                var strain = sliderSpeed / Math.Sqrt(0.12f * (nextNote.length / AVERAGE_NOTE_LENGTH));
                accStrain = strain * weight * comboMultiplier;
            }

            return accStrain;
        }

        public static double CalcAimEndurance(Note nextNote, Note previousNote, double weight, float directionalMultiplier)
        {
            var endurance = 0d;

            var distance = MathF.Abs(nextNote.pitchStart - previousNote.pitchEnd) / 225f;
            var t = nextNote.position - (previousNote.position + previousNote.length);
            var enduranceAimStrain = distance / t;

            if (nextNote.pitchDelta != 0 && nextNote.position - (previousNote.position + previousNote.length) <= 0) //Calc extra speed if its a slider
                enduranceAimStrain += (MathF.Abs(nextNote.pitchDelta) / nextNote.length) / 225f; //This is equal to 0 if its not a slider

            endurance += Math.Sqrt(enduranceAimStrain) / 750f;

            return endurance * weight * directionalMultiplier;
        }

        public static double CalcTapEndurance(Note nextNote, Note previousNote, double weight)
        {
            var endurance = 0d;

            var enduranceTapStrain = 0d;
            if (nextNote.pitchDelta == 0 && nextNote.position - (previousNote.position + previousNote.length) > 0)
            {
                var timeDelta = nextNote.position - previousNote.position;
                enduranceTapStrain = .15f / Math.Pow(timeDelta, 1.05f);
            }

            endurance += Math.Sqrt(enduranceTapStrain) / 750f;

            return endurance * weight;
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
                    (i < aimPerformanceDict[1].Count ? $"a: {aimPerformanceDict[1][i].performance} t: {tapPerformanceDict[1][i].performance} a: {accPerformanceDict[1][i].performance}" : ""));
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


        public class SerializableDiffData
        {
            public SerializableDataVector speed050;
            public SerializableDataVector speed075;
            public SerializableDataVector speed100;
            public SerializableDataVector speed125;
            public SerializableDataVector speed150;
            public SerializableDataVector speed175;
            public SerializableDataVector speed200;
        }

        public class SerializableDataVector
        {
            public List<DataVector> aim;
            public List<DataVector> tap;
            public List<DataVector> acc;
        }
    }
}
