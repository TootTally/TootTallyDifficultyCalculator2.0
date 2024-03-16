using Newtonsoft.Json;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
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

        public MainForm()
        {
            InitializeComponent();
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NAaF1cXmhLYVJ+WmFZfVpgd19FZFZSTWY/P1ZhSXxXdkdiWH5bc3BWT2VUWEE=");
            Directory.CreateDirectory(Program.EXPORT_DIRECTORY);
            Directory.CreateDirectory(Program.DOWNLOAD_DIRECTORY);
            Directory.CreateDirectory(Program.CACHE_DIRECTORY);
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
            LoadAllCharts();
            AsignLeaderboardsToCharts();
            OnDisplayChartsButtonClick(sender, e);

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
        }

        private void OnDisplayChartsButtonClick(object sender, EventArgs e)
        {
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
                var leaderboardText = DisplayLeaderboard(chart);
                List<string> leaderboardTextLines = new();
                if (chart.leaderboard == null || leaderboardText.Count == 0)
                    continue;
                else if (chart.leaderboard != null)
                {
                    leaderboardTextLines = new List<string>
                    {
                        $"{chart.name} processed in {chart.calculationTime.TotalSeconds}s",
                        GetLeaderboardScoreHeader(),
                        "----------------------------------------------------------------------------------------------------------"
                    };
                }

                leaderboardTextLines.AddRange(leaderboardText);
                leaderboardTextLines.Add("==========================================================================================================");
                leaderboardTextLines.Add("");
                allLeaderboardTextLines.AddRange(leaderboardTextLines);

            }
            sw.Stop();
            _leaderboardCalcTime = sw.Elapsed;
            allLeaderboardTextLines.Insert(1, $"Score Calculation time took {_leaderboardCalcTime.TotalSeconds}s for {chartList.Count} charts.");
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
            textLines.Add($"SPEED: {chart.GAME_SPEED[speedIndex]:0.00}x rated {chart.GetStarRating(speedIndex):0.0000}");
            textLines.Add($"  aim: {aimAnalytics.perfWeightedAverage:0.0000} min: {aimAnalytics.perfMin:0.0000} max: {aimAnalytics.perfMax:0.0000}");
            textLines.Add($"  tap: {tapAnalytics.perfWeightedAverage:0.0000} min: {tapAnalytics.perfMin:0.0000} max: {tapAnalytics.perfMax:0.0000}");
            //textLines.Add($"  acc: {accAnalytics.perfWeightedAverage:0.0000} min: {accAnalytics.perfMin:0.0000} max: {accAnalytics.perfMax:0.0000}");
            textLines.Add("-------------------------------------------------");
        }

        public List<string> DisplayLeaderboard(TMBChart chart)
        {
            var count = 1;
            List<string> textLines = new List<string>();
            chart.leaderboard?.results.OrderByDescending(s => s.tt).ToList().ForEach(score =>
                {
                    score.tt = (float)CalculateScoreTT(chart, score);
                    if (!FilterModifierOnly.Checked || (FilterModifierOnly.Checked && score.modifiers != null && !score.modifiers.Contains("NONE")))
                        if (score.tt >= (float)FilterMinTT.Value && score.tt <= (float)FilterMaxTT.Value && score.player.ToLower().Contains(FilterPlayerName.Text.ToLower()))
                            textLines.Add(GetDisplayScoreLine2(score, chart, count));
                    count++;
                });
            return textLines;
        }

        public string GetDisplayScoreLine(Leaderboard.ScoreDataFromDB score, TMBChart chart, int count) =>
            $"#{count} {score.player}\t\t{score.score}\t\t({score.replay_speed:0.00}x)\t {score.percentage:0.00}%\t{score.grade}\t{score.tt:0.00}tt\tdiff:{chart.GetDynamicDiffRating(score.percentage / 100f, score.replay_speed, score.modifiers):0.00}";

        public string GetDisplayScoreLine2(Leaderboard.ScoreDataFromDB score, TMBChart chart, int count) =>
            FormatLeaderboardScore(count.ToString(), score.player, score.score.ToString(), score.replay_speed.ToString("0.00"), score.percentage.ToString("0.00"), score.grade, score.tt.ToString("0.00"), chart.GetDynamicDiffRating(score.percentage / 100f, score.replay_speed, score.modifiers).ToString("0.00"), score.modifiers);

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

        //TT for S rank (60% score)
        //https://www.desmos.com/calculator/jaeyctccxg
        public static float CalculateBaseTT(float starRating)
        {

            return 0.7f * FastPow(starRating, 2) + (12f * starRating) + 0.05f;
            //y = 0.7x^2 + 12x + 0.05
        }

        public const float c = 0.028091281f;
        public const float b = 6f;

        //https://www.desmos.com/calculator/x7c0zutgsn
        public static double CalculateScoreTT(TMBChart chart, Leaderboard.ScoreDataFromDB score)
        {
            var percent = score.percentage / 100f;

            var baseTT = CalculateBaseTT(chart.GetDynamicDiffRating(percent, score.replay_speed, score.modifiers));

            float scoreTT;
            if (percent < 0.98f)
                scoreTT = ((c * MathF.Pow(MathF.E, b * percent)) - c) * baseTT; //y = (0.28091281 * e^6x - 0.028091281) * b
            else
                scoreTT = FastPow(9.2f * percent - 7.43037117f, 5) * baseTT;

            return scoreTT;
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
            string date = DateTime.Now.ToString("yyyyMMddHHmmss");
            string fileName = Program.EXPORT_DIRECTORY + "ChartRatings" + date + ".txt";
            string text = "";
            TextBoxChartData.Lines.ToList().ForEach(line => { text += line + "\n"; });
            ChartReader.SaveChartData(fileName, text);
            fileName = Program.EXPORT_DIRECTORY + "Leaderboards" + date + ".txt";
            text = "";
            TextBoxLeaderboardData.Lines.ToList().ForEach(line => { text += line + "\n"; });
            ChartReader.SaveChartData(fileName, text);
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
    }
}