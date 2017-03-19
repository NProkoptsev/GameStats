using System;

namespace GameStats.Server.Storage
{
    internal interface IMatchStorage
    {
        void Add(MatchData matchData);
        string GetAll();
        string GetOne(string endpoint, DateTime timeStamp);
    }
}