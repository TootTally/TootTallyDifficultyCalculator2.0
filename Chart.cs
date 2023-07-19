using System;
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

        public ChartPerformances performances;

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

            performances = new ChartPerformances(this);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < GAME_SPEED.Length; i++)
            {
                performances.CalculatePerformances(GAME_SPEED[i]);
                performances.CalculateAnalytics(GAME_SPEED[i]);
                performances.CalculateRatings(GAME_SPEED[i]);
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
            TootTallyAPIServices.GetChartData(this, (leaderboard) => this.leaderboard = leaderboard);
        }

        public float GetDiffRating(float speed) => performances.GetDiffRating(speed);

        public float GetAimPerformance(float speed) => performances.aimAnalyticsDict[speed].perfAverage;
        public float GetAimEndPerformance(float speed) => performances.aimEndPerfDict[speed].Average(x=>x.performance);
        public float GetTapPerformance(float speed) => performances.tapAnalyticsDict[speed].perfAverage;
        public float GetTapEndPerformance(float speed) => performances.tapEndPerfDict[speed].Average(x=>x.performance);
        public float GetAccPerformance(float speed) => performances.accAnalyticsDict[speed].perfAverage;

        public float GetStarRating(float speed) => performances.starRatingDict[speed];
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

        public float GetTempoMultiplier() => (float.Parse(tempo) / 100f);
        public static double BeatToSeconds(double time, float bpm)
        {
            return time / bpm * 60f;
        }

        public static double BeatToSeconds2(double beat, float bpm) => (60f / bpm) * beat;

    }
}
