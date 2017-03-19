using System.Collections.Generic;
using System.Linq;
using GameStats.Server.Storage;
using LiteDB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GameStats.Server.Statistics.ServerStatistics
{
    internal class Top5GameModes : IBaseStatistics
    {
        private Dictionary<string, int> _modes = new Dictionary<string, int>();

        public void Get(IDictionary<string, JRaw> stats)
        {
            var a = _modes.OrderByDescending(x => x.Value).Take(5).Select(x => x.Key).ToList()[0].Length;
            stats.Add("top5GameModes", new JRaw(
                JsonConvert.SerializeObject(_modes.OrderByDescending(x => x.Value).Take(5).Select(x => x.Key).ToList())));
        }

        public void Add(MatchData matchData, string player)
        {
            int count;
            _modes.TryGetValue(matchData.Results.GameMode, out count);
            _modes[matchData.Results.GameMode] = count + 1;
        }

        public void Load(IDictionary<string, BsonValue> dictionary)
        {
            _modes = dictionary["Top5GameModes._modes"].AsDocument.ToDictionary(x => x.Key, x => x.Value.AsInt32);
        }
    }
}