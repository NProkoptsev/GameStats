using System;
using GameStats.Server.Entities;
using Newtonsoft.Json;

namespace GameStats.Server.Storage
{
    public class MatchData
    {
        public MatchData()
        {
        }

        public MatchData(string endpoint, DateTime timestamp, MatchInfo results)
        {
            Endpoint = endpoint;
            Timestamp = timestamp;
            Results = results;
        }

        [JsonProperty("server")]
        public string Endpoint { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("results")]
        public MatchInfo Results { get; set; }
    }
}