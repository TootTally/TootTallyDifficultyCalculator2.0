using Newtonsoft.Json;
using System.CodeDom;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

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

            LoadAllCharts();
            LoadAllChartsLeaderboards();
            FillComboBoxReplay();
        }


        public void LoadAllCharts()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var filesList = Directory.GetFiles(Program.MAIN_DIRECTORY);
            Parallel.ForEach(filesList, name =>
            {

                chartList.Add(ChartReader.LoadChart(name));
                if (!File.Exists(Program.EXPORT_DIRECTORY + name.Remove(0, Program.MAIN_DIRECTORY.Length).Split('.')[0] + ".json"))
                    ExportChartToJson(Program.EXPORT_DIRECTORY + name.Remove(0, Program.MAIN_DIRECTORY.Length).Split('.')[0] + ".json", chartList.Last());

                _fileNameLists.Add(name.Remove(0, Program.MAIN_DIRECTORY.Length));
            });
            stopwatch.Stop();
            _calculationTime = stopwatch.Elapsed;
        }

        public void FillComboBoxReplay()
        {
            foreach (string replayNames in Directory.GetFiles(Program.REPLAY_DIRECTORY))
            {
                ComboBoxReplay.Items.Add(replayNames.Remove(0, Program.REPLAY_DIRECTORY.Length));
                _replayNameLists.Add(replayNames.Remove(0, Program.REPLAY_DIRECTORY.Length));
            }
        }

        public void LoadAllChartsLeaderboards()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Parallel.ForEach(chartList, new ParallelOptions() { MaxDegreeOfParallelism = 8 }, chart => chart.GetLeaderboardFromAPI());
            stopwatch.Stop();
            _leaderboardLoadingTime = stopwatch.Elapsed;

        }

        public void ExportChartToJson(string path, Chart chart)
        {
            Chart.SerializableDiffData chartdata = new Chart.SerializableDiffData()
            {
                speed050 = new Chart.SerializableDataVector() { aim = chart.aimPerformanceDict[0.5f], tap = chart.aimPerformanceDict[0.5f], acc = chart.aimPerformanceDict[0.5f] },
                speed075 = new Chart.SerializableDataVector() { aim = chart.aimPerformanceDict[0.75f], tap = chart.aimPerformanceDict[0.75f], acc = chart.aimPerformanceDict[0.75f] },
                speed100 = new Chart.SerializableDataVector() { aim = chart.aimPerformanceDict[1f], tap = chart.aimPerformanceDict[1f], acc = chart.aimPerformanceDict[1f] },
                speed125 = new Chart.SerializableDataVector() { aim = chart.aimPerformanceDict[1.25f], tap = chart.aimPerformanceDict[1.25f], acc = chart.aimPerformanceDict[1.25f] },
                speed150 = new Chart.SerializableDataVector() { aim = chart.aimPerformanceDict[1.5f], tap = chart.aimPerformanceDict[1.5f], acc = chart.aimPerformanceDict[1.5f] },
                speed175 = new Chart.SerializableDataVector() { aim = chart.aimPerformanceDict[1.75f], tap = chart.aimPerformanceDict[1.75f], acc = chart.aimPerformanceDict[1.75f] },
                speed200 = new Chart.SerializableDataVector() { aim = chart.aimPerformanceDict[2f], tap = chart.aimPerformanceDict[2f], acc = chart.aimPerformanceDict[2f] },

            };
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
                var chartTextLines = new List<string>();
                var leaderboardTextLines = new List<string>();

                if (!chart.name.ToLower().Contains(textBox1.Text.ToLower())) continue;
                chartTextLines.Add($"{chart.name} processed in {chart.calculationTime.TotalSeconds}s");
                leaderboardTextLines.Add($"{chart.name} processed in {chart.calculationTime.TotalSeconds}s");
                if (checkboxAllSpeed.Checked)
                    DisplayAllSpeed(chart, ref chartTextLines);
                else
                    DisplayNormalSpeed(chart, ref chartTextLines);
                leaderboardTextLines.AddRange(DisplayLeaderboard(chart));
                leaderboardTextLines.Add("=============================================");
                chartTextLines.Add("=============================================");
                allChartDataTextLines.AddRange(chartTextLines);
                allLeaderboardTextLines.AddRange(leaderboardTextLines);

            }
            TextBoxChartData.Lines = allChartDataTextLines.ToArray();
            TextBoxLeaderboardData.Lines = allLeaderboardTextLines.ToArray();
        }

        public void DisplayAllSpeed(Chart chart, ref List<string> textLines)
        {
            for (int i = 0; i < chart.GAME_SPEED.Length; i++)
            {
                var gamespeed = chart.GAME_SPEED[i];
                Chart.DataVectorAnalytics aimAnalytics = chart.aimAnalyticsDict[gamespeed];
                Chart.DataVectorAnalytics tapAnalytics = chart.tapAnalyticsDict[gamespeed];
                Chart.DataVectorAnalytics accAnalytics = chart.accAnalyticsDict[gamespeed];
                textLines.Add($"SPEED: {gamespeed:0.00}x rated {chart.starRatingDict[gamespeed]}");
                textLines.Add($"  aim: " + aimAnalytics.perfAverage + " min: " + aimAnalytics.perfMin + " max: " + aimAnalytics.perfMax);
                textLines.Add($"  tap: " + tapAnalytics.perfAverage + " min: " + tapAnalytics.perfMin + " max: " + tapAnalytics.perfMax);
                textLines.Add($"  acc: " + accAnalytics.perfAverage + " min: " + accAnalytics.perfMin + " max: " + accAnalytics.perfMax);
                textLines.Add("--------------------------------------------");
            }
        }

        public void DisplayNormalSpeed(Chart chart, ref List<string> textLines)
        {
            var gamespeed = 1f;
            Chart.DataVectorAnalytics aimAnalytics = chart.aimAnalyticsDict[gamespeed];
            Chart.DataVectorAnalytics tapAnalytics = chart.tapAnalyticsDict[gamespeed];
            Chart.DataVectorAnalytics accAnalytics = chart.accAnalyticsDict[gamespeed];
            textLines.Add($"SPEED: {gamespeed:0.00}x rated {chart.starRatingDict[gamespeed]}");
            textLines.Add($"  aim: " + aimAnalytics.perfAverage + " min: " + aimAnalytics.perfMin + " max: " + aimAnalytics.perfMax);
            textLines.Add($"  tap: " + tapAnalytics.perfAverage + " min: " + tapAnalytics.perfMin + " max: " + tapAnalytics.perfMax);
            textLines.Add($"  acc: " + accAnalytics.perfAverage + " min: " + accAnalytics.perfMin + " max: " + accAnalytics.perfMax);
            textLines.Add("--------------------------------------------");
        }

        public List<string> DisplayLeaderboard(Chart chart)
        {
            var count = 0;
            List<string> textLines = new List<string>();
            chart.leaderboard?.results.ForEach(score =>
                {
                    textLines.Add(GetDisplayScoreLine(score, chart, count));
                    count++;
                });
            return textLines;
        }

        public string GetDisplayScoreLine(Leaderboard.ScoreDataFromDB score, Chart chart, int count) =>
            $"#{count} {score.player} {score.score} ({score.replay_speed:0.00}x) {score.percentage:0.00}% {score.grade} {CalculateScoreTT(chart, score):0.00}tt diff:{chart.GetDiffRating(score.replay_speed):0.00}";


        public double CalculateBaseTT(float starRating)
        {

            return 3f * Chart.FastPow(starRating, 2) + (starRating * 15f);
        }

        public double CalculateScoreTT(Chart chart, Leaderboard.ScoreDataFromDB score)
        {
            var baseTT = CalculateBaseTT(chart.GetDiffRating(score.replay_speed));

            var percentage = score.percentage / 100f;

            var scoreTT = ((0.0080042 * Math.Pow(Math.E, 6.90823  * percentage)) - 0.0080042) * baseTT;

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
            ButtonLoadChart.Visible = true;
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
    }
}