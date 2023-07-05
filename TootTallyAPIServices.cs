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

        public static void GetChartData(string songHash, Action<Chart> callback)
        {
            int chartID = int.Parse(GetStringRequest($"hashcheck/custom/?songHash={songHash}").Result);
            Chart chart = null;

            if (chartID != 0)
            {
                var charts = JsonConvert.DeserializeObject<APIChartRequest>(GetStringRequest($"songs/{chartID}").Result);
                chart = charts.results.First();
            }

            if (chart != null)
                callback(chart);
        }


        private static Task<HttpResponseMessage> PostUploadRequest(string query, dynamic data)
        {

            var response = webRequest.PostAsync(query, data);
            return response;
        }

        private static Task<string> GetStringRequest(string query)
        {
            Trace.WriteLine(APIURL + query);
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
