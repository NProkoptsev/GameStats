using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using GameStats.Server.Entities;
using GameStats.Server.Reports;
using GameStats.Server.Routing;
using GameStats.Server.Statistics;
using GameStats.Server.Statistics.PlayerStatistics;
using GameStats.Server.Statistics.ServerStatistics;
using GameStats.Server.Storage;
using LiteDB;

namespace GameStats.Server
{
    internal class StatServer : IDisposable
    {
        private readonly LiteDatabase _database;

        private readonly HttpListener _listener;
        private readonly IMatchStorage _matchStorage;

        private readonly IReportsManager _reportsManager;
        private readonly IDictionary<string, IRoute> _routes = CreateRoutes();
        private readonly IServerStorage _serverStorage;
        private bool _disposed;
        private volatile bool _isRunning;

        private Thread _listenerThread;

        public StatServer(LiteDatabase database)
        {
            _listener = new HttpListener();
            _database = database;
            _matchStorage = new MatchStorage(_database);
            _reportsManager = CreateReportsManager(_database);
            _serverStorage = new ServerStorage(_database);
            InitializeStatisticManager(_database);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            var timeCol = _database.GetCollection<DateWrapper>("LastMatch");
            timeCol.Upsert(0, new DateWrapper {Date = AverageMatchesPerDay.LastDate});

            Stop();

            _listener.Close();
        }

        public void Start(string prefix)
        {
            lock (_listener)
            {
                if (!_isRunning)
                {
                    _listener.Prefixes.Clear();
                    _listener.Prefixes.Add(prefix);
                    try
                    {
                        _listener.Start();
                    }
                    catch (HttpListenerException hlex)
                    {
                        Console.Error.WriteLine(hlex.Message);
                        return;
                    }

                    _listenerThread = new Thread(Listen)
                    {
                        IsBackground = true,
                        Priority = ThreadPriority.Highest
                    };
                    _listenerThread.Start();

                    _isRunning = true;
                }
            }
        }

        public void Stop()
        {
            lock (_listener)
            {
                if (!_isRunning)
                    return;

                _listener.Stop();

                _listenerThread.Abort();
                _listenerThread.Join();

                _isRunning = false;
            }
        }

        private async void Listen()
        {
            while (true)
                try
                {
                    if (_listener.IsListening)
                    {
                        var context = _listener.GetContext();
                        Task.Run(() => HandleContextAsync(context));
                    }
                    else
                    {
                        Thread.Sleep(0);
                    }
                }
                catch (ThreadAbortException)
                {
                    return;
                }
                catch (Exception error)
                {
                    Console.Error.WriteLine("Error in Listening \r\n" + error);
                    Console.WriteLine(error.StackTrace);
                }
        }

        public static void InitializeStatisticManager(LiteDatabase db)
        {
            ServerStatisticsManager.Register(typeof(TotalMatchesPlayed));
            ServerStatisticsManager.Register(typeof(MaximumMatchesPerDay));
            ServerStatisticsManager.Register(typeof(AverageMatchesPerDay));
            ServerStatisticsManager.Register(typeof(MaximumPopulation));
            ServerStatisticsManager.Register(typeof(AveragePopulation));
            ServerStatisticsManager.Register(typeof(Top5GameModes));
            ServerStatisticsManager.Register(typeof(Top5Maps));

            PlayerStatisticsManager.Register(typeof(TotalMatchesPlayed));
            PlayerStatisticsManager.Register(typeof(TotalMatchesWon));
            PlayerStatisticsManager.Register(typeof(FavouriteServer));
            PlayerStatisticsManager.Register(typeof(UniqueServers));
            PlayerStatisticsManager.Register(typeof(FavoriteGameMode));
            PlayerStatisticsManager.Register(typeof(AverageScoreboardPercent));
            PlayerStatisticsManager.Register(typeof(MaximumMatchesPerDay));
            PlayerStatisticsManager.Register(typeof(AverageMatchesPerDay));
            PlayerStatisticsManager.Register(typeof(LastMatchPlayed));
            PlayerStatisticsManager.Register(typeof(KillToDeathRatio));

            var timeCol = db.GetCollection<DateWrapper>("LastMatch");
            var date = timeCol.FindById(0);
            AverageMatchesPerDay.LastDate = date?.Date ?? new DateTime(DateTime.MinValue.Ticks);
        }


        public static IReportsManager CreateReportsManager(LiteDatabase database)
        {
            var manager = new ReportsManager();
            manager.Register("recent-matches", new RecentMatches(database));
            manager.Register("best-players", new BestPlayers(database));
            manager.Register("popular-servers", new PopularServers(database));
            return manager;
        }

        private static IDictionary<string, IRoute> CreateRoutes()
        {
            var routes = new Dictionary<string, IRoute>
            {
                [@"^/servers/[^/]+\-[^/]+/info$"] = new ServersEndpointInfoRoute(),
                [@"^/servers/[^/]+\-[^/]+/matches/[^/]+$"] = new ServersEndpointMatchesRoute(),
                [@"^/servers/info$"] = new ServersInfoRoute(),
                [@"^/servers/[^/]+\-[^/]+/stats$"] = new ServersEndpointStatsRoute(),
                [@"^/players/[^\/]+/stats$"] = new PlayersStatsRoute(),
                [@"^/reports/[a-z-]+[/0-9]*$"] = new ReportsRoute()
            };
            return routes;
        }

        private async Task HandleContextAsync(HttpListenerContext listenerContext)
        {
            try
            {
                var serverManager = new ServerStatisticsManager(_database, "ServerStatistics", "ServerEndpoint");
                var playerManager = new PlayerStatisticsManager(_database, "PlayerStatistics", "PlayerName");
                var router = new Router(serverManager, playerManager, _reportsManager, _serverStorage, _matchStorage,
                    _routes);
                router.Route(listenerContext);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Internal server error " + 500 + " while perfoming request:  " +
                                        listenerContext.Request.HttpMethod + "  " +
                                        listenerContext.Request.RawUrl + " \r\n");
                Console.WriteLine(e.StackTrace);
                listenerContext.Response.StatusCode = 500;
                listenerContext.Response.Close();
            }
        }
    }
}