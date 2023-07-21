using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
                var newTempo = float.Parse(tempo) * gamespeed;
                var minLength = BeatToSeconds2(0.01, newTempo);
                int count = 1;
                notesDict[i] = new List<Note>(notes.Length);
                foreach (string[] n in notes)
                {
                    notesDict[i].Add(new Note(count, BeatToSeconds2(double.Parse(n[0]), newTempo), BeatToSeconds2(double.Parse(n[1]), newTempo), float.Parse(n[2]), float.Parse(n[3]), float.Parse(n[4])));
                    if (notesDict[i].Last().length == 0)
                        notesDict[i].Last().length = minLength;
                    count++;
                }
            }

            performances = new ChartPerformances(this);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < GAME_SPEED.Length; i++)
            {
                performances.CalculatePerformances(i);
                performances.CalculateAnalytics(i);
                performances.CalculateRatings(i);
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

        public double GetDiffRating(float speed) => performances.GetDiffRating(speed);

        public double GetAimPerformance(float speed) => performances.aimAnalyticsDict[speed].perfAverage;
        public double GetTapPerformance(float speed) => performances.tapAnalyticsDict[speed].perfAverage;
        public double GetAccPerformance(float speed) => performances.accAnalyticsDict[speed].perfAverage;

        public double GetStarRating(float speed) => performances.starRatingDict[speed];
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
        public static double BeatToSeconds(double time, float bpm)
        {
            return time / bpm * 60d;
        }

        public static double BeatToSeconds2(double beat, float bpm) => (60d / bpm) * beat;

    }
}
