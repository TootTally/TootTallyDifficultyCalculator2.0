using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TootTallyDifficultyCalculator2._0
{
    public static class ChartReader
    {
        private static List<Chart> _allChartList = new List<Chart>();

        public static void AddChartToList(string path) =>
            _allChartList.Add(LoadChart(path));

        public static Chart LoadChart(string path)
        {
            StreamReader reader = new StreamReader(path);
            string json = reader.ReadToEnd();
            Chart chart = JsonConvert.DeserializeObject<Chart>(json);
            chart.OnDeserialize();
            reader.Close();
            return chart;
        }

        public static void SaveChartData(string path, string json)
        {
            StreamWriter writer = new StreamWriter(path);
            writer.WriteLine(json);
            writer.Close();
        }

        public static ReplayData LoadReplay(string path)
        {
            string json = GetJsonStringFromZipFilePath(path);
            ReplayData replayData = JsonConvert.DeserializeObject<ReplayData>(json);
            replayData.OnDeserialize();
            return replayData;
        }

        private static string GetJsonStringFromZipFilePath(string path)
        {
            string jsonString;
            using (var memoryStream = new MemoryStream())
            {
                using (var fileStream = new FileStream(path, FileMode.Open))
                {
                    fileStream.CopyTo(memoryStream);
                }

                using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Read, true))
                {
                    var zipFile = zipArchive.GetEntry(zipArchive.Entries[0].Name);

                    using (var entry = zipFile.Open())
                    using (var sr = new StreamReader(entry))
                    {
                        jsonString = sr.ReadToEnd();
                    }

                }
            }
            return jsonString;
        }
    }
}
