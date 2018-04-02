using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration.Binder;

namespace TheCountBot.Configuration
{
   public class ConfigurationRootSettingsProvider
   {
      private readonly IConfigurationRoot _configurationRoot;

       public ConfigurationRootSettingsProvider( IConfigurationRoot configurationRoot )
       {
          _configurationRoot = configurationRoot;
       }

       public object Retrieve( string settingKey )
       {
           return _configurationRoot[settingKey];
       }

       public List<string> GetSection( string settingKey )
       {
           return _configurationRoot.GetSection( settingKey ).Get<List<string>>();
       }
   }
}