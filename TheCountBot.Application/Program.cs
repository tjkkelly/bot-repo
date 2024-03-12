using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using TheCountBot.Data;
using TheCountBot.Factories;
using TheCountBot.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace TheCountBot
{
    class Program
    {
        private static void RegisteredDependencies(IServiceCollection serviceCollection)
        {
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            serviceCollection.AddSingleton<ITelegramBotClient>((IServiceProvider sp) => new TelegramBotClient(sp.GetService<IOptions<Settings>>().Value.BotIdSecret));
            serviceCollection.AddScoped<ITelegramBotManager, TelegramBotManager>();
            serviceCollection.AddScoped<INumberStoreRepository, NumberStoreRepository>();
            serviceCollection.AddScoped<IStatsManager, StatsManager>();

            serviceCollection.AddDbContext<NumberStoreContext>((IServiceProvider sp, DbContextOptionsBuilder options) => options.UseMySQL(sp.GetService<IOptions<Settings>>().Value.MySqlConnectionString), ServiceLifetime.Transient);
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            IConfiguration configuration = ConfigurationRootFactory.CreateConfigurationBuilder();

            serviceCollection.AddOptions();
            serviceCollection.Configure<Settings>(configuration);

            RegisteredDependencies(serviceCollection);
        }

        static async Task Main(string[] args)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            await serviceProvider.GetService<ITelegramBotManager>().RunAsync(serviceProvider);
        }
    }
}
