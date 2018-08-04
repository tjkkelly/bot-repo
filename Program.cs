using System;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using TheCountBot.Configuration;
using TheCountBot.Models;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace TheCountBot
{
    class Program
    {
        private static ReleaseMode releaseMode = ReleaseMode.Debug;

        private static void ConfigureServices( IServiceCollection serviceCollection )
        {
            string fileName = $"cntBotSettings.{releaseMode.ToString()}.json";

            IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath( Directory.GetCurrentDirectory() )
            .AddJsonFile( fileName )
            .Build();

            serviceCollection.AddOptions();
            serviceCollection.Configure<Settings>( configuration );
        }

        static void Main(string[] args)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            serviceProvider.GetService<TelegramBotManager>().Run().Wait();
        }
    }

    internal enum ReleaseMode
    {
        Debug = 0,
        Release = 1
    }
}
