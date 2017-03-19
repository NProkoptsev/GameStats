using System.Collections.Generic;
using System.Linq;
using GameStats.Server.Storage;
using LiteDB;
using Newtonsoft.Json.Linq;

namespace GameStats.Server.Statistics.PlayerStatistics
{
    internal class TotalMatchesWon : IBaseStatistics
    {
        private int _count;

        public void Get(IDictionary<string, JRaw> stats)
        {
            stats.Add("totalMatchesWon", new JRaw(_count.ToString()));
        }

        public void Add(MatchData matchData, string player)
        {
            if (matchData.Results.ScoreBoard.First().Name.ToLower().Equals(player))
                _count++;
        }

        public void Load(IDictionary<string, BsonValue> dictionary)
        {
            _count = dictionary["TotalMatchesWon._count"].AsInt32;
        }
    }
}