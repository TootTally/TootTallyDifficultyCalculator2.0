using Newtonsoft.Json;
using System.Diagnostics;
using static TootTallyDifficultyCalculator2._0.ChartPerformances;

namespace TootTallyDifficultyCalculator2._0
{
    public partial class MainForm : Form
    {
        public static List<TMBChart> chartList;
        public static List<Leaderboard> leaderboardList;
        private TimeSpan _deserializingTime;
        private TimeSpan _algoTime;
        private TimeSpan _leaderboardLoadingTime;
        private TimeSpan _leaderboardCalcTime;
        public static MainForm Instance;
        public static float[] WEIGHTS;
        public const bool DO_LEADERBOARDS = true;

        public MainForm()
        {
            InitializeComponent();
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzE5NDM0MkAzMjM0MmUzMDJlMzBReUtYUlhkZkFtaks2L1V0RWcyNU1qakdLZ09RaU52c3M5ZkpqdnFoL0Y4PQ==");
            Directory.CreateDirectory(Program.EXPORT_DIRECTORY);
            Directory.CreateDirectory(Program.DOWNLOAD_DIRECTORY);
            Directory.CreateDirectory(Program.CACHE_DIRECTORY);
            WEIGHTS = new float[64];
            for (int i = 0; i < WEIGHTS.Length; i++)
                WEIGHTS[i] = FastPow(.92f, i);
            Instance ??= this;
        }

        public float GetAimNum() => (float)AimNum.Value;
        public float GetTapNum() => (float)TapNum.Value;
        public float GetAccNum() => (float)AccNum.Value;
        public float GetAimEndNote() => (float)AimEndNote.Value;
        public float GetAimEndSlider() => (float)AimEndSlider.Value;
        public float GetAimEndMult() => (float)AimEndMult.Value;
        public float GetTapEndMult() => (float)TapEndMult.Value;
        public float GetRatingOffset() => (float)RatingOffset.Value;
        public float GetMaxTime() => (float)MaxTime.Value;

        public float GetEndDrain() => (float)EndDrain.Value;
        public float GetEndDrainExtra() => (float)EndDrainExtra.Value;
        public float GetBiasMult() => (float)BiasMult.Value;

        public float GetMacc() => (float)MaccValue.Value / 100f;
        public float GetMap() => (float)MapValue.Value / 100f;

        public float[] GetHDWeights() => new float[] { (float)HDAimWeight.Value, (float)HDTapWeight.Value };
        public float[] GetFLWeights() => new float[] { (float)FLAimWeight.Value, (float)FLTapWeight.Value };
        public float[] GetEZWeights() => new float[] { (float)EZAimWeight.Value, (float)EZTapWeight.Value };

        public async void DownloadAllTmbs(object sender, EventArgs e)
        {
            List<int> idList = TootTallyAPIServices.GetAllRatedChartIDs();
            List<Leaderboard.SongInfoFromDB> fileHashes = ChartReader.GetCachedFileHashes();
            fileHashes ??= new();
            var filteredIDs = idList.Where(id => fileHashes.Any(x => x != null && x.id == id)).ToList();

            Trace.WriteLine("Waiting for all leaderboards...");
            leaderboardList = new List<Leaderboard>(idList.Count);

            var maxCount = idList.Count;
            var currentCount = 0;
            ProgressBarLoading.Maximum = maxCount;
            ProgressBarLoading.Value = 0;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            if (DO_LEADERBOARDS)
                Parallel.ForEach(idList, new ParallelOptions() { MaxDegreeOfParallelism = 12 }, id =>
                {
                    TootTallyAPIServices.GetLeaderboardFromId(id, leaderboard =>
                    leaderboardList.Add(leaderboard));
                    currentCount++;
                    if (!ProgressBarLoading.InvokeRequired)
                    {
                        UpdateProgressBar(currentCount, maxCount);
                    }
                });
            Trace.WriteLine("Leaderboards finished processing.");
            stopwatch.Stop();
            _leaderboardLoadingTime = stopwatch.Elapsed;

            List<string> urls = new List<string>(leaderboardList.Count);
            Parallel.ForEach(leaderboardList, l =>
            {
                if (l.results.Count > 0 && !filteredIDs.Any(id => id == l.song_info.id) && l.song_info != null)
                {
                    urls.Add($"{l.song_info.file_hash}.tmb");
                    fileHashes.Add(new Leaderboard.SongInfoFromDB() { id = l.song_info.id, file_hash = l.song_info.file_hash, track_ref = l.song_info.track_ref });
                }
            });

            if (urls.Count > 0)
            {
                Trace.WriteLine($"{urls.Count} new file hashes found, saving to cache");
                ChartReader.SaveCacheFileHashes(fileHashes);

                Trace.WriteLine("Waiting for all TMBS downloads...");
                var t3 = await Task.Run(() => TootTallyAPIServices.GetAllTmbsJson(urls.ToArray()));

                Parallel.ForEach(t3, file =>
                {
                    var fileName = ChartReader.LoadChartFromJson(file).trackRef;
                    if (!File.Exists(Program.DOWNLOAD_DIRECTORY + fileName.Replace('/', '-').Replace('.', '_') + ".tmb"))
                        ChartReader.SaveChartData(Program.DOWNLOAD_DIRECTORY + fileName.Replace('/', '-').Replace('.', '_') + ".tmb", file);
                });
                Trace.WriteLine("Tmbs all downloaded.");
            }
            OnForceRefreshClick(sender, e);

            ProgressBarLoading.Visible = false;
            LoadingLabel.Visible = false;
        }

        public void LoadAllCharts()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var filesList = Directory.GetFiles(Program.DOWNLOAD_DIRECTORY);
            chartList = new List<TMBChart>(filesList.Length);
            var maxCount = filesList.Length;
            var currentCount = 0;
            ProgressBarLoading.Maximum = maxCount;
            ProgressBarLoading.Value = 0;
            Parallel.ForEach(filesList, name =>
            {
                chartList.Add(ChartReader.LoadChart(name));
                currentCount++;
                if (!ProgressBarLoading.InvokeRequired)
                {
                    UpdateProgressBar(currentCount, maxCount);
                }
            });
            chartList.Sort((x, y) => String.Compare(x.name, y.name));
            _deserializingTime = stopwatch.Elapsed;
            stopwatch.Restart();
            Parallel.ForEach(chartList, chart => chart.OnDeserialize());
            _algoTime = stopwatch.Elapsed;
            ChartNameCB.Items.Clear();
            chartList.ForEach(c => ChartNameCB.Items.Add(c.shortName));

        }

        public void UpdateProgressBar(int value, int maxValue)
        {
            ProgressBarLoading.Value = value;
            float percent = (value / (float)maxValue) * 100f;
            LoadingLabel.Text = $"LOADING {percent:0.00}%";
            this.Update();
        }

        public void AsignLeaderboardsToCharts()
        {
            var maxCount = leaderboardList.Count;
            var currentCount = 0;
            ProgressBarLoading.Maximum = maxCount;
            ProgressBarLoading.Value = 0;
            Parallel.ForEach(leaderboardList, new ParallelOptions() { MaxDegreeOfParallelism = 12 }, l =>
            {
                var chart = chartList.Find(chart => chart.trackRef == l.song_info.track_ref);
                if (chart != null) { chart.leaderboard = l; }
                currentCount++;
                if (!ProgressBarLoading.InvokeRequired)
                {
                    UpdateProgressBar(currentCount, maxCount);
                }
            });
        }

        public void ExportChartToJson(string path, TMBChart chart)
        {
            SerializableDiffData chartdata = new SerializableDiffData(chart.performances);
            var json = JsonConvert.SerializeObject(chartdata, Formatting.Indented);
            ChartReader.SaveChartData(path, json);
        }

        private void OnForceRefreshClick(object sender, EventArgs e)
        {
            LoadAllCharts();
            AsignLeaderboardsToCharts();
            OnDisplayChartsButtonClick(sender, e);

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
        }

        private void OnDisplayChartsButtonClick(object sender, EventArgs e)
        {
            var entryCount = 0;
            TextBoxChartData.Clear();
            TextBoxLeaderboardData.Clear();
            var allChartDataTextLines = new List<string>((checkboxAllSpeed.Checked ? 40 : 7) * chartList.Count);
            var allLeaderboardTextLines = new List<string>();

            allChartDataTextLines.Add($"Json Calculation time took {_deserializingTime.TotalSeconds}s for {chartList.Count} charts and {chartList.Count * 7} diffs");
            allChartDataTextLines.Add($"Algo Calculation time took {_algoTime.TotalSeconds}s for {chartList.Count} charts and {chartList.Count * 7} diffs");
            allLeaderboardTextLines.Add($"Calculation time took {_leaderboardLoadingTime.TotalSeconds}s for {chartList.Count} charts.");
            Stopwatch sw = Stopwatch.StartNew();
            foreach (TMBChart chart in chartList)
            {
                if (!chart.name.ToLower().Contains(FilterMapName.Text.ToLower())) continue;
                //CHART DISPLAY
                var chartTextLines = new List<string>()
                {
                    $"{chart.name} processed in {chart.calculationTime.TotalSeconds}s"
                };

                if (checkboxAllSpeed.Checked)
                    DisplayAllSpeed(chart, ref chartTextLines);
                else
                    DisplayAtSpeed(chart, 2, ref chartTextLines);
                chartTextLines.Add("==========================================================================================================");
                chartTextLines.Add("");
                allChartDataTextLines.AddRange(chartTextLines);

                //LEADERBOARD DISPLAY
                if (chart.leaderboard == null) continue;
                var leaderboardText = DisplayLeaderboard(ref entryCount, chart);
                if (leaderboardText.Count == 0) continue;

                var leaderboardTextLines = new List<string>
                    {
                        $"{chart.name} processed in {chart.calculationTime.TotalSeconds}s",
                        GetLeaderboardScoreHeader(),
                        "----------------------------------------------------------------------------------------------------------"
                    };

                leaderboardTextLines.AddRange(leaderboardText);
                leaderboardTextLines.Add("==========================================================================================================");
                leaderboardTextLines.Add("");
                allLeaderboardTextLines.AddRange(leaderboardTextLines);

            }
            sw.Stop();
            _leaderboardCalcTime = sw.Elapsed;
            allLeaderboardTextLines.Insert(1, $"Score Calculation time took {_leaderboardCalcTime.TotalSeconds}s for {chartList.Count} charts and {entryCount} entries.");
            TextBoxChartData.Lines = allChartDataTextLines.ToArray();
            TextBoxLeaderboardData.Lines = allLeaderboardTextLines.ToArray();
        }

        public void DisplayAllSpeed(TMBChart chart, ref List<string> textLines)
        {
            for (int i = 0; i < chart.GAME_SPEED.Length; i++)
                DisplayAtSpeed(chart, i, ref textLines);
        }

        public void DisplayAtSpeed(TMBChart chart, int speedIndex, ref List<string> textLines)
        {
            DataVectorAnalytics aimAnalytics = chart.performances.aimAnalyticsArray[speedIndex];
            DataVectorAnalytics tapAnalytics = chart.performances.tapAnalyticsArray[speedIndex];
            //DataVectorAnalytics accAnalytics = chart.performances.accAnalyticsArray[speedIndex];
            textLines.Add($"SPEED: {chart.GAME_SPEED[speedIndex]:0.00}x rated {chart.GetStarRating(speedIndex):0.0000}) ({chart.performances.aimAnalyticsArray[speedIndex].weightSum} - {chart.songLengthMult}x)");
            textLines.Add($"  aim: {aimAnalytics.perfWeightedAverage:0.0000} min: {aimAnalytics.perfMin:0.0000} max: {aimAnalytics.perfMax:0.0000}");
            textLines.Add($"  tap: {tapAnalytics.perfWeightedAverage:0.0000} min: {tapAnalytics.perfMin:0.0000} max: {tapAnalytics.perfMax:0.0000}");
            //textLines.Add($"  acc: {accAnalytics.perfWeightedAverage:0.0000} min: {accAnalytics.perfMin:0.0000} max: {accAnalytics.perfMax:0.0000}");
            textLines.Add("-------------------------------------------------");
        }

        public List<string> DisplayLeaderboard(ref int entryCount, TMBChart chart)
        {
            var count = 1;
            var entryCountIncrement = 0;
            List<string> textLines = new List<string>();
            for (int i = 0; i < chart.leaderboard.results.Count; i++)
                chart.leaderboard.results[i].tt = (float)CalculateScoreTT(chart, chart.leaderboard.results[i]);
            chart.leaderboard?.results.OrderByDescending(s => s.tt).ToList().ForEach(score =>
                {
                    //score.tt = (float)CalculateScoreTT(chart, score);
                    if (!FilterModifierOnly.Checked || (FilterModifierOnly.Checked && score.modifiers != null && !score.modifiers.Contains("NONE")))
                        if (!FilterEZOnly.Checked || (FilterEZOnly.Checked && score.modifiers != null && score.modifiers.Contains("EZ")))
                            if (score.tt >= (float)FilterMinTT.Value && score.tt <= (float)FilterMaxTT.Value && score.player.ToLower().Contains(FilterPlayerName.Text.ToLower()) && score.percentage >= (float)AccFilter.Value * 100)
                            {
                                entryCountIncrement++;
                                textLines.Add(GetDisplayScoreLine2(score, chart, count));
                            }
                    count++;
                });
            entryCount += entryCountIncrement;
            return textLines;
        }

        public string GetDisplayScoreLine(Leaderboard.ScoreDataFromDB score, TMBChart chart, int count) =>
            $"#{count} {score.player}\t\t{score.score}\t\t({score.replay_speed:0.00}x)\t {score.percentage:0.00}%\t{score.grade}\t{score.tt:0.00}tt\tdiff:{chart.GetDynamicDiffRating(score.GetHitCount, score.replay_speed, score.modifiers):0.00}";

        public string GetDisplayScoreLine2(Leaderboard.ScoreDataFromDB score, TMBChart chart, int count) =>
            FormatLeaderboardScore(count.ToString(), score.player, score.score.ToString(), score.replay_speed.ToString("0.00"), score.percentage.ToString("0.00"), score.grade, score.tt.ToString("0.00"), chart.GetDynamicDiffRating(score.GetHitCount, score.replay_speed, score.modifiers).ToString("0.00"), score.modifiers);

        public string FormatLeaderboardScore(string count, string player, string score, string replaySpeed, string percentage, string grade, string tt, string diff, string[] modifiers)
        {
            return String.Format("{0,-4} | {1,-30} | {2, -11} | {3, -8} | {4, -6} | {5, -5} | {6, -10} | {7, 5} | {8, 4} |",
                $"#{count}",
                $"{player}",
                $"{score}",
                $"({replaySpeed}x)",
                $"{percentage}%",
                $"{grade}",
                $"{tt}tt",
                $"{diff}",
                $"{(modifiers != null ? string.Join(',', modifiers) : "NONE")}"
                );
        }

        public string GetLeaderboardScoreHeader()
        {
            return String.Format("{0,-4} | {1,-30} | {2, -11} | {3, -8} | {4, -6} | {5, -5} | {6, -10} | {7, 5} | {8, 4} |",
                $"Rank",
                $"Name",
                $"Score",
                $"Speed",
                $"Perc",
                $"Grade",
                $"TT",
                $"Diff",
                $"Mods"
                );
        }

        //https://www.desmos.com/calculator/cemkp1quua
        //NEW x^3 https://www.desmos.com/calculator/ciazumjpbx
        public static float CalculateBaseTT(float starRating)
        {
            return (.03f * FastPow(starRating, 3)) - (.13f * FastPow(starRating, 2)) + (10f * starRating) + .05f;
            //y = 0.13x^3 -1.4x^2 + 12x + .05
        }
        public static float CalculateBaseTTOLD(float starRating)
        {
            return 0.5f * FastPow(starRating, 2) + (7f * starRating) + 0.05f;
            //y = 0.7x^2 + 12x + 0.05
        }

        public const float c = 0.734992228f;
        public const float b = 2.8f;

        // OLD: https://www.desmos.com/calculator/mvwr1tcpz8
        // NEW: https://www.desmos.com/calculator/0su982a1gg
        public static double CalculateScoreTT(TMBChart chart, Leaderboard.ScoreDataFromDB score)
        {
            var percent = score.percentage / 100f;

            var baseTT = CalculateBaseTT(chart.GetDynamicDiffRating(score.GetHitCount, score.replay_speed, score.modifiers));

            return baseTT * GetMultiplier(percent, score.modifiers);
        }

        public static readonly Dictionary<float, float> accToMultDictV1 = new Dictionary<float, float>()
        {
            { 1f, 22.095f },
            { .999f, 21.771f },
            { .996f, 20.819f },
            { .993f, 19.899f },
            { .99f, 19.010f },
            { .985f, 17.595f },
            { .98f, 16.260f },
            { .97f, 13.819f },
            { .96f, 11.665f },
            { .95f, 9.772f },
            { .92f, 8.399f },
            { .9f, 7.592f },
            { .8f, 4.571f },
            { .7f, 2.739f },
            { .6f, 1.627f },
            { .5f, 0.953f },
            { .25f, 0.212f },
            { 0, 0 },
        };

        public static readonly Dictionary<float, float> accToMultDictV2 = new Dictionary<float, float>()
        {
            { 1f, 22.1f },
            { .999f, 21.7f },
            { .996f, 20.8f },
            { .993f, 19.8f },
            { .99f, 19f },
            { .985f, 17.5f },
            { .98f, 16.2f },
            { .97f, 13.8f },
            { .96f, 12f },
            { .95f, 11f },
            { .925f, 8.9f },
            { .875f, 6.8f },
            { .8f, 4.5f },
            { .7f, 2.7f },
            { .6f, 1.4f },
            { .5f, 0.6f },
            { .25f, 0.2f },
            { 0, 0 },
        };

        public static readonly Dictionary<float, float> accToMultDict = new Dictionary<float, float>()
        {
            { 1f, 40.2f },
            { .999f, 32.4f },
            { .996f, 27.2f },
            { .993f, 23.2f },
            { .99f, 20.5f },
            { .985f, 18.1f },
            { .98f, 16.1f },
            { .97f, 13.8f },
            { .96f, 11.8f },
            { .95f, 10.8f },
            { .925f, 9.2f },
            { .9f, 8.2f },
            { .875f, 7.5f },
            { .85f, 7f },
            { .8f, 6f },
            { .7f, 4f },
            { .6f, 2.2f },
            { .5f, 0.65f },
            { .25f, 0.2f },
            { 0, 0 },
        };

        public static readonly Dictionary<float, float> ezAccToMultDict = new Dictionary<float, float>()
        {
             { 1f, 15.4f },    //{ 1f, 15.4f },    //{ 1f, 15.4f },    //V3{ 1f, 10.4f },   //V2{ 1f, 7.5f },   //V1{ 1f, 7.1f },
             { .999f, 12.6f }, //{ .999f, 12.6f }, //{ .999f, 12.6f }, //{ .999f, 10.2f },  //{ .999f, 6.05f }, //{ .999f, 7f },
             { .996f, 11.6f }, //{ .996f, 10.8f }, //{ .996f, 10.6f }, //{ .996f, 9.8f },   //{ .996f, 5.05f }, //{ .996f, 6.8f },
             { .993f, 11f }, //{ .993f, 10.6f },  //{ .993f, 9.2f },  //{ .993f, 9.5f },   //{ .993f, 4.35f }, //{ .993f, 6.6f },
             { .99f, 10.6f },   //{ .99f, 9.2f },   //{ .99f, 8.6f },   //{ .99f, 9.2f },    //{ .99f, 3.8f },   //{ .99f, 6.4f },
             { .985f, 10f },  //{ .985f, 8.6f },    //{ .985f, 8f },    //{ .985f, 8.75f },  //{ .985f, 3.4f },  //{ .985f, 6.2f },
             { .98f, 9.6f },   //{ .98f, 8.2f },   //{ .98f, 7.6f },   //{ .98f, 8.3f },    //{ .98f, 3f },     //{ .98f, 5.9f },
             { .97f, 9f },   //{ .97f, 7.6f },     //{ .97f, 7f },     //{ .97f, 7.5f },    //{ .97f, 2.5f },   //{ .97f, 5.45f },
             { .96f, 8.6f },   //{ .96f, 7.2f },   //{ .96f, 6.6f },   //{ .96f, 6.6f },    //{ .96f, 2.2f },   //{ .96f, 5.15f },
             { .95f, 8.3f },   //{ .95f, 6.9f },   //{ .95f, 6.2f },   //{ .95f, 6f },      //{ .95f, 2f },     //{ .95f, 4.75f },
             { .925f, 7.6f },  //{ .925f, 6.1f },  //{ .925f, 5.5f },  //{ .925f, 5.25f },  //{ .925f, 1.75f }, //{ .925f, 4.1f },
             { .9f, 6.8f },    //{ .9f, 5.5f },    //{ .9f, 4.8f },    //{ .9f, 4.65f },    //{ .9f, 1.55f },   //{ .9f, 3.6f },
             { .875f, 6.2f },  //{ .875f, 4.8f },  //{ .875f, 4.2f },  //{ .875f, 4.35f },  //{ .875f, 1.45f }, //{ .875f, 3.1f },
             { .85f, 5.6f },   //{ .85f, 4.2f },   //{ .85f, 3.8f },   //{ .85f, 3.9f },    //{ .85f, 1.3f },   //{ .85f, 2.6f },
             { .8f, 4.6f },    //{ .8f, 3.3f },      //{ .8f, 3f },      //{ .8f, 3.45f },    //{ .8f, 1.15f },   //{ .8f, 2.1f },
             { .7f, 2.5f },   //{ .7f, 1.95f },   //{ .7f, 1.75f },   //{ .7f, 2.25f },    //{ .7f, .75f },    //{ .7f, 1.4f },
             { .6f, 1.12f },    //{ .6f, .84f },    //{ .6f, .84f },    //{ .6f, 1.23f },    //{ .6f, .41f },    //{ .6f, .8f },
             { .5f, .22f },    //{ .5f, .22f },    //{ .5f, .22f },    //{ .5f, .33f },     //{ .5f, .11f },    //{ .5f, .4f },
             { .25f, .03f },   //{ .25f, .03f },   //{ .25f, .03f },   //{ .25f, .06f },    //{ .25f, .02f },   //{ .25f, .05f },
             { 0, 0 },         //{ 0, 0 },         //{ 0, 0 },         //{ 0, 0 },          //{ 0, 0 },         //{ 0, 0 },
        };
        public static float GetMultiplier(float percent, string[] modifiers = null)
        {
            var multDict = (modifiers != null && modifiers.Contains("EZ")) ? ezAccToMultDict : accToMultDict;
            int index;
            for (index = 1; index < multDict.Count && multDict.Keys.ElementAt(index) > percent; index++) ;
            var percMax = multDict.Keys.ElementAt(index);
            var percMin = multDict.Keys.ElementAt(index - 1);
            var by = (percent - percMin) / (percMax - percMin);
            var mult = Lerp(multDict[percMin], multDict[percMax], by);
            return mult;
        }

        private void OnDropDownSongNameValueChange(object sender, EventArgs e)
        {
            ButtonForceRefresh.Visible = true;
        }


        private void OnTextBoxTextChanged(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
                OnDisplayChartsButtonClick(sender, e);
        }

        private void OnSwitchBoxClick(object sender, EventArgs e)
        {
            TextBoxChartData.Visible = !TextBoxChartData.Visible;
            TextBoxLeaderboardData.Visible = !TextBoxLeaderboardData.Visible;

        }

        private void OnFormShown(object sender, EventArgs e)
        {
            DownloadAllTmbs(sender, e);
        }

        private void OnValueBoxTextChanged(object sender, EventArgs e)
        {
            OnDisplayChartsButtonClick(sender, e);
        }

        private void OnSaveToButtonPress(object sender, EventArgs e)
        {
            ChartReader.SaveChartData($"{Program.EXPORT_DIRECTORY}ChartRatings{DateTime.Now:yyyyMMddHHmmss}.txt", string.Join('\n', TextBoxChartData.Lines) + '\n');
            ChartReader.SaveChartData($"{Program.EXPORT_DIRECTORY}Leaderboards{DateTime.Now:yyyyMMddHHmmss}.txt", string.Join('\n', TextBoxLeaderboardData.Lines) + '\n');
        }

        private void OnSelectedValueChanged(object sender, EventArgs e)
        {
            var chart = chartList.Find(x => x.shortName == ChartNameCB.Text);
            if (chart != null)
            {
                var g = new GraphForm(chart);
                g.Show();
            }
        }

        private void OnReplaySelectedValueChanged(object sender, EventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}