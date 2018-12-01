using System;
using MediatR;

namespace TheCountBot.Core.Commands
{
    public class AddNewEntryCommand : IRequest
    {
        public string Username
        {
            get;
            set;
        }

        public int Number
        {
            get;
            set;
        }

        public bool IsCorrect 
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
