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
using Microsoft.Extensions.Hosting;

namespace TheCountBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    IConfiguration configuration = ConfigurationRootFactory.CreateConfigurationBuilder();
                    services.AddOptions();
                    services.Configure<Settings>(configuration);

                    services.AddSingleton<ITelegramBotClient>((IServiceProvider sp) => new TelegramBotClient(sp.GetService<IOptions<Settings>>().Value.BotIdSecret));
                    services.AddScoped<INumberStoreRepository, NumberStoreRepository>();
                    services.AddScoped<IStatsManager, StatsManager>();

                    services.AddDbContext<NumberStoreContext>((IServiceProvider sp, DbContextOptionsBuilder options) => options.UseMySQL(sp.GetService<IOptions<Settings>>().Value.MySqlConnectionString), ServiceLifetime.Transient);

                    services.AddScoped<UpdateHandler>();
                    services.AddScoped<ReceiverService>();
                    services.AddHostedService<PollingService>();
                }).Build();

            await host.RunAsync();
        }
    }
}
