using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.IO;

namespace TheCountBot.Factories
{
    public static class ConfigurationRootFactory
    {
        public static IConfigurationRoot CreateConfigurationBuilder()
        {
            string debugFileName = $"/private/cntBotSettings.debug.json";
            string releaseFileName = $"/private/cntBotSettings.release.json";

            return new ConfigurationBuilder()
                .SetBasePath( Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location ) )
                .AddJsonFile( releaseFileName, optional: true )
                .AddJsonFile( debugFileName, optional: true )
                .Build();
        }
    }
}