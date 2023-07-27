using RGiesecke.DllExport;
using System.Runtime.InteropServices;

namespace TootTallyDifficultyCalculator2._0
{
    public class PythonExport
    {
        [DllExport("LoadChart", CallingConvention = CallingConvention.StdCall)]
        public static Chart LoadChart(string path)
        {
            return ChartReader.LoadChart(path);
        }

        [DllExport("GetChartRatings", CallingConvention = CallingConvention.StdCall)]
        public static double[][] GetChartRatings(string path)
        {
            
            var chart = ChartReader.LoadChart(path);
            double[][] ratings = new double[7][];
            for (int i = 0; i < 7; i++)
                ratings[i] = new double[]
                {
                    chart.GetStarRating(i),
                    chart.GetAimPerformance(i),
                    chart.GetTapPerformance(i),
                    chart.GetAccPerformance(i),
                };

            return ratings;
        }



    }
}
