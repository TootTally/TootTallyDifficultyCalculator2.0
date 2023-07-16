using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public async static Task<List<int>> GetLeaderboardsId(params string[] urls)
        {
            var requests = urls.Select
                (
                    url => GetStringRequest(url)
                ).ToList();

            //Wait for all the requests to finish
            await Task.WhenAll(requests);

            //Get the responses
            var responses = requests.Where(task => task.Result != null).Select
                (
                    task => int.Parse(task.Result)
                );

            return responses.ToList();
        }

        public async static Task<List<Leaderboard>> GetLeaderboards(params string[] urls)
        {
            var requests = urls.Select
                (
                    url => GetStringRequest(url)
                ).ToList();

            //Wait for all the requests to finish
            await Task.WhenAll(requests);

            //Get the responses
            var responses = requests.Where(task => task.Result != null).Select
                (
                    task => JsonConvert.DeserializeObject<Leaderboard>(task.Result)
                );

            return responses.ToList();
        }

        public static void GetChartData(Chart chart, Action<Leaderboard> callback)
        {
            int chartID;
            try
            {
                chartID = int.Parse(GetStringRequestOld($"hashcheck/custom/?songHash={chart.songHash}").Result);
            }
            catch (Exception)
            {
                Trace.WriteLine("Couldn't find songID for " + chart.songHash);
                return;
            }
            Leaderboard leaderboard = null;

            if (chartID != 0)
            {
                try
                {
                    leaderboard = JsonConvert.DeserializeObject<Leaderboard>(GetStringRequestOld($"songs/{chartID}/leaderboard").Result);
                }
                catch (Exception)
                {
                    Trace.WriteLine("Couldn't find leaderboard for " + chartID);
                }
            }
            Parallel.ForEach(leaderboard.results, score => score.tt = (float)MainForm.CalculateScoreTT(chart, score));
            leaderboard.results.OrderBy(x => x.tt);
            if (leaderboard != null)
                callback(leaderboard);
        }


        private static Task<HttpResponseMessage> PostUploadRequest(string query, dynamic data)
        {

            var response = webRequest.PostAsync(query, data);
            return response;
        }

        private async static Task<string> GetStringRequest(string query)
        {
            Trace.WriteLine(APIURL + query);
            try
            {
                return await webRequest.GetStringAsync(query);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        private static Task<string> GetStringRequestOld(string query)
        {
            var reponse = webRequest.GetStringAsync(query);
            return reponse;
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
