using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using GameStats.Server.Reports;
using GameStats.Server.Statistics;
using GameStats.Server.Storage;

namespace GameStats.Server.Routing
{
    internal class Router
    {
        private readonly IDictionary<string, IRoute> _routes;

        public Router(StatisticsManager serverManager, StatisticsManager playerManager, IReportsManager reportsManger,
            IServerStorage serverStorage, IMatchStorage matchStorage, IDictionary<string, IRoute> routes)
        {
            ServerManager = serverManager;
            PlayerManager = playerManager;
            ReportsManger = reportsManger;
            ServerStorage = serverStorage;
            MatchStorage = matchStorage;
            _routes = routes;
        }

        public StatisticsManager ServerManager { get; protected set; }
        public StatisticsManager PlayerManager { get; protected set; }
        public IReportsManager ReportsManger { get; protected set; }
        public IServerStorage ServerStorage { get; protected set; }
        public IMatchStorage MatchStorage { get; protected set; }

        public void Route(HttpListenerContext context)
        {
            foreach (var key in _routes.Keys)
            {
                if (!Regex.IsMatch(context.Request.RawUrl, key)) continue;
                _routes[key].Process(this, context);
                return;
            }
            ErrorRoute.Process(context, 404);
        }
    }
}