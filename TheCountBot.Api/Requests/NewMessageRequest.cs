using System;

namespace TheCountBot.Api.Requests
{
    public class NewMessageRequest
    {
        public string Username
        {
            get;
            set;
        }

        public string Number
        {
            get;
            set;
        }

        public DateTime Timestamp
        {
            get;
            set;
        }
    }
}
