using System;
using LiteDB;
using Newtonsoft.Json;

namespace GameStats.Server.Storage
{
    internal class MatchStorage : IMatchStorage
    {
        private readonly LiteCollection<MatchData> _collection;

        public MatchStorage(LiteDatabase database)
        {
            _collection = database.GetCollection<MatchData>("Matches");
            _collection.EnsureIndex(x => x.Endpoint);
            _collection.EnsureIndex(x => x.Timestamp);
        }

        public void Add(MatchData matchData)
        {
            _collection.Insert(matchData);
        }

        public string GetOne(string endpoint, DateTime timeStamp)
        {
            var result = _collection.FindOne(x => x.Endpoint.Equals(endpoint) && x.Timestamp.Equals(timeStamp));
            return result == null ? "" : JsonConvert.SerializeObject(result.Results);
        }

        public string GetAll()
        {
            return JsonConvert.SerializeObject(_collection.FindAll());
        }
    }
}