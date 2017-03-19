using System.Collections.Generic;
using GameStats.Server.Storage;
using LiteDB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GameStats.Server.Statistics
{
    internal abstract class StatisticsManager
    {
        protected StatisticsManager(LiteDatabase database, string collectionName, string entityName)
        {
            Database = database;
            Collection = database.GetCollection(collectionName);
            EntityName = entityName;
            Collection.EnsureIndex(entityName);
        }

        protected LiteCollection<BsonDocument> Collection { get; set; }
        protected string EntityName { get; set; }
        protected LiteDatabase Database { get; set; }
        protected BsonValue Id { get; set; }

        protected List<IBaseStatistics> Stats { get; set; } = new List<IBaseStatistics>();

        internal abstract void Add(MatchData matchData);

        internal virtual void Load(string entity)
        {
        }

        internal virtual void Save(string entity)
        {
        }

        internal virtual string Get(string entity)
        {
            var dictionary = Collection.FindOne(Query.EQ(EntityName, entity));
            if (dictionary == null) return "";
            foreach (var stat in Stats)
                stat.Load(dictionary);
            var result = new Dictionary<string, JRaw>();
            foreach (var stat in Stats)
                stat.Get(result);
            return JsonConvert.SerializeObject(result);
        }
    }
}