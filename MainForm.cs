using System.Security.Cryptography.X509Certificates;

namespace TootTallyDifficultyCalculator2._0
{
    public partial class MainForm : Form
    {
        private List<string> _fileNameLists;
        private Chart _currentChart;

        public MainForm()
        {
            InitializeComponent();
            _fileNameLists = new List<string>();
            if (!Directory.Exists(Program.MAIN_DIRECTORY))
                Directory.CreateDirectory(Program.MAIN_DIRECTORY);

            FillComboBoxSongName();
        }

        public void FillComboBoxSongName()
        {
            foreach (string fileNames in Directory.GetFiles(Program.MAIN_DIRECTORY))
            {
                ComboBoxSongName.Items.Add(fileNames.Remove(0, Program.MAIN_DIRECTORY.Length));
                _fileNameLists.Add(fileNames.Remove(0, Program.MAIN_DIRECTORY.Length));
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

        private void OnDropDownSongNameValueChange(object sender, EventArgs e)
        {
            ButtonLoadChart.Visible = true;
        }
    }
}