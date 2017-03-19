using System.Collections.Generic;
using System.Linq;
using GameStats.Server.Storage;
using LiteDB;
using Newtonsoft.Json.Linq;

namespace GameStats.Server.Statistics.PlayerStatistics
{
    internal class FavoriteGameMode : IBaseStatistics
    {
        private Dictionary<string, int> _modes = new Dictionary<string, int>();

        public void Get(IDictionary<string, JRaw> stats)
        {
            stats.Add("favoriteGameMode", new JRaw(_modes.OrderByDescending(x => x.Value).FirstOrDefault().Key));
        }

        public void Add(MatchData matchData, string player)
        {
            int count;
            _modes.TryGetValue(matchData.Results.GameMode, out count);
            _modes[matchData.Results.GameMode] = count + 1;
        }

        public void Load(IDictionary<string, BsonValue> dictionary)
        {
            _modes = dictionary["FavoriteGameMode._modes"].AsDocument.ToDictionary(x => x.Key, x => x.Value.AsInt32);
        }
    }
}