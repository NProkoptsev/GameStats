using System.Collections.Generic;
using System.Globalization;
using GameStats.Server.Storage;
using LiteDB;
using Newtonsoft.Json.Linq;

namespace GameStats.Server.Statistics.PlayerStatistics
{
    internal class KillToDeathRatio : IBaseStatistics
    {
        private int _deaths;
        private int _kills;
        private double _value;

        public void Get(IDictionary<string, JRaw> stats)
        {
            stats.Add("killToDeathRatio", new JRaw(_value.ToString(CultureInfo.InvariantCulture)));
        }

        public void Add(MatchData matchData, string player)
        {
            var score = matchData.Results.ScoreBoard.FindIndex(x => x.Name.ToLower().Equals(player));
            _kills += matchData.Results.ScoreBoard[score].Kills;
            _deaths += matchData.Results.ScoreBoard[score].Deaths;
            if (_deaths != 0)
                _value = (double) _kills / _deaths;
            else
                _value = 0;
        }

        public void Load(IDictionary<string, BsonValue> dictionary)
        {
            _kills = dictionary["KillToDeathRatio._kills"].AsInt32;
            _deaths = dictionary["KillToDeathRatio._deaths"].AsInt32;
            _value = dictionary["KillToDeathRatio._value"].AsDouble;
        }
    }
}