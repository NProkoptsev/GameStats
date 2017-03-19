namespace GameStats.Server.Reports
{
    internal interface IReportsManager
    {
        string Get(string name, int? count);
        void Register(string name, IBaseReport report);
        bool Contains(string name);
    }
}