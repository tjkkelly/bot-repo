using MediatR;

namespace TheCountBot.Core.Commands
{
    public class SendIncorrectNumberCommand : IRequest
    {
        public string Username
        {
            get;
            set;
        }
    }
}
