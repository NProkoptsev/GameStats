using System.Collections.Generic;
using System.Linq;
using GameStats.Server.Storage;
using LiteDB;
using Newtonsoft.Json.Linq;

namespace GameStats.Server.Statistics.PlayerStatistics
{
    internal class FavouriteServer : IBaseStatistics
    {
        private Dictionary<string, int> _servers = new Dictionary<string, int>();

        public void Get(IDictionary<string, JRaw> stats)
        {
            stats.Add("favouriteServer", new JRaw(_servers.OrderByDescending(x => x.Value).FirstOrDefault().Key));
        }

        public void Add(MatchData matchData, string player)
        {
            int count;
            _servers.TryGetValue(matchData.Endpoint, out count);
            _servers[matchData.Endpoint] = count + 1;
        }

        public void Load(IDictionary<string, BsonValue> dictionary)
        {
            _servers = dictionary["FavouriteServer._servers"].AsDocument.ToDictionary(x => x.Key, x => x.Value.AsInt32);
        }
    }
}