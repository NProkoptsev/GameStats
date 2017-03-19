using System;
using System.IO;
using Fclp;
using LiteDB;

namespace GameStats.Server
{
    internal class EntryPoint
    {
        private static FileStream file;
        private static MemoryStream ms;

        public static void Main(string[] args)
        {
            var commandLineParser = new FluentCommandLineParser<Options>();

            commandLineParser
                .Setup(options => options.Prefix)
                .As("prefix")
                .SetDefault("http://+:8080/")
                .WithDescription("HTTP prefix to listen on");

            commandLineParser
                .SetupHelp("h", "help")
                .WithHeader($"{AppDomain.CurrentDomain.FriendlyName} [--prefix <prefix>]")
                .Callback(text => Console.WriteLine(text));

            if (commandLineParser.Parse(args).HelpCalled)
                return;

            RunServer(commandLineParser.Object);
        }

        private static void RunServer(Options options)
        {
            using (var db = new LiteDatabase(@"MyData.db"))
            {
                using (var server = new StatServer(db))
                {
                    server.Start(options.Prefix);
                    Console.ReadKey(true);
                }
            }
        }


        private class Options
        {
            public string Prefix { get; set; }
        }
    }
}