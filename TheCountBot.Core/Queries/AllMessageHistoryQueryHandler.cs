using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TheCountBot.Data.Repositories;
using System.Linq;
using TheCountBot.Core.DataModels;

namespace TheCountBot.Core.Queries
{
    public class AllMessageHistoryQueryHandler : IRequestHandler<AllMessageHistoryQuery, IReadOnlyList<UserMessage>>
    {
        private readonly INumberStoreRepository _numberStoreRepository;

        public AllMessageHistoryQueryHandler( INumberStoreRepository numberStoreRepository )
        {
            _numberStoreRepository = numberStoreRepository;
        }

        public async Task<IReadOnlyList<UserMessage>> Handle( AllMessageHistoryQuery request, CancellationToken cancellationToken )
        {
            return ( await _numberStoreRepository.GetHistoryAsync() )
                .Select( m => new UserMessage
                {
                    Username = m.Username,
                    Number = m.Number,
                    IsCorrect = m.Correct,
                    Timestamp = m.Timestamp
                } )
                .ToList();
        }
    }
}
