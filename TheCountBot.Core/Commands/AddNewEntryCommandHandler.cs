using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TheCountBot.Data.Models;
using TheCountBot.Data.Repositories;

namespace TheCountBot.Core.Commands
{
    public class AddNewEntryCommandHandler : IRequestHandler<AddNewEntryCommand>
    {
        private readonly INumberStoreRepository _numberStoreRepository;

        public AddNewEntryCommandHandler(INumberStoreRepository numberStoreRepository)
        {
           _numberStoreRepository = numberStoreRepository;
        }

        public async Task<Unit> Handle( AddNewEntryCommand request, CancellationToken cancellationToken )
        {
            await _numberStoreRepository.AddNewMessageEntryAsync( new MessageEntry
            {
                Username = request.Username,
                Number = request.Number,
                Correct = request.IsCorrect,
                Timestamp = request.Timestamp
            });

            return Unit.Value;
        }
    }
}
