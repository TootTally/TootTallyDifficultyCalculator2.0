using Newtonsoft.Json;
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

            FillComboBoxSongName();
            FillComboBoxReplay();
        }

        public void FillComboBoxSongName()
        {
            ComboBoxSongName.Items.Add("ALL");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            foreach (string name in Directory.GetFiles(Program.MAIN_DIRECTORY))
            {

                chartList.Add(ChartReader.LoadChart(name));
                if (!File.Exists(Program.EXPORT_DIRECTORY + name.Remove(0, Program.MAIN_DIRECTORY.Length).Split('.')[0] + ".json"))
                    ExportChartToJson(Program.EXPORT_DIRECTORY + name.Remove(0, Program.MAIN_DIRECTORY.Length).Split('.')[0] + ".json", chartList.Last());

                ComboBoxSongName.Items.Add(name.Remove(0, Program.MAIN_DIRECTORY.Length));
                _fileNameLists.Add(name.Remove(0, Program.MAIN_DIRECTORY.Length));
            }
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

        public void ExportChartToJson(string path, Chart chart)
        {
            Chart.SerializableDiffData chartdata = new Chart.SerializableDiffData()
            {
                speed050 = new Chart.SerializableDataVector() { aim = chart.aimPerformanceDict[0.5f],   tap = chart.aimPerformanceDict[0.5f],   acc = chart.aimPerformanceDict[0.5f] },
                speed075 = new Chart.SerializableDataVector() { aim = chart.aimPerformanceDict[0.75f],  tap = chart.aimPerformanceDict[0.75f],  acc = chart.aimPerformanceDict[0.75f] },
                speed100 = new Chart.SerializableDataVector() { aim = chart.aimPerformanceDict[1f],     tap = chart.aimPerformanceDict[1f],     acc = chart.aimPerformanceDict[1f] },
                speed125 = new Chart.SerializableDataVector() { aim = chart.aimPerformanceDict[1.25f],  tap = chart.aimPerformanceDict[1.25f],  acc = chart.aimPerformanceDict[1.25f] },
                speed150 = new Chart.SerializableDataVector() { aim = chart.aimPerformanceDict[1.5f],   tap = chart.aimPerformanceDict[1.5f],   acc = chart.aimPerformanceDict[1.5f] },
                speed175 = new Chart.SerializableDataVector() { aim = chart.aimPerformanceDict[1.75f],  tap = chart.aimPerformanceDict[1.75f],  acc = chart.aimPerformanceDict[1.75f] },
                speed200 = new Chart.SerializableDataVector() { aim = chart.aimPerformanceDict[2f],     tap = chart.aimPerformanceDict[2f],     acc = chart.aimPerformanceDict[2f] },

            };
            var json = JsonConvert.SerializeObject(chartdata, Formatting.Indented);
            ChartReader.SaveChartData(path, json);
        }


        private void OnLoadChartButtonClick(object sender, EventArgs e)
        {
            ListboxMapData.Items.Clear();

            if (ComboBoxSongName.Text == "ALL")
            {
                chartList.Sort((x, y) => String.Compare(x.name,y.name)); 
                ListboxMapData.Items.Add($"Calculation time took {_calculationTime.TotalSeconds}s for {chartList.Count} charts and {chartList.Count * 7} diffs");
                foreach (Chart chart in chartList)
                {
                    ListboxMapData.Items.Add($"{chart.name} processed in {chart.calculationTime.TotalSeconds}s");
                    for (int i = 0; i < chart.GAME_SPEED.Length; i++)
                    {
                        var gamespeed = chart.GAME_SPEED[i];
                        Chart.DataVectorAnalytics aimAnalytics = chart.aimAnalyticsDict[gamespeed];
                        Chart.DataVectorAnalytics tapAnalytics = chart.tapAnalyticsDict[gamespeed];
                        Chart.DataVectorAnalytics accAnalytics = chart.accAnalyticsDict[gamespeed];
                        ListboxMapData.Items.Add($"SPEED: {gamespeed:0.00}x rated {chart.starRatingDict[gamespeed]}");
                        ListboxMapData.Items.Add($"  aim: " + aimAnalytics.perfAverage + " min: " + aimAnalytics.perfMin + " max: " + aimAnalytics.perfMax);
                        ListboxMapData.Items.Add($"  tap: " + tapAnalytics.perfAverage + " min: " + tapAnalytics.perfMin + " max: " + tapAnalytics.perfMax);
                        ListboxMapData.Items.Add($"  acc: " + accAnalytics.perfAverage + " min: " + accAnalytics.perfMin + " max: " + accAnalytics.perfMax);
                        ListboxMapData.Items.Add("--------------------------------------------");
                    }
                    ListboxMapData.Items.Add("=============================================");
                }
            }
            else
            {
                _currentChart = ChartReader.LoadChart(Program.MAIN_DIRECTORY + ComboBoxSongName.Text);
                _currentChart.ToDisplayData().ForEach(x => ListboxMapData.Items.Add(x));
                /*
                var aimData = new Graph("Aim Data", _currentChart.aimPerformanceDict);
                aimData.Show();
                var tapData = new Graph("Tap Data", _currentChart.tapPerformanceDict);
                tapData.Show();
                var accData = new Graph("Acc Data", _currentChart.accPerformanceDict);
                accData.Show();
                */
            }
            ListboxMapData.Visible = true;
        }

        private void OnLoadReplayButtonClick(object sender, EventArgs e)
        {
            _currentReplay = ChartReader.LoadReplay(Program.REPLAY_DIRECTORY + ComboBoxReplay.Text);
            TootTallyAPIServices.GetChartData(_currentReplay.songhash, (chart) =>
            {
                _currentChart = chartList.Find(c => c.name == chart.name);
                _currentReplay.SetChart(_currentChart);
                ListboxMapData.Items.Clear();
                ListboxMapData.Items.Add("Replay Loaded: " + _currentReplay.username);
                _currentReplay.ToDisplayData().ForEach(x => ListboxMapData.Items.Add(x));
                ListboxMapData.Visible = true;
            });

        }

        private void OnDropDownSongNameValueChange(object sender, EventArgs e)
        {
            ButtonLoadChart.Visible = true;
        }

        private void OnDropDownReplayValueChange(object sender, EventArgs e)
        {
            ButtonLoadReplay.Visible = true;
        }


    }
}