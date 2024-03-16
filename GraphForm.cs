using Syncfusion.Windows.Forms.Chart;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TootTallyDifficultyCalculator2._0
{
    public partial class GraphForm : Form
    {
        private TMBChart _chart;
        public GraphForm(TMBChart chart)
        {
            _chart = chart;
            InitializeComponent();
            AimRatingGraph.Series.Clear();
            var aimSeries = new ChartSeries("Aim Rating", ChartSeriesType.Line);
            aimSeries.Style.Border = new ChartLineInfo()
            {
                Width = 2,
                Color = Color.Red
            };
            var aimEndSeries = new ChartSeries("Aim Endurance", ChartSeriesType.Line);
            aimEndSeries.Style.Border = new ChartLineInfo()
            {
                Width = 2,
                Color = Color.Blue
            };

            var tapSeries = new ChartSeries("Tap Rating", ChartSeriesType.Line);
            var tapEndSeries = new ChartSeries("Tap Endurance", ChartSeriesType.Line);
            tapSeries.Style.Border = aimSeries.Style.Border;
            var starSeries = new ChartSeries("Star Rating", ChartSeriesType.Line);
            starSeries.Style.Border = aimSeries.Style.Border;

            for (int i = 0; i < _chart.performances.aimPerfMatrix[2].Count; i++)
            {
                var time = _chart.performances.aimPerfMatrix[2][i].time;
                aimSeries.Points.Add(time, _chart.performances.aimPerfMatrix[2][i].performance);
                aimEndSeries.Points.Add(time, _chart.performances.aimPerfMatrix[2][i].endurance);
                tapSeries.Points.Add(time, _chart.performances.tapPerfMatrix[2][i].performance);
                tapEndSeries.Points.Add(time, _chart.performances.tapPerfMatrix[2][i].endurance);
                starSeries.Points.Add(time, (_chart.performances.aimPerfMatrix[2][i].performance + _chart.performances.tapPerfMatrix[2][i].performance) / 2f);
            }

            AimRatingGraph.Series.Add(aimSeries);
            AimRatingGraph.Series.Add(aimEndSeries);
            TapRatingGraph.Series.Add(tapSeries);
            TapRatingGraph.Series.Add(tapEndSeries);
            StarRatingGraph.Series.Add(starSeries);
        }

        private void OnFormClose(object sender, FormClosedEventArgs e)
        {
            Dispose();
        }
    }
}
