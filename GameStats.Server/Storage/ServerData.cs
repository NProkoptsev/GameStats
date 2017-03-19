using GameStats.Server.Entities;
using LiteDB;
using Newtonsoft.Json;

namespace GameStats.Server.Storage
{
    public class ServerData
    {
        public ServerData()
        {
        }

        public ServerData(string endpoint, ServerInfo serverInfo)
        {
            Endpoint = endpoint;
            Info = serverInfo;
        }

        [JsonProperty("endpoint")]
        [BsonId]
        public string Endpoint { get; set; }

        [JsonProperty("info")]
        public ServerInfo Info { get; set; }
    }
}