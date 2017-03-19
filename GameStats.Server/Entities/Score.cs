using Newtonsoft.Json;

namespace GameStats.Server.Entities
{
    public class Score
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("frags")]
        public int Frags { get; set; }

        [JsonProperty("kills")]
        public int Kills { get; set; }

        [JsonProperty("deaths")]
        public int Deaths { get; set; }
    }
}