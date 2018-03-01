using System;

namespace TheCountBot.Configuration
{  
    public static class Settings
    {
        private static ISettingsProvider _settingsProvider = null;

        public static string BotIdSecret => ThrowIfNotInitialized( "botIdSecret" );

        public static int CountingChatId => Convert.ToInt32( ThrowIfNotInitialized( "countingChatId" ) );

        public static int MetaCountingChatId => Convert.ToInt32( ThrowIfNotInitialized( "metaCountingChatId" ) ); 

        public static void Initialize( ISettingsProvider settingsProvider )
        {
            _settingsProvider = settingsProvider;
        }

        private static string ThrowIfNotInitialized( string configurationKey )
        {
            if ( _settingsProvider == null )
            {
                throw new InvalidOperationException( "Configuration root has not been initialized. Call Settings.Initialize on app startup." );
            }

            return _settingsProvider.Retrieve( configurationKey );
        }
    }
}