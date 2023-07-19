using Newtonsoft.Json;
using System.CodeDom;
using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using static System.Formats.Asn1.AsnWriter;
using static TootTallyDifficultyCalculator2._0.ChartPerformances;

namespace TootTallyDifficultyCalculator2._0
{
    public partial class MainForm : Form
    {
        private List<string> _fileNameLists;
        private List<string> _replayNameLists;
        private Chart _currentChart;
        private ReplayData _currentReplay;
        public List<Chart> chartList;
        private TimeSpan _calculationTime;
        private TimeSpan _leaderboardLoadingTime;

        public MainForm()
        {
            InitializeComponent();
            _fileNameLists = new List<string>();
            _replayNameLists = new List<string>();
            chartList = new List<Chart>();

            if (!Directory.Exists(Program.MAIN_DIRECTORY))
                Directory.CreateDirectory(Program.MAIN_DIRECTORY);
            if (!Directory.Exists(Program.REPLAY_DIRECTORY))
                Directory.CreateDirectory(Program.REPLAY_DIRECTORY);
            if (!Directory.Exists(Program.EXPORT_DIRECTORY))
                Directory.CreateDirectory(Program.EXPORT_DIRECTORY);
            if (!Directory.Exists(Program.LEADERBOARD_DIRECTORY))
                Directory.CreateDirectory(Program.LEADERBOARD_DIRECTORY);
        }


        public void LoadAllCharts()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var filesList = Directory.GetFiles(Program.MAIN_DIRECTORY);
            var maxCount = filesList.Count();
            var currentCount = 0;
            ProgressBarLoading.Maximum = maxCount;
            ProgressBarLoading.Value = 0;
            Parallel.ForEach(filesList, new ParallelOptions() { MaxDegreeOfParallelism = 12 }, name =>
            {

                chartList.Add(ChartReader.LoadChart(name));
                if (!File.Exists(Program.EXPORT_DIRECTORY + name.Remove(0, Program.MAIN_DIRECTORY.Length).Split('.')[0] + ".json"))
                    ExportChartToJson(Program.EXPORT_DIRECTORY + name.Remove(0, Program.MAIN_DIRECTORY.Length).Split('.')[0] + ".json", chartList.Last());

                _fileNameLists.Add(name.Remove(0, Program.MAIN_DIRECTORY.Length));
                currentCount++;
                if (!ProgressBarLoading.InvokeRequired)
                {
                    UpdateProgressBar(currentCount, maxCount);
                }
            });
            stopwatch.Stop();
            _calculationTime = stopwatch.Elapsed;
        }

        public void UpdateProgressBar(int value, int maxValue)
        {
            ProgressBarLoading.Value = value;
            float percent = (value / (float)maxValue) * 100f;
            LoadingLabel.Text = $"LOADING {percent:0.00}%";
        }

        public void FillComboBoxReplay()
        {
            foreach (string replayNames in Directory.GetFiles(Program.REPLAY_DIRECTORY))
            {
                ComboBoxReplay.Items.Add(replayNames.Remove(0, Program.REPLAY_DIRECTORY.Length));
                _replayNameLists.Add(replayNames.Remove(0, Program.REPLAY_DIRECTORY.Length));
            }
        }

        public async void LoadAllChartsLeaderboards()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var maxCount = chartList.Count;
            var currentCount = 0;
            ProgressBarLoading.Maximum = maxCount;
            ProgressBarLoading.Value = 0;
            Parallel.ForEach(chartList, new ParallelOptions() { MaxDegreeOfParallelism = 12 }, chart =>
            {
                chart.GetLeaderboardFromAPI();
                currentCount++;
                if (!ProgressBarLoading.InvokeRequired)
                {
                    UpdateProgressBar(currentCount, maxCount);
                }
            });
            /*List<string> urls = new List<string>();
            chartList.ForEach(chart => urls.Add($"hashcheck/custom/?songHash={chart.songHash}"));
            var t = await Task.Run(() => TootTallyAPIServices.GetLeaderboardsId(urls.ToArray()));

            urls.Clear();
            t.ForEach(id => urls.Add($"songs/{id}/leaderboard"));
            var t2 = await Task.Run(() => TootTallyAPIServices.GetLeaderboards(urls.ToArray()));
            t2.ForEach(leaderboard => chartList.Find(chart => chart.songHash == leaderboard.songHash).leaderboard = leaderboard);*/

            stopwatch.Stop();
            _leaderboardLoadingTime = stopwatch.Elapsed;

        }

        public void ExportChartToJson(string path, Chart chart)
        {
            SerializableDiffData chartdata = new SerializableDiffData(chart.performances);
            var json = JsonConvert.SerializeObject(chartdata, Formatting.Indented);
            ChartReader.SaveChartData(path, json);
        }

        private void OnDisplayChartsButtonClick(object sender, EventArgs e)
        {
            TextBoxChartData.Clear();
            TextBoxLeaderboardData.Clear();
            var allChartDataTextLines = new List<string>();
            var allLeaderboardTextLines = new List<string>();

            chartList.Sort((x, y) => String.Compare(x.name, y.name));
            allChartDataTextLines.Add($"Calculation time took {_calculationTime.TotalSeconds}s for {chartList.Count} charts and {chartList.Count * 7} diffs");
            allLeaderboardTextLines.Add($"Calculation time took {_leaderboardLoadingTime.TotalSeconds}s for {chartList.Count} charts.");
            foreach (Chart chart in chartList)
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
                    DisplayAtSpeed(chart,1f, ref chartTextLines);
                chartTextLines.Add("=====================================================================================================");
                chartTextLines.Add("");
                allChartDataTextLines.AddRange(chartTextLines);

                //LEADERBOARD DISPLAY
                var leaderboardText = DisplayLeaderboard(chart);
                if (leaderboardText.Count == 0) continue; //Skip leaderboard display if no scores found
                var leaderboardTextLines = new List<string>
                {
                    $"{chart.name} processed in {chart.calculationTime.TotalSeconds}s",
                    GetLeaderboardScoreHeader(),
                    "-----------------------------------------------------------------------------------------------------"
                };

                leaderboardTextLines.AddRange(leaderboardText);
                leaderboardTextLines.Add("=====================================================================================================");
                leaderboardTextLines.Add("");
                allLeaderboardTextLines.AddRange(leaderboardTextLines);

            }
            TextBoxChartData.Lines = allChartDataTextLines.ToArray();
            TextBoxLeaderboardData.Lines = allLeaderboardTextLines.ToArray();
        }

        public void DisplayAllSpeed(Chart chart, ref List<string> textLines)
        {
            for (int i = 0; i < chart.GAME_SPEED.Length; i++)
                DisplayAtSpeed(chart, chart.GAME_SPEED[i], ref textLines);
        }

        public void DisplayAtSpeed(Chart chart, float gamespeed, ref List<string> textLines)
        {
            DataVectorAnalytics aimAnalytics = chart.performances.aimAnalyticsDict[gamespeed];
            DataVectorAnalytics aimEndAnalytics = chart.performances.aimEndAnalyticsDict[gamespeed];
            DataVectorAnalytics tapAnalytics = chart.performances.tapAnalyticsDict[gamespeed];
            DataVectorAnalytics tapEndAnalytics = chart.performances.tapEndAnalyticsDict[gamespeed];
            DataVectorAnalytics accAnalytics = chart.performances.accAnalyticsDict[gamespeed];
            textLines.Add($"SPEED: {gamespeed:0.00}x rated {chart.GetStarRating(gamespeed)}");
            textLines.Add($"  aim: " + aimAnalytics.perfAverage + " min: " + aimAnalytics.perfMin + " max: " + aimAnalytics.perfMax);
            textLines.Add($"  aend: " + aimEndAnalytics.perfAverage + " min: " + aimEndAnalytics.perfMin + " max: " + aimEndAnalytics.perfMax);
            textLines.Add($"  tap: " + tapAnalytics.perfAverage + " min: " + tapAnalytics.perfMin + " max: " + tapAnalytics.perfMax);
            textLines.Add($"  tend: " + tapEndAnalytics.perfAverage + " min: " + tapEndAnalytics.perfMin + " max: " + tapEndAnalytics.perfMax);
            textLines.Add($"  acc: " + accAnalytics.perfAverage + " min: " + accAnalytics.perfMin + " max: " + accAnalytics.perfMax);
            textLines.Add("--------------------------------------------");
        }

        public List<string> DisplayLeaderboard(Chart chart)
        {
            var count = 0;
            List<string> textLines = new List<string>();
            chart.leaderboard?.results.ForEach(score =>
                {
                    score.tt = (float)CalculateScoreTT(chart, score);
                    if (score.tt >= (float)FilterMinTT.Value && score.tt <= (float)FilterMaxTT.Value && score.player.Contains(FilterPlayerName.Text))
                        textLines.Add(GetDisplayScoreLine2(score, chart, count));
                    count++;
                });
            return textLines;
        }

        public string GetDisplayScoreLine(Leaderboard.ScoreDataFromDB score, Chart chart, int count) =>
            $"#{count} {score.player}\t\t{score.score}\t\t({score.replay_speed:0.00}x)\t {score.percentage:0.00}%\t{score.grade}\t{score.tt:0.00}tt\tdiff:{chart.GetDiffRating(score.replay_speed):0.00}";

        public string GetDisplayScoreLine2(Leaderboard.ScoreDataFromDB score, Chart chart, int count) =>
            FormatLeaderboardScore(count.ToString(), score.player, score.score.ToString(), score.replay_speed.ToString("0.00"), score.percentage.ToString("0.00"), score.grade, score.tt.ToString("0.00"), chart.GetDiffRating(score.replay_speed).ToString("0.00"));

        public string FormatLeaderboardScore(string count, string player, string score, string replaySpeed, string percentage, string grade, string tt, string diff)
        {
            return String.Format("{0,-4} | {1,-30} | {2, -11} | {3, -8} | {4, -6} | {5, -5} | {6, -10} | {7, 5}",
                $"#{count}",
                $"{player}",
                $"{score}",
                $"({replaySpeed}x)",
                $"{percentage}%",
                $"{grade}",
                $"{tt}tt",
                $"{diff}"
                );
        }

        public string GetLeaderboardScoreHeader()
        {
            return String.Format("{0,-4} | {1,-30} | {2, -11} | {3, -8} | {4, -6} | {5, -5} | {6, -10} | {7, 5}",
                $"Rank",
                $"Name",
                $"Score",
                $"Speed",
                $"Perc",
                $"Grade",
                $"TT",
                $"Diff"
                );
        }

        //TT for S rank (60% score)
        //https://www.desmos.com/calculator/rhwqyp21nr
        public static double CalculateBaseTT(float starRating)
        {

            return 1.05f * FastPow(starRating, 2) + (3f * starRating) + 0.01f;
            //y = 1.05x^2 + 3x + 0.01
        }

        //https://www.desmos.com/calculator/bnyo9f5u1y
        public static double CalculateScoreTT(Chart chart, Leaderboard.ScoreDataFromDB score)
        {
            var baseTT = CalculateBaseTT(chart.GetDiffRating(score.replay_speed));

            var percentage = score.percentage / 100d;

            var scoreTT = ((0.028091281 * Math.Pow(Math.E, 6d * percentage)) - 0.028091281) * baseTT;

            return scoreTT;
        }

        private void OnLoadReplayButtonClick(object sender, EventArgs e)
        {
            _currentReplay = ChartReader.LoadReplay(Program.REPLAY_DIRECTORY + ComboBoxReplay.Text);
            _currentReplay.SetChart(_currentChart);
            TextBoxChartData.Clear();
            List<string> textLines = new List<string>
            {
                "Replay Loaded: " + _currentReplay.username
            };
            _currentReplay.ToDisplayData().ForEach(x => textLines.Add(x));
            TextBoxChartData.Lines = textLines.ToArray();
            TextBoxChartData.Visible = true;
        }

        private void OnDropDownSongNameValueChange(object sender, EventArgs e)
        {
            ButtonForceRefresh.Visible = true;
        }

        private void OnDropDownReplayValueChange(object sender, EventArgs e)
        {
            ButtonLoadReplay.Visible = true;
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
            LoadAllCharts();
            //LoadAllChartsLeaderboards();
            FillComboBoxReplay();
            OnDisplayChartsButtonClick(sender, e);

            ProgressBarLoading.Visible = false;
            LoadingLabel.Visible = false;
        }

        private void OnValueBoxTextChanged(object sender, EventArgs e)
        {
            OnDisplayChartsButtonClick(sender, e);
        }

        private void OnSaveToButtonPress(object sender, EventArgs e)
        {
            string date = DateTime.Now.ToString("yyyyMMddHHmmss");
            string fileName = Program.LEADERBOARD_DIRECTORY + "ChartRatings" + date + ".txt";
            string text = "";
            TextBoxChartData.Lines.ToList().ForEach(line => { text += line + "\n"; });
            ChartReader.SaveChartData(fileName, text);
            fileName = Program.LEADERBOARD_DIRECTORY + "Leaderboards" + date + ".txt";
            text = "";
            TextBoxLeaderboardData.Lines.ToList().ForEach(line => { text += line + "\n"; });
            ChartReader.SaveChartData(fileName, text);
        }
    }
}