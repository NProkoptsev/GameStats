namespace GameStats.Server.Reports
{
    internal interface IBaseReport
    {
        string Get(int? count);
    }
}