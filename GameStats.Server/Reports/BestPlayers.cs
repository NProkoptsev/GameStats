using System.Collections.Generic;
using Fclp.Internals.Extensions;
using LiteDB;
using Newtonsoft.Json;

namespace GameStats.Server.Reports
{
    internal class BestPlayers : IBaseReport
    {
        private readonly LiteCollection<BsonDocument> _collection;

        public BestPlayers(LiteDatabase database)
        {
            _collection = database.GetCollection("PlayerStatistics");
            _collection.EnsureIndex("KillToDeathRatio._value");
        }

        public string Get(int? count)
        {
            if (count == null)
                count = 5;
            else if (count > 50)
                count = 50;
            else if (count < 1)
                return "[]";
            var result = new List<Dictionary<string, string>>();
            _collection.Find(Query.All("KillToDeathRatio._value", Query.Descending), 0, count.Value).ForEach(x =>
            {
                if (x["KillToDeathRatio._deaths"].AsInt32 == 0 || x["TotalMatchesPlayed._count"].AsInt32 < 10) return;
                var dict = new Dictionary<string, string>
                {
                    {"name", x["PlayerName"].AsString},
                    {"killToDeathRatio", x["KillToDeathRatio._value"].AsString}
                };
                result.Add(dict);
            });
            return JsonConvert.SerializeObject(result);
        }
    }
}