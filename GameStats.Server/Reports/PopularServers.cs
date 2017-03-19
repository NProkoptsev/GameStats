using System.Collections.Generic;
using Fclp.Internals.Extensions;
using GameStats.Server.Storage;
using LiteDB;
using Newtonsoft.Json;

namespace GameStats.Server.Reports
{
    internal class PopularServers : IBaseReport
    {
        private readonly LiteCollection<ServerData> _serverDataCollection;
        private readonly LiteCollection<BsonDocument> _serverStatCollection;

        public PopularServers(LiteDatabase database)
        {
            _serverStatCollection = database.GetCollection("ServerStatistics");
            _serverStatCollection.EnsureIndex("AverageMatchesPerDay._value");
            _serverDataCollection = database.GetCollection<ServerData>("Servers");
            _serverDataCollection.EnsureIndex(x => x.Info.Name);
        }

        public string Get(int? count)
        {
            if (count == null)
                count = 5;
            else if (count > 50)
                count = 50;
            else if (count < 1)
                return "[]";
            var result = new List<Dictionary<string, string>>();
            _serverStatCollection.Find(Query.All("AverageMatchesPerDay._value", Query.Descending), 0, count.Value)
                .ForEach(x =>
                {
                    var server = _serverDataCollection.FindOne(y => y.Endpoint == x["ServerEndpoint"].AsString);
                    if (server == null) return;
                    var dict = new Dictionary<string, string>();
                    dict.Add("endpoint", x["ServerEndpoint"].AsString);
                    dict.Add("name", server.Info.Name);
                    dict.Add("averageMatchesPerDay", x["AverageMatchesPerDay._value"].AsString);
                    result.Add(dict);
                });
            return JsonConvert.SerializeObject(result);
        }
    }
}