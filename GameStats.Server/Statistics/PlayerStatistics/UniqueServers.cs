using System.Collections.Generic;
using System.Linq;
using GameStats.Server.Storage;
using LiteDB;
using Newtonsoft.Json.Linq;

namespace GameStats.Server.Statistics.PlayerStatistics
{
    internal class UniqueServers : IBaseStatistics
    {
        private HashSet<string> _servers = new HashSet<string>();

        public void Get(IDictionary<string, JRaw> stats)
        {
            stats.Add("uniqueServers", new JRaw(_servers.Count.ToString()));
        }

        public void Add(MatchData matchData, string player)
        {
            _servers.Add(matchData.Endpoint);
        }

        public void Load(IDictionary<string, BsonValue> dictionary)
        {
            _servers = new HashSet<string>(dictionary["UniqueServers._servers"].AsArray.Select(x => x.AsString));
        }
    }
}