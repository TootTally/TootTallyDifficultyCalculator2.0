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
            var accSeries = new ChartSeries("Acc Rating", ChartSeriesType.Line);
            var accEndSeries = new ChartSeries("Acc Endurance", ChartSeriesType.Line);
            accSeries.Style.Border = aimSeries.Style.Border;
            var starSeries = new ChartSeries("Star Rating", ChartSeriesType.Line);
            var weightSeries = new ChartSeries("Weight", ChartSeriesType.Line);
            starSeries.Style.Border = aimSeries.Style.Border;
            weightSeries.Style.Border = aimEndSeries.Style.Border;

            var weightSum = _chart.performances.aimPerfMatrix[2].Sum(x => x.weight);

            for (int i = 0; i < _chart.performances.aimPerfMatrix[2].Count; i++)
            {
                var time = _chart.performances.aimPerfMatrix[2][i].time;
                var weight = _chart.performances.aimPerfMatrix[2][i].weight / weightSum;
                aimSeries.Points.Add(time, _chart.performances.aimPerfMatrix[2][i].performance * weight);
                aimEndSeries.Points.Add(time, _chart.performances.aimPerfMatrix[2][i].endurance);
                tapSeries.Points.Add(time, _chart.performances.tapPerfMatrix[2][i].performance * weight);
                tapEndSeries.Points.Add(time, _chart.performances.tapPerfMatrix[2][i].endurance);
                accSeries.Points.Add(time, _chart.performances.accPerfMatrix[2][i].performance * weight);
                accEndSeries.Points.Add(time, _chart.performances.accPerfMatrix[2][i].endurance);
                starSeries.Points.Add(time, (_chart.performances.aimPerfMatrix[2][i].performance + _chart.performances.tapPerfMatrix[2][i].performance + _chart.performances.accPerfMatrix[2][i].performance) * weight / 3f);
                weightSeries.Points.Add(time, weight * 100f);
            }

            AimRatingGraph.Series.Add(aimSeries);
            AimRatingGraph.Series.Add(aimEndSeries);
            TapRatingGraph.Series.Add(tapSeries);
            TapRatingGraph.Series.Add(tapEndSeries);
            AccRatingGraph.Series.Add(accSeries);
            AccRatingGraph.Series.Add(accEndSeries);
            StarRatingGraph.Series.Add(starSeries);
            StarRatingGraph.Series.Add(weightSeries);
        }

        private void OnFormClose(object sender, FormClosedEventArgs e)
        {
            Dispose();
        }
    }
}
