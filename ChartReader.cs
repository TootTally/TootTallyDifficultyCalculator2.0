using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TootTallyDifficultyCalculator2._0
{
    public static class ChartReader
    {
        public static Chart LoadChart(string path)
        {
            StreamReader reader = new StreamReader(path);
            string json = reader.ReadToEnd();
            Chart chart = JsonConvert.DeserializeObject<Chart>(json);
            chart.OnDeserialize();
            reader.Close();
            return chart;

        }
    }
}
