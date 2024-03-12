using Microsoft.Extensions.Configuration;
using System.IO;

namespace TheCountBot.Factories
{
    public static class ConfigurationRootFactory
    {
        public static IConfigurationRoot CreateConfigurationBuilder()
        {
            string releaseFileName = $"/private/cntBotSettings.release.json";

            return new ConfigurationBuilder()
                .SetBasePath( Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location ) )
                .AddJsonFile( releaseFileName, optional: true )
                .AddEnvironmentVariables()
                .Build();
        }
    }
}