using System.Collections.Generic;

namespace TheCountBot
{  
    public class Settings
    {
        public string BotIdSecret{ get; set; }

        public int CountingChatId{ get; set; }

        public int MetaCountingChatId{ get; set; }

        public string MySqlConnectionString{ get; set; }

        public int TimerWaitTime{ get; set; }

        public List<string> InsultsForMessingUpTheNumber{ get; set; }
    }
}