using System.Collections.Generic;
using MediatR;
using TheCountBot.Core.DataModels;

namespace TheCountBot.Core.Queries
{
    public class AllMessageHistoryQuery : IRequest<IReadOnlyList<UserMessage>>
    {
    }
}
