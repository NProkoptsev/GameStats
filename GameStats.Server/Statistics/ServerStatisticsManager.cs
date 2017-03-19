using System;
using System.Collections.Generic;
using System.Reflection;
using GameStats.Server.Helpers;
using GameStats.Server.Storage;
using LiteDB;

namespace GameStats.Server.Statistics
{
    internal class ServerStatisticsManager : StatisticsManager
    {
        private readonly object _lock = new object();


        public ServerStatisticsManager(LiteDatabase database, string collectionName, string entityName)
            : base(database, collectionName, entityName)
        {
            foreach (var constructor in Constructors)
                Stats.Add((IBaseStatistics) constructor.Invoke(null));
        }

        private static IList<ConstructorInfo> Constructors { get; } = new List<ConstructorInfo>();
        private static IDictionary<Type, MethodInfo> Resets { get; } = new Dictionary<Type, MethodInfo>();
        private static IDictionary<Type, MethodInfo> Saves { get; } = new Dictionary<Type, MethodInfo>();

        internal override void Add(MatchData matchData)
        {
            lock (_lock)
            {
                Load(matchData.Endpoint);
                foreach (var stat in Stats)
                    stat.Add(matchData, null);
                Save(matchData.Endpoint);
            }
        }

        internal override void Load(string entity)
        {
            Id = -1;
            var dictionary = Collection.FindOne(Query.EQ(EntityName, entity));
            if (dictionary == null)
            {
                foreach (var stat in Stats)
                    Resets[stat.GetType()].Invoke(null, new object[] {stat});
            }
            else
            {
                Id = dictionary["_id"];
                foreach (var stat in Stats)
                    stat.Load(dictionary);
            }
        }

        internal override void Save(string entity)
        {
            var dictionary = new Dictionary<string, BsonValue>();
            dictionary.Add(EntityName, entity);
            foreach (var stat in Stats)
                Saves[stat.GetType()].Invoke(null, new object[] {dictionary, stat});
            if (Id == -1)
                Collection.Insert(new BsonDocument(dictionary));
            else
                Collection.Update(Id, new BsonDocument(dictionary));
        }

        internal static void Register(Type type)
        {
            Constructors.Add(type.GetConstructor(Type.EmptyTypes));
            Saves.Add(type, typeof(FieldBackuper<>).MakeGenericType(type)
                .GetMethod("Save"));
            Resets.Add(type, typeof(FieldBackuper<>).MakeGenericType(type)
                .GetMethod("Reset"));
        }
    }
}