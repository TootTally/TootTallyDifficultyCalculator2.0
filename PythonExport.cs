using RGiesecke.DllExport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TootTallyDifficultyCalculator2._0
{
    public class PythonExport
    {
        [DllExport("LoadChart", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static Chart LoadChart(string path)
        {
            return ChartReader.LoadChart(path);
        }

        [DllExport("GetChartRatings", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static double[][] GetRatings(string path)
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
