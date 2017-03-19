using System.Collections.Generic;
using System.Linq;
using GameStats.Server.Storage;
using LiteDB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GameStats.Server.Statistics.ServerStatistics
{
    internal class Top5Maps : IBaseStatistics
    {
        private Dictionary<string, int> _maps = new Dictionary<string, int>();

        public void Get(IDictionary<string, JRaw> stats)
        {
            stats.Add("top5Maps",
                new JRaw(JsonConvert.SerializeObject(_maps.OrderByDescending(x => x.Value).Take(5).Select(x => x.Key))));
        }

        public void Add(MatchData matchData, string player)
        {
            int count;
            _maps.TryGetValue(matchData.Results.Map, out count);
            _maps[matchData.Results.Map] = count + 1;
        }

        public void Load(IDictionary<string, BsonValue> dictionary)
        {
            _maps = dictionary["Top5Maps._maps"].AsDocument.ToDictionary(x => x.Key, x => x.Value.AsInt32);
        }
    }
}