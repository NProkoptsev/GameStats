using System;
using System.Collections.Generic;
using System.Globalization;
using GameStats.Server.Entities;
using GameStats.Server.Statistics;
using GameStats.Server.Statistics.PlayerStatistics;
using GameStats.Server.Statistics.ServerStatistics;
using GameStats.Server.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace GameStats.Server.Tests
{
    [TestFixture]
    public class Statistics_Should
    {
        [SetUp]
        public void SetUp()
        {
            _dict = new Dictionary<string, JRaw>();
        }

        private static readonly string _matchInfo1 =
            "{\"map\": \"DM-HelloWorld\"," +
            "\"gameMode\": \"DM\"," +
            "\"fragLimit\": 20," +
            "\"timeLimit\": 20," +
            "\"timeElapsed\": 12.345678," +
            "\"scoreboard\": [" +
            "{" +
            "\"name\": \"Player1\"," +
            "\"frags\": 20," +
            "\"kills\": 21," +
            "\"deaths\": 3" +
            "}," +
            "{" +
            "\"name\": \"Player2\"," +
            "\"frags\": 2," +
            "\"kills\": 2," +
            "\"deaths\": 21" +
            "}" +
            "]" +
            "}";

        private static readonly string _matchInfo2 =
            "{\"map\": \"TM-HelloWorld\"," +
            "\"gameMode\": \"TM\"," +
            "\"fragLimit\": 20," +
            "\"timeLimit\": 20," +
            "\"timeElapsed\": 12.345678," +
            "\"scoreboard\": [" +
            "{" +
            "\"name\": \"Player2\"," +
            "\"frags\": 20," +
            "\"kills\": 21," +
            "\"deaths\": 3" +
            "}," +
            "{" +
            "\"name\": \"Player1\"," +
            "\"frags\": 2," +
            "\"kills\": 2," +
            "\"deaths\": 21" +
            "}," +
            "{" +
            "\"name\": \"Player3\"," +
            "\"frags\": 2," +
            "\"kills\": 2," +
            "\"deaths\": 21" +
            "}" +
            "]" +
            "}";

        private static readonly DateTime _dateTime1 = DateTime.ParseExact("2017-01-24T15:13:00Z",
            "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", CultureInfo.InvariantCulture);

        private static DateTime _dateTime2 = DateTime.ParseExact("2017-01-25T15:13:00Z",
            "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", CultureInfo.InvariantCulture);

        private readonly MatchData _matchData1 = new MatchData("localhost-8080", _dateTime1,
            JsonConvert.DeserializeObject<MatchInfo>(_matchInfo1));

        private readonly MatchData _matchData2 = new MatchData("localhost-8081", _dateTime2,
            JsonConvert.DeserializeObject<MatchInfo>(_matchInfo2));

        private IDictionary<string, JRaw> _dict;


        private void InitializeStat(IBaseStatistics stat)
        {
            for (var i = 0; i < 5; i++)
            {
                stat.Add(_matchData1, "player1");
                stat.Add(_matchData2, "player1");
            }
        }

        [Test]
        public void CorrectlyCalculateAverageMatchesPerDay_WhenAdded()
        {
            var matchData = new MatchData("localhost-8081", _dateTime2,
                JsonConvert.DeserializeObject<MatchInfo>(_matchInfo2));
            var stat = new AverageMatchesPerDay();
            for (var i = 0; i < 7; i++)
            for (var j = i; j < 7; j++)
            {
                matchData.Timestamp = DateTime.Now.AddDays(i);
                stat.Add(matchData, "player1");
            }
            stat.Get(_dict);
            Assert.Less(Math.Abs((double) _dict["averageMatchesPerDay"] - (7 + 6 + 5 + 4 + 3 + 2 + 1) / 7.0), 10e-8);
        }

        [Test]
        public void CorrectlyCalculateAveragePopulation_WhenAdded()
        {
            var stat = new AveragePopulation();
            InitializeStat(stat);
            stat.Get(_dict);
            Assert.Less(Math.Abs((double) _dict["averagePopulation"] - 2.5), 10e-8);
        }


        [Test]
        public void CorrectlyCalculateAverageScoreboardPercent_WhenAdded()
        {
            var stat = new AverageScoreboardPercent();
            InitializeStat(stat);
            stat.Get(_dict);
            Assert.AreEqual((int) _dict["averageScoreboardPercent"], 75);
        }

        [Test]
        public void CorrectlyCalculateFavoriteGameMode_WhenAdded()
        {
            var stat = new FavoriteGameMode();
            InitializeStat(stat);
            stat.Add(_matchData2, "player1");
            stat.Get(_dict);
            Assert.AreEqual((string) _dict["favoriteGameMode"], "TM");
        }

        [Test]
        public void CorrectlyCalculateFavoriteServer_WhenAdded()
        {
            var stat = new FavouriteServer();
            InitializeStat(stat);
            stat.Add(_matchData2, "player1");
            stat.Get(_dict);
            Assert.AreEqual((string) _dict["favouriteServer"], "localhost-8081");
        }

        [Test]
        public void CorrectlyCalculateKillToDeathRatio_WhenAdded()
        {
            var stat = new KillToDeathRatio();
            InitializeStat(stat);
            stat.Get(_dict);
            Assert.Less(Math.Abs((double) _dict["killToDeathRatio"] - 23 / 24.0), 10e-8);
        }

        [Test]
        public void CorrectlyCalculateLastMatchPlayed_WhenAdded()
        {
            var stat = new LastMatchPlayed();
            InitializeStat(stat);
            stat.Get(_dict);
            Assert.AreEqual((string) _dict["lastMatchPlayed"],
                _dateTime2.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'"));
        }

        [Test]
        public void CorrectlyCalculateMaximumMatchesPerDay_WhenAdded()
        {
            var matchData = new MatchData("localhost-8081", _dateTime2,
                JsonConvert.DeserializeObject<MatchInfo>(_matchInfo2));
            var stat = new MaximumMatchesPerDay();
            for (var i = 0; i < 7; i++)
            for (var j = i; j < 7; j++)
            {
                matchData.Timestamp = DateTime.Now.AddDays(i);
                stat.Add(matchData, "player1");
            }
            stat.Get(_dict);
            Assert.AreEqual((int) _dict["maximumMatchesPerDay"], 7);
        }

        [Test]
        public void CorrectlyCalculateMaximumPopulation_WhenAdded()
        {
            var stat = new MaximumPopulation();
            InitializeStat(stat);
            stat.Get(_dict);
            Assert.AreEqual((int) _dict["maximumPopulation"], 3);
        }

        [Test]
        public void CorrectlyCalculateTop5GameModes_WhenAdded()
        {
            var matchData = new MatchData("localhost-8081", _dateTime2,
                JsonConvert.DeserializeObject<MatchInfo>(_matchInfo2));
            var stat = new Top5GameModes();
            for (var i = 0; i < 7; i++)
            for (var j = i; j < 7; j++)
            {
                matchData.Results.GameMode = j.ToString();
                stat.Add(matchData, "player1");
            }
            stat.Get(_dict);
            Assert.AreEqual((string) _dict["top5GameModes"], "[\"6\",\"5\",\"4\",\"3\",\"2\"]");
        }

        [Test]
        public void CorrectlyCalculateTop5Maps_WhenAdded()
        {
            var matchData = new MatchData("localhost-8081", _dateTime2,
                JsonConvert.DeserializeObject<MatchInfo>(_matchInfo2));
            var stat = new Top5Maps();
            for (var i = 0; i < 7; i++)
            for (var j = i; j < 7; j++)
            {
                matchData.Results.Map = j.ToString();
                stat.Add(matchData, "player1");
            }
            stat.Get(_dict);
            Assert.AreEqual((string) _dict["top5Maps"], "[\"6\",\"5\",\"4\",\"3\",\"2\"]");
        }

        [Test]
        public void CorrectlyCalculateTotalMatchesPlayed_WhenAdded()
        {
            var matchData = new MatchData("localhost-8081", _dateTime2,
                JsonConvert.DeserializeObject<MatchInfo>(_matchInfo2));
            var stat = new TotalMatchesPlayed();
            for (var i = 0; i < 7; i++)
            for (var j = i; j < 7; j++)
            {
                matchData.Timestamp = DateTime.Now.AddDays(i);
                stat.Add(matchData, "player1");
            }
            stat.Get(_dict);
            Assert.AreEqual((int) _dict["totalMatchesPlayed"], 7 + 6 + 5 + 4 + 3 + 2 + 1);
        }

        [Test]
        public void CorrectlyCalculateTotalMatchesWon_WhenAdded()
        {
            var stat = new TotalMatchesWon();
            InitializeStat(stat);
            stat.Get(_dict);
            Assert.AreEqual((int) _dict["totalMatchesWon"], 5);
        }

        [Test]
        public void CorrectlyCalculateUniqueServers_WhenAdded()
        {
            var stat = new UniqueServers();
            InitializeStat(stat);
            stat.Get(_dict);
            Assert.AreEqual((int) _dict["uniqueServers"], 2);
        }
    }
}