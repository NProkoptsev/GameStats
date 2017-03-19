namespace GameStats.Server.Storage
{
    internal interface IServerStorage
    {
        void Add(ServerData serverData);
        string GetAll();
        string GetOne(string endpoint);
    }
}