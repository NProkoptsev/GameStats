using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameStats.Server.Entities
{
    public class MatchInfo
    {
        [JsonProperty("map")]
        public string Map { get; set; }

        [JsonProperty("gameMode")]
        public string GameMode { get; set; }

        [JsonProperty("fragLimit")]
        public int FragLimit { get; set; }

        [JsonProperty("timeLimit")]
        public int TimeLimit { get; set; }

        [JsonProperty("timeElapsed")]
        public double TimeElapsed { get; set; }

        [JsonProperty("scoreBoard")]
        public List<Score> ScoreBoard { get; set; }
    }
}