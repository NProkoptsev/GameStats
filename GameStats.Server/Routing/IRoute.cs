using System.Net;

namespace GameStats.Server.Routing
{
    internal interface IRoute
    {
        void Process(Router router, HttpListenerContext context);
    }
}