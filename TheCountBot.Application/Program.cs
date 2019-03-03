using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using TheCountBot.Data;
using Microsoft.EntityFrameworkCore;
using TheCountBot.Factories;
using TheCountBot.Data.Repositories;
using MediatR;
using TheCountBot.Application.Clients;
using TheCountBot.Core.Factories;

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
            serviceCollection.AddSingleton<ICountBotApi>( new CountBotApi( settings.ApiBaseUrl ) );
            serviceCollection.AddScoped<TheCountBot.Core.Interfaces.ITelegramBotClient, TheCountBot.Application.Implementations.TelegramBotClient>();
            serviceCollection.AddSingleton( new CustomizedInsultFactory( settings.InsultsForMessingUpTheNumber, new Random() ) );
            serviceCollection.AddMediatR();

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

            await serviceProvider.GetService<ITelegramBotManager>().RunAsync();
        }
    }
}
