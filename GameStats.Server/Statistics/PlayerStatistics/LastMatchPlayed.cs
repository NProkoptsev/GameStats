using System;
using System.Collections.Generic;
using GameStats.Server.Storage;
using LiteDB;
using Newtonsoft.Json.Linq;

namespace GameStats.Server.Statistics.PlayerStatistics
{
    internal class LastMatchPlayed : IBaseStatistics
    {
        private DateTime _dateTime;

        public void Get(IDictionary<string, JRaw> stats)
        {
            stats.Add("lastMatchPlayed",
                new JRaw(_dateTime.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'")));
        }

        public void Add(MatchData matchData, string player)
        {
            _dateTime = matchData.Timestamp;
        }

        public void Load(IDictionary<string, BsonValue> dictionary)
        {
            _dateTime = dictionary["LastMatchPlayed._dateTime"].AsDateTime;
        }
    }
}