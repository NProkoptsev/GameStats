using System;
using System.Collections.Generic;
using System.Globalization;
using GameStats.Server.Storage;
using LiteDB;
using Newtonsoft.Json.Linq;

namespace GameStats.Server.Statistics
{
    internal class AverageMatchesPerDay : IBaseStatistics
    {
        private DateTime _firstDate = new DateTime(DateTime.MinValue.Ticks);
        private int _matchesCount;
        private double _value;
        public static DateTime LastDate { get; set; } = new DateTime(DateTime.MinValue.Ticks);

        public void Get(IDictionary<string, JRaw> stats)
        {
            stats.Add("averageMatchesPerDay",
                new JRaw(_value.ToString(CultureInfo.InvariantCulture)));
        }

        public void Add(MatchData matchData, string player)
        {
            _matchesCount++;
            if (_firstDate.Ticks == DateTime.MinValue.Ticks)
                _firstDate = matchData.Timestamp.Date;
            if (matchData.Timestamp.Date > LastDate)
                LastDate = matchData.Timestamp.Date;
            _value = (double) _matchesCount / ((int) (LastDate - _firstDate).TotalDays + 1);
        }

        public void Load(IDictionary<string, BsonValue> dictionary)
        {
            _firstDate = dictionary["AverageMatchesPerDay._firstDate"].AsDateTime;
            _matchesCount = dictionary["AverageMatchesPerDay._matchesCount"].AsInt32;
            _value = dictionary["AverageMatchesPerDay._value"].AsDouble;
        }
    }
}