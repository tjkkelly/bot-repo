using Microsoft.Extensions.Configuration;
using System.IO;

namespace TheCountBot.Factories
{
    public static class ConfigurationRootFactory
    {
        public static IConfigurationRoot CreateConfigurationBuilder()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location))
                .AddEnvironmentVariables()
                .Build();
        }
    }
}