using System;
using System.Diagnostics;

namespace TootTallyDifficultyCalculator2._0
{
    public class TMBChart
    {
        public readonly float[] GAME_SPEED = { .5f, .75f, 1f, 1.25f, 1.5f, 1.75f, 2f };

        public float[][] notes;
        public string[][] bgdata;
        public Dictionary<float, List<Note>> notesDict;
        public List<string> note_color_start;
        public List<string> note_color_end;
        public string endpoint;
        public float savednotespacing;
        public float tempo;
        public string timesig;
        public string trackRef;
        public string name;
        public string shortName;
        public string author;
        public string genre;
        public string description;
        public string difficulty;
        public string year;
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
                var newTempo = tempo * gamespeed;
                var minLength = BeatToSeconds2(0.01f, newTempo);
                int count = 1;
                notesDict[i] = new List<Note>(notes.Length);
                var sortedNotes = notes.OrderBy(x => x[0]).ToArray();
                for (int j = 0; j < sortedNotes.Length; j++)
                {
                    float length = sortedNotes[j][1];
                    if (length <= 0)//minLength only applies if the note is less or equal to 0 beats, else it keeps its "lower than minimum" length
                        length = 0.015f;

                    bool isSlider;
                    if (i > 0)
                        isSlider = notesDict[0][j].isSlider;
                    else
                        isSlider = j + 1 < sortedNotes.Length && IsSlider(sortedNotes[j], sortedNotes[j + 1]);

                    notesDict[i].Add(new Note(count, BeatToSeconds2(sortedNotes[j][0], newTempo), BeatToSeconds2(length, newTempo), sortedNotes[j][2], sortedNotes[j][3], sortedNotes[j][4], isSlider));
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

            /*var breakDistThreshold = 3.5f / (float.Parse(tempo) / 120f);


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
            Trace.WriteLine($"-----------------------------------------------------");*/
        }

        //public float GetDiffRating(float speed) => performances.GetDiffRating(speed);

        public float GetDynamicDiffRating(float percent, float speed, string[] modifiers) => performances.GetDynamicDiffRating(percent, speed, modifiers);

        public float GetAimPerformance(int speed) => performances.aimAnalyticsArray[speed].perfWeightedAverage;
        public float GetTapPerformance(int speed) => performances.tapAnalyticsArray[speed].perfWeightedAverage;
        //public float GetAccPerformance(int speed) => performances.accAnalyticsArray[speed].perfWeightedAverage;

        public double GetStarRating(int speed) => performances.GetDynamicDiffRating(1, speed * .25f + 0.5f);

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

        public static float BeatToSeconds2(float beat, float bpm) => (60f / bpm) * beat;

        public ReplayData ConvertReplayV2(ReplayData replay)
        {
            bool wasSlider = false;
            bool releasedBetweenNotes;
            int currentScore = 0;
            float health = 0; // 0 to 100
            int combo = 0;
            int highestCombo = 0;
            int multiplier = 0; // 0 to 10
            int[] noteTally = new int[5];

            List<dynamic[]> convertedNoteData = new List<dynamic[]>();
            float[] nextNote = null;
            //Loop through all the notes in a chart
            for (int i = 0; i < notes.Length; i++)
            {
                wasSlider = false;
                releasedBetweenNotes = replay.notedata[i][1] == 1;
                float[] currNote = notes[i];
                if (i + 1 < notes.Length)
                    nextNote = notes[i + 1];
                List<LengthAccPair> noteLengths = new List<LengthAccPair>
                {
                    new LengthAccPair(currNote[1], (float)replay.notedata[i][0])
                };

                //Scroll forward until the next note is no longer a slider
                while (i + 1 < notes.Length && nextNote != null && IsSlider(currNote, nextNote))
                {
                    wasSlider = true;
                    currNote = notes[++i];
                    noteLengths.Add(new LengthAccPair(currNote[1], (float)replay.notedata[i][0])); //Create note length and note acc pair to weight later
                    if (i >= noteLengths.Count)
                        break;
                    nextNote = notes[i + 1];
                }

                float noteAcc = 0f;
                float totalLength = 0f;
                if (wasSlider)
                {
                    //Get total length of all slider bodies
                    totalLength = noteLengths.Select(x => x.length).Sum();
                    for (int j = 0; j < noteLengths.Count; j++)
                        noteAcc += noteLengths[j].acc * (noteLengths[j].length / totalLength); //Length weighted acc sum of all slider bodies
                }
                else
                {
                    //If its not a slider, just take the acc and length of it
                    noteAcc = replay.notedata[i][0];
                    totalLength = currNote[1];
                }

                //Calc the score before doing the combo and health because fucking base game logic is MIND BLOWING I know
                currentScore += GetScore(noteAcc, totalLength, multiplier, health == 100);

                //Calc new health
                var healthDiff = releasedBetweenNotes ? GetHealthDiff(noteAcc) : -15f;

                if (health == 100 && healthDiff < 0)
                    health = 0;
                else if (health != 100)
                    health += healthDiff;
                health = Math.Clamp(health, 0, 100);

                //Get the note tally
                int tally = 0;
                if (noteAcc > 95f) tally = 4;
                else if (noteAcc > 88f) tally = 3;
                else if (noteAcc > 79f) tally = 2;
                else if (noteAcc > 70f) tally = 1;
                noteTally[4 - tally]++;
                //Only increase combo if you get more than 79% acc + update highest if needed
                if (tally > 2)
                {
                    if (++combo > highestCombo)
                        highestCombo = combo;
                }
                else
                    combo = 0;

                multiplier = Math.Min(combo, 10);

                convertedNoteData.Add(new dynamic[9]
                {
                    noteAcc,
                    releasedBetweenNotes ? 1 : 0,
                    i,
                    combo,
                    multiplier,
                    currentScore,
                    health,
                    highestCombo,
                    tally
                });
            }

            replay.notedata = convertedNoteData;
            replay.finalnotetallies = noteTally;
            replay.finalscore = convertedNoteData.Last()[5];
            replay.maxcombo = highestCombo;

            return replay;
        }

        public static bool IsSlider(float[] currNote, float[] nextNote) => currNote[0] + currNote[1] + .025f >= nextNote[0];
        public static float GetHealthDiff(float acc) => Math.Clamp((acc - 79f) * 0.2193f, -15f, 4.34f);
        public static bool TestMissTheGap(float health, int tally) => false; //TODO
        public static int GetScore(float acc, float totalLength, float mult, bool champ)
        {
            var baseScore = Math.Clamp(totalLength, 0.2f, 5f) * 8f + 10f;
            return (int)Math.Floor(baseScore * acc * ((mult + (champ ? 1.5f : 0f)) * .1f + 1f)) * 10;
        }

        public class LengthAccPair
        {
            public float length, acc;

            public LengthAccPair(float length, float acc)
            {
                this.length = length;
                this.acc = acc;
            }
        }
    }
}
