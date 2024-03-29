﻿using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace TootTallyDifficultyCalculator2._0
{
    public static class ChartReader
    {

        public static ReplayData LoadReplay(string path)
        {
            ReplayData replay = JsonConvert.DeserializeObject<ReplayData>(File.ReadAllText(path));
            return replay;
        }

        public static ReplayData LoadReplayFromJson(string json)
        {
            ReplayData replay = JsonConvert.DeserializeObject<ReplayData>(json);
            replay.OnDeserialize();
            return replay;
        }

        public static TMBChart LoadChart(string path)
        {
            TMBChart chart = JsonConvert.DeserializeObject<TMBChart>(File.ReadAllText(path));
            return chart;
        }

        public static TMBChart LoadChartFromJson(string json)
        {
            TMBChart chart = JsonConvert.DeserializeObject<TMBChart>(json);
            chart.OnDeserialize();
            return chart;
        }

        public static List<Leaderboard.SongInfoFromDB> GetCachedFileHashes()
        {
            if (!File.Exists(Program.CACHE_DIRECTORY + "file_hash.txt"))
                File.Create(Program.CACHE_DIRECTORY + "file_hash.txt").Close();

            StreamReader reader = new StreamReader(Program.CACHE_DIRECTORY + "file_hash.txt");
            string json = reader.ReadToEnd();
            reader.Close();
            try
            {
                return JsonConvert.DeserializeObject<List<Leaderboard.SongInfoFromDB>>(json);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static void SaveCacheFileHashes(List<Leaderboard.SongInfoFromDB> fileHashes)
        {
            if (File.Exists(Program.CACHE_DIRECTORY + "file_hash.txt"))
                File.Delete(Program.CACHE_DIRECTORY + "file_hash.txt");

            StreamWriter writer = new StreamWriter(Program.CACHE_DIRECTORY + "file_hash.txt");
            writer.WriteLine(JsonConvert.SerializeObject(fileHashes));
            writer.Close();

        }

        public static string CalcSHA256Hash(byte[] data)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                string ret = "";
                byte[] hashArray = sha256.ComputeHash(data);
                foreach (byte b in hashArray)
                {
                    ret += $"{b:x2}";
                }
                return ret;
            }
        }

        public static void SaveChartData(string path, string json)
        {
            StreamWriter writer = new StreamWriter(path);
            writer.WriteLine(json);
            writer.Close();
        }

    }
}
