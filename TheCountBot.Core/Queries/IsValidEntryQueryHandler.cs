using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace TheCountBot.Core.Queries
{
    public class IsValidEntryQueryHandler : IRequestHandler<IsValidEntryQuery, bool>
    {
        private readonly PreviousMessageStateTracker _previousMessageStateTracker;

        public IsValidEntryQueryHandler( PreviousMessageStateTracker previousMessageStateTracker )
        {
            _previousMessageStateTracker = previousMessageStateTracker;
        }

        public Task<bool> Handle( IsValidEntryQuery request, CancellationToken cancellationToken )
        {
            if ( NumberStartsWithZero( request.Number ) )
            {
                _previousMessageStateTracker.SetMessageStateToInvalid();
                return Task.FromResult( false );
            }

            if ( !IsProperNumber( request.Number ) )
            {
                _previousMessageStateTracker.SetMessageStateToInvalid();
                return Task.FromResult( false );
            }

            int number = int.Parse( request.Number );

            if ( !_previousMessageStateTracker.IsUserNumberCompositionValid( request.Username, number ) )
            {
                _previousMessageStateTracker.SetMessageStateToInvalid();
                return Task.FromResult( false );
            }

            _previousMessageStateTracker.SetMessageStateToValid( request.Username, number );
            return Task.FromResult( true );
        }

        private bool NumberStartsWithZero( string number )
        {
            return number.StartsWith( "0", StringComparison.InvariantCulture );
        }

        private bool IsProperNumber( string number )
        {
            return int.TryParse( number, out int result );
        }
    }
}
