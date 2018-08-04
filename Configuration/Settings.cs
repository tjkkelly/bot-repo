using System;
using System.Collections;
using System.Collections.Generic;

namespace TheCountBot.Configuration
{  
    public class Settings
    {
        public string BotIdSecret{ get; set; }

        public int CountingChatId{ get; set; }

        public int MetaCountingChatId{ get; set; }

        public int TimerWaitTime{ get; set; }

        public List<string> InsultsForMessingUpTheNumber{ get; set; }

        public string ConnectionString{ get; set; }
    }
}