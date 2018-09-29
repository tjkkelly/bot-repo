﻿using System;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using TheCountBot.Models;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;

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
        }

        private static void ConfigureServices( IServiceCollection serviceCollection )
        {
            string debugFileName = $"cntBotSettings.debug.json";
            string releaseFileName = $"cntBotSettings.release.json";

            IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath( Directory.GetCurrentDirectory() )
            .AddJsonFile( releaseFileName, optional: true )
            .AddJsonFile( debugFileName, optional: true )
            .Build();

            serviceCollection.AddOptions();
            serviceCollection.Configure<Settings>( configuration );

            RegisteredDependencies( serviceCollection );
        }

        static async Task Main(string[] args)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices( serviceCollection );
            
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            await serviceProvider.GetService<ITelegramBotManager>().RunAsync();
        }
    }
}
