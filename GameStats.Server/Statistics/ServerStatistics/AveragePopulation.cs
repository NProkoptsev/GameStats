using System.Collections.Generic;
using System.Globalization;
using GameStats.Server.Storage;
using LiteDB;
using Newtonsoft.Json.Linq;

namespace GameStats.Server.Statistics.ServerStatistics
{
    internal class AveragePopulation : IBaseStatistics
    {
        private int _matchesCount;
        private int _populationCount;

        public void Get(IDictionary<string, JRaw> stats)
        {
            double result;
            if (_populationCount != 0)
                result = (double) _populationCount / _matchesCount;
            else
                result = 0;
            stats.Add("averagePopulation",
                new JRaw(result.ToString(CultureInfo.InvariantCulture)));
        }

        public void Add(MatchData matchData, string player)
        {
            _matchesCount++;
            _populationCount += matchData.Results.ScoreBoard.Count;
        }

        public void Load(IDictionary<string, BsonValue> dictionary)
        {
            _matchesCount = dictionary["AveragePopulation._matchesCount"].AsInt32;
            _populationCount = dictionary["AveragePopulation._populationCount"].AsInt32;
        }
    }
}