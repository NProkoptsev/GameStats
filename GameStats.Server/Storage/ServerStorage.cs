using LiteDB;
using Newtonsoft.Json;

namespace GameStats.Server.Storage
{
    internal class ServerStorage : IServerStorage
    {
        private readonly LiteCollection<ServerData> _collection;

        public ServerStorage(LiteDatabase database)
        {
            _collection = database.GetCollection<ServerData>("Servers");
            _collection.EnsureIndex(x => x.Endpoint);
        }

        public void Add(ServerData serverData)
        {
            _collection.Upsert(serverData);
        }

        public string GetOne(string endpoint)
        {
            var result = _collection.FindOne(x => x.Endpoint.Equals(endpoint));
            return result == null ? "" : JsonConvert.SerializeObject(result.Info);
        }

        public string GetAll()
        {
            return JsonConvert.SerializeObject(_collection.FindAll());
        }
    }
}