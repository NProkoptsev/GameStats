using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using NUnit.Framework;

namespace GameStats.Server.Tests
{
    [TestFixture]
    public class StatServer_Should
    {
        private LiteDatabase _db;
        private StatServer _server;

        [OneTimeSetUp]
        public void SetUp()
        {
            var fileName = Path.GetTempPath() + Guid.NewGuid();
            _db = new LiteDatabase(fileName);
            _server = new StatServer(_db);
            _server.Start("http://+:8081/");
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            _server.Dispose();
            _db.Dispose();
        }

        private readonly string matchInfo =
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

        private readonly string serverInfo =
            "{\r\n\t\"name\": \"] My P3rfect Server [\",\r\n\t\"gameModes\": [ \"DM\", \"TDM\" ]\r\n}";


        [Test]
        public void GetError404_WhenTryingToGetStatsFromNotAdvertisedServer()
        {
            var httpWebRequest =
                (HttpWebRequest)
                WebRequest.Create(
                    @"http://localhost:8081/servers/192.168.1.1-8080/stats");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            var e = Assert.Throws<WebException>(() => httpWebRequest.GetResponse());
            Assert.AreEqual(((HttpWebResponse) e.Response).StatusCode, HttpStatusCode.NotFound);
        }

        [Test]
        public void GetError404_WhenTryingToPutStatsFromNotAdvertisedServer()
        {
            var httpWebRequest =
                (HttpWebRequest)
                WebRequest.Create(
                    @"http://localhost:8081/servers/192.168.1.1-8080/stats");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "PUT";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(matchInfo);
                streamWriter.Flush();
                streamWriter.Close();
            }
            var e = Assert.Throws<WebException>(() => httpWebRequest.GetResponse());
            Assert.AreEqual(((HttpWebResponse) e.Response).StatusCode, HttpStatusCode.NotFound);
        }

        [Test]
        public void IgnoreMatches_FromNotAdvertisedServers()
        {
            var httpWebRequest =
                (HttpWebRequest)
                WebRequest.Create(
                    @"http://localhost:8081/servers/192.168.1.1-8080/matches/2017-01-24T15:13:00Z");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "PUT";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(matchInfo);
                streamWriter.Flush();
                streamWriter.Close();
            }
            var e = Assert.Throws<WebException>(() => httpWebRequest.GetResponse());
            Assert.AreEqual(((HttpWebResponse) e.Response).StatusCode, HttpStatusCode.BadRequest);
        }
    }
}