using System;

namespace TheCountBot.Core.DataModels
{
    public class UserMessage
    {
        public string Username
        {
            get;
            internal set;
        }

        public int Number
        {
            get;
            internal set;
        }

        public bool IsCorrect
        {
            get;
            internal set;
        }

        public DateTime Timestamp
        {
            get;
            internal set;
        }
    }
}
