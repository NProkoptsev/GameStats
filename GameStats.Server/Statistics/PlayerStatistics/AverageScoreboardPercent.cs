using System.Collections.Generic;
using System.Globalization;
using GameStats.Server.Storage;
using LiteDB;
using Newtonsoft.Json.Linq;

namespace GameStats.Server.Statistics.PlayerStatistics
{
    internal class AverageScoreboardPercent : IBaseStatistics
    {
        private int _count;
        private double _totalPercent;

        public void Get(IDictionary<string, JRaw> stats)
        {
            stats.Add("averageScoreboardPercent",
                new JRaw((_totalPercent / _count).ToString(CultureInfo.InvariantCulture)));
        }

        public void Add(MatchData matchData, string player)
        {
            // scoreboardPercent = playersBelowCurrent / (totalPlayers - 1) * 100%​.
            _count++;
            var scoreboard = matchData.Results.ScoreBoard;
            if (scoreboard.Count - 1 == 0)
                _totalPercent += 100;
            else
                _totalPercent +=
                    (double) (scoreboard.Count - 1 - scoreboard.FindIndex(x => x.Name.ToLower().Equals(player))) /
                    (scoreboard.Count - 1) * 100;
        }

        public void Load(IDictionary<string, BsonValue> dictionary)
        {
            _count = dictionary["AverageScoreboardPercent._count"].AsInt32;
            _totalPercent = dictionary["AverageScoreboardPercent._totalPercent"].AsInt32;
        }
    }
}