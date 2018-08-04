using System;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using TheCountBot.Configuration;
using TheCountBot.Models;

namespace TheCountBot
{
    class Program
    {
        private static IConfiguration _configuration { get; set; }

        private static TelegramBotManager _botManager;
        
        private static ReleaseMode releaseMode = ReleaseMode.Debug;

        static void Main(string[] args)
        {
            string fileName = $"cntBotSettings.{releaseMode.ToString()}.json";
            Settings.Initialize( new ConfigurationRootSettingsProvider( new ConfigurationBuilder().AddJsonFile( fileName ).Build() ) );

            _botManager = new TelegramBotManager();
            _botManager.StartupAsync().Wait();

            Thread.Sleep(Timeout.Infinite);

            _botManager.ShutdownAsync().Wait();
        }
    }

    internal enum ReleaseMode
    {
        Debug = 0,
        Release = 1
    }
}
