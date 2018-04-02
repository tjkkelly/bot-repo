using System;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using TheCountBot.Configuration;

namespace TheCountBot
{
    class Program
    {
        private static IConfiguration _configuration { get; set; }

        private static TelegramBotManager _botManager;

        private static string releaseMode = "debug"; // alt is release

        static void Main(string[] args)
        {
            string fileName = $"cntBotSettings.{releaseMode}.json";
            Settings.Initialize( new ConfigurationRootSettingsProvider( new ConfigurationBuilder().AddJsonFile( fileName ).Build() ) );

            _botManager = new TelegramBotManager();
            _botManager.StartupAsync().Wait();

            Thread.Sleep(Timeout.Infinite);

            _botManager.ShutdownAsync().Wait();
        }
    }
}