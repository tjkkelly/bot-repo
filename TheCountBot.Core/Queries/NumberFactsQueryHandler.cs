using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TheCountBot.Core.DataModels;

namespace TheCountBot.Core.Queries
{
    public class NumberFactsQueryHandler : IRequestHandler<NumberFactsQuery, NumberFacts>
    {
        public Task<NumberFacts> Handle( NumberFactsQuery request, CancellationToken cancellationToken )
        {
            return Task.FromResult( new NumberFacts
            {
                IsPowerOf10 = IsPowerOf10( request.Number ),
                IsAllSameDigits = IsAllSameDigits( request.Number ),
                IsPalindrome = IsPalindrome( request.Number )
            } );
        }

        private bool IsAllSameDigits( int x )
        {
            if ( x < 10 ) return false;

            int firstDigit = x % 10;

            while ( x > 0 )
            {
                if ( x % 10 != firstDigit ) return false;
                x /= 10;
            }
            return true;
        }

        private bool IsPalindrome( int x )
        {
            if ( x < 10 ) return false;

            int original = x;
            int reverse = 0;

            while ( x > 0 )
            {
                reverse *= 10;
                reverse += x % 10;
                x /= 10;
            }

            return original == reverse;
        }

        private bool IsPowerOf10( int x )
        {
            return x > 1000 && x % 1000 == 0;
        }
    }
}
