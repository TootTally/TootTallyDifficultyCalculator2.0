using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            int chartID = int.Parse(GetRequest($"hashcheck/custom/?songHash={songHash}").Result);
            Chart chart = null;

            if (chartID != 0)
                 chart = JsonConvert.DeserializeObject<Chart>(GetRequest($"songs/{chartID}").Result);

            if (chart != null)
                callback(chart);
        }


        private static async Task<HttpResponseMessage> PostUploadRequest(string query, dynamic data, string contentType = "application/json")
        {

            var response = await webRequest.PostAsync(query, data);
            return response;
        }

        private static async Task<string> GetRequest(string query)
        {
            Trace.WriteLine(APIURL + query);
            var response = await webRequest.GetStringAsync(query);
            return response;
        }

    }
}
