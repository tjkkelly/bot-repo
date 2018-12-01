using MediatR;

namespace TheCountBot.Core.Queries
{
    public class IsValidEntryQuery : IRequest<bool>
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
    }
}
