using System.Collections.Generic;
using GameStats.Server.Storage;
using LiteDB;
using Newtonsoft.Json.Linq;

namespace GameStats.Server.Statistics
{
    internal interface IBaseStatistics
    {
        void Get(IDictionary<string, JRaw> stats);
        void Add(MatchData matchData, string player);
        void Load(IDictionary<string, BsonValue> dictionary);
    }
}