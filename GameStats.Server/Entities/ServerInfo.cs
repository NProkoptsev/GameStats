using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameStats.Server.Entities
{
    public class ServerInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("gameModes")]
        public List<string> GameModes { get; set; }
    }
}