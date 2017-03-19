using System.Globalization;
using GameStats.Server.Storage;
using LiteDB;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GameStats.Server.Reports
{
    internal class RecentMatches : IBaseReport
    {
        private readonly LiteCollection<MatchData> _collection;

        public RecentMatches(LiteDatabase database)
        {
            _collection = database.GetCollection<MatchData>("Matches");
            _collection.EnsureIndex(x => x.Timestamp);
        }

        public string Get(int? count)
        {
            if (count == null)
                count = 5;
            else if (count > 50)
                count = 50;
            else if (count < 1)
                return "[]";
            var converter = new IsoDateTimeConverter();
            converter.DateTimeStyles = DateTimeStyles.AssumeUniversal;
            return
                JsonConvert.SerializeObject(_collection.Find(Query.All("Timestamp", Query.Descending), 0, count.Value),
                    converter);
        }
    }
}