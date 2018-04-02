using System;
using System.Collections;
using System.Collections.Generic;

namespace TheCountBot.Configuration
{  
    public static class Settings
    {
        private static ConfigurationRootSettingsProvider _settingsProvider = null;

        public static string BotIdSecret => ThrowIfStringNotInitialized( "botIdSecret" );

        public static int CountingChatId => Convert.ToInt32( ThrowIfStringNotInitialized( "countingChatId" ) );

        public static int MetaCountingChatId => Convert.ToInt32( ThrowIfStringNotInitialized( "metaCountingChatId" ) );

        public static int TimerWaitTime => Convert.ToInt32( ThrowIfStringNotInitialized("timerWaitTime") );

        public static List<string> InsultsForMessingUpTheNumber => ThrowIfArrayNotInitialized("insultsForMessingUpTheNumber");

        public static void Initialize( ConfigurationRootSettingsProvider settingsProvider )
        {
            _settingsProvider = settingsProvider;
        }

        private static string ThrowIfStringNotInitialized( string configurationKey )
        {
            if ( _settingsProvider == null )
            {
                throw new InvalidOperationException( "Configuration root has not been initialized. Call Settings.Initialize on app startup." );
            }

            return _settingsProvider.Retrieve( configurationKey ).ToString();
        }

        private static List<string> ThrowIfArrayNotInitialized( string configurationKey )
        {
            if ( _settingsProvider == null )
            {
                throw new InvalidOperationException( "Configuration root has not been initialized. Call Settings.Initialize on app startup." );
            }

            return _settingsProvider.GetSection( configurationKey );
        }
    }
}