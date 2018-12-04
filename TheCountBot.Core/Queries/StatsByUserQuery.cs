using System.Collections.Generic;
using MediatR;
using TheCountBot.Core.DataModels;

namespace TheCountBot.Core.Queries
{
    public class StatsByUserQuery : IRequest<IReadOnlyList<UserStatistics>>
    {
        public IReadOnlyList<UserMessage> UsersMessages
        {
            get;
            set;
        }
    }
}
