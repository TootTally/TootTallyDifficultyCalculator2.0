using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TootTallyDifficultyCalculator2._0
{
    public static class TootTallyAPIServices
    {
        public static HttpClient webRequest;
        public const string APIURL = "https://toottally.com/api/";
        public const string REPLAYURL = "http://cdn.toottally.com/replays/";

        static TootTallyAPIServices()
        {
            webRequest = new HttpClient
            {
                BaseAddress = new Uri(APIURL)
            };
            webRequest.DefaultRequestHeaders.Accept.Clear();
            webRequest.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static void GetChartData(string songHash, Action<Leaderboard> callback)
        {
            int chartID;
            try
            {
                chartID = int.Parse(GetStringRequest($"hashcheck/custom/?songHash={songHash}").Result);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Couldn't find songID for " + songHash);
                return;
            }
            Leaderboard leaderboard = null;

            if (chartID != 0)
            {
                try
                {
                    leaderboard = JsonConvert.DeserializeObject<Leaderboard>(GetStringRequest($"songs/{chartID}/leaderboard").Result);
                } catch(Exception ex)
                {
                    Trace.WriteLine("Couldn't find leaderboard for " + chartID);
                }
            }

            if (leaderboard != null)
                callback(leaderboard);
        }


        private static Task<HttpResponseMessage> PostUploadRequest(string query, dynamic data)
        {

            var response = webRequest.PostAsync(query, data);
            return response;
        }

        private static Task<string> GetStringRequest(string query)
        {
            //Trace.WriteLine(APIURL + query);
            var response = webRequest.GetStringAsync(query);
            return response;
        }

        private class APIChartRequest
        {
            public int count;
            public string next;
            public string previous;
            public List<Chart> results;
        }
    }
}
