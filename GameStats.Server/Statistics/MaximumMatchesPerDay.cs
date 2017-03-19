using System;
using System.Collections.Generic;
using GameStats.Server.Storage;
using LiteDB;
using Newtonsoft.Json.Linq;

namespace GameStats.Server.Statistics
{
    internal class MaximumMatchesPerDay : IBaseStatistics
    {
        private int _currentCount;
        private DateTime _currentDate = new DateTime(DateTime.MinValue.Ticks);
        private int _maxCount;

        public void Get(IDictionary<string, JRaw> stats)
        {
            stats.Add("maximumMatchesPerDay", new JRaw(_maxCount.ToString()));
        }

        public void Add(MatchData matchData, string player)
        {
            if (matchData.Timestamp.Date > _currentDate)
            {
                _currentDate = matchData.Timestamp.Date;
                _currentCount = 0;
            }
            if (++_currentCount > _maxCount)
                _maxCount = _currentCount;
        }

        public void Load(IDictionary<string, BsonValue> dictionary)
        {
            _currentDate = dictionary["MaximumMatchesPerDay._currentDate"].AsDateTime;
            _maxCount = dictionary["MaximumMatchesPerDay._maxCount"].AsInt32;
            _currentCount = dictionary["MaximumMatchesPerDay._currentCount"].AsInt32;
        }
    }
}