using System.Collections.Generic;
using GameStats.Server.Storage;
using LiteDB;
using Newtonsoft.Json.Linq;

namespace GameStats.Server.Statistics.ServerStatistics
{
    internal class MaximumPopulation : IBaseStatistics
    {
        private int _max;

        public void Get(IDictionary<string, JRaw> stats)
        {
            stats.Add("maximumPopulation", new JRaw(_max.ToString()));
        }

        public void Add(MatchData matchData, string player)
        {
            var count = matchData.Results.ScoreBoard.Count;
            if (count > _max)
                _max = count;
        }

        public void Load(IDictionary<string, BsonValue> dictionary)
        {
            _max = dictionary["MaximumPopulation._max"].AsInt32;
        }
    }
}