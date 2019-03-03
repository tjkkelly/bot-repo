using System;
using System.Collections.Generic;

namespace TheCountBot
{
    public class Settings
    {
        public string BotIdSecret{ get; set; }

        public Uri ApiBaseUrl { get; set; }

        public int CountingChatId{ get; set; }

        public int MetaCountingChatId{ get; set; }

        public int TimerWaitTime{ get; set; }

        public List<string> InsultsForMessingUpTheNumber{ get; set; }

        public string ConnectionString{ get; set; }

        public string SqlConnectionStringReadWrite { get; set; }

        public bool IsDebug { get; set; }
    }
}