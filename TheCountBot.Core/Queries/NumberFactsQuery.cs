using MediatR;
using TheCountBot.Core.DataModels;

namespace TheCountBot.Core.Queries
{
    public class NumberFactsQuery : IRequest<NumberFacts>
    {
        public int Number { get; set; }
    }
}
