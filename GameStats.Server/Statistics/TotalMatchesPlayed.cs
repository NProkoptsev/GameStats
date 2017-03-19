using System.Collections.Generic;
using GameStats.Server.Storage;
using LiteDB;
using Newtonsoft.Json.Linq;

namespace GameStats.Server.Statistics
{
    internal class TotalMatchesPlayed : IBaseStatistics
    {
        private int _count;


        public void Get(IDictionary<string, JRaw> stats)
        {
            stats.Add("totalMatchesPlayed", new JRaw(_count.ToString()));
        }

        public void Add(MatchData matchData, string player)
        {
            _count++;
        }

        public void Load(IDictionary<string, BsonValue> dictionary)
        {
            _count = dictionary["TotalMatchesPlayed._count"].AsInt32;
        }
    }
}