using Microsoft.Extensions.Configuration;

namespace TheCountBot.Configuration
{
   public class ConfigurationRootSettingsProvider : ISettingsProvider
   {
      private readonly IConfigurationRoot _configurationRoot;

       public ConfigurationRootSettingsProvider( IConfigurationRoot configurationRoot )
       {
          _configurationRoot = configurationRoot;
       }

       public string Retrieve( string settingKey )
       {
           return _configurationRoot[settingKey];
       }
   }
}