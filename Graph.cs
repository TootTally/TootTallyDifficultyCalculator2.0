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
    public partial class Graph : Form
    {
        public Graph(string name, List<ChartPerformances.DataVector> points)
        {
            InitializeComponent();
            ChartData.Series.Clear();
            ChartData.PrimaryXAxis.RangeType = ChartAxisRangeType.Set;
            ChartData.PrimaryXAxis.ValueType = ChartValueType.Custom;
            ChartData.PrimaryXAxis.Range = new MinMaxInfo(0, points.Last().time, 0);
            ChartData.ChartInterior = new Syncfusion.Drawing.BrushInfo(Color.Silver);
            ChartSeries series = new ChartSeries(name, ChartSeriesType.Area);
            for (int i = 0; i < points.Count; i++)
                series.Points.Add(points[i].time, points[i].performance);

            ChartData.Series.Add(series);
            series.Style.Border.Width = 2;
            series.Style.Border.Color = Color.Red;
        }
    }
}
