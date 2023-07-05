using System.Security.Cryptography.X509Certificates;

namespace TootTallyDifficultyCalculator2._0
{
    public partial class MainForm : Form
    {
        private List<string> _fileNameLists;
        private List<string> _replayNameLists;
        private Chart _currentChart;
        private ReplayData _currentReplay;
        public List<Chart> chartList;

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

            FillComboBoxSongName();
            FillComboBoxReplay();
        }

        public void FillComboBoxSongName()
        {
            foreach (string name in Directory.GetFiles(Program.MAIN_DIRECTORY))
            {
                chartList.Add(ChartReader.LoadChart(name));
                ComboBoxSongName.Items.Add(name.Remove(0, Program.MAIN_DIRECTORY.Length));
                _fileNameLists.Add(name.Remove(0, Program.MAIN_DIRECTORY.Length));
            }
        }

        public void FillComboBoxReplay()
        {
            foreach (string replayNames in Directory.GetFiles(Program.REPLAY_DIRECTORY))
            {
                ComboBoxReplay.Items.Add(replayNames.Remove(0, Program.REPLAY_DIRECTORY.Length));
                _replayNameLists.Add(replayNames.Remove(0, Program.REPLAY_DIRECTORY.Length));
            }
        }

        private void OnLoadChartButtonClick(object sender, EventArgs e)
        {
            _currentChart = ChartReader.LoadChart(Program.MAIN_DIRECTORY + ComboBoxSongName.Text);
            ListboxMapData.Items.Clear();
            ListboxMapData.Items.Add("HasSamePosition: " + _currentChart.CheckIfSamePosition());
            _currentChart.ToDisplayData().ForEach(x => ListboxMapData.Items.Add(x));
            ListboxMapData.Visible = true;
            var aimData = new Graph("Aim Data", _currentChart.aimPerformanceList);
            aimData.Show();
            var tapData = new Graph("Tap Data", _currentChart.tapPerformanceList);
            tapData.Show();
            var accData = new Graph("Acc Data", _currentChart.accPerformanceList);
            accData.Show();
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