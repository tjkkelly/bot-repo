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

        static void Main(string[] args)
        {
            Settings.Initialize( new ConfigurationRootSettingsProvider( new ConfigurationBuilder().AddJsonFile( "cntBotSettings.debug.json" ).Build() ) );

            _botManager = new TelegramBotManager();
            _botManager.StartupAsync().Wait();

            Thread.Sleep(Timeout.Infinite);

            _botManager.ShutdownAsync().Wait();
        }
    }
}