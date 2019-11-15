using System;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using TheCountBot.Data.Models;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using TheCountBot.Data;
using Microsoft.EntityFrameworkCore;
using TheCountBot.Factories;
using TheCountBot.Data.Repositories;

namespace TheCountBot
{
    class Program
    {
        private static void RegisteredDependencies( IServiceCollection serviceCollection )
        {
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            Settings settings = serviceProvider.GetService<IOptions<Settings>>().Value;

            ITelegramBotClient telegramBotClient = new TelegramBotClient( settings.BotIdSecret );

            serviceCollection.AddSingleton<ITelegramBotClient>( telegramBotClient );
            serviceCollection.AddScoped<ITelegramBotManager, TelegramBotManager>();
            serviceCollection.AddScoped<INumberStoreRepository, NumberStoreRepository>();
            serviceCollection.AddScoped<IStatsManager, StatsManager>();

            serviceCollection.AddDbContext<NumberStoreContext>( options => 
            {
                if ( settings.IsDebug )
                {
                    options.UseInMemoryDatabase( databaseName: "doesntMatter" );
                }
                else
                {
                    options.UseSqlServer( settings.SqlConnectionStringReadWrite );
                }
            }, ServiceLifetime.Transient );
        }

        private static void ConfigureServices( IServiceCollection serviceCollection )
        {
            IConfiguration configuration = ConfigurationRootFactory.CreateConfigurationBuilder();

            serviceCollection.AddOptions();
            serviceCollection.Configure<Settings>( configuration );

            RegisteredDependencies( serviceCollection );
        }

        static async Task Main( string[] args )
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices( serviceCollection );
            
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            await serviceProvider.GetService<ITelegramBotManager>().RunAsync( serviceProvider );
        }
    }
}
