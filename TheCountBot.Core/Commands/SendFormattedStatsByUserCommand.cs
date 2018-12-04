using System.Collections.Generic;
using MediatR;
using TheCountBot.Core.DataModels;

namespace TheCountBot.Core.Commands
{
    public class SendFormattedStatsByUserCommand : IRequest
    {
        public IList<UserStatistics> UsersStatistics { get; set; }
    }
}
