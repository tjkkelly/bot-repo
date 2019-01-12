using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheCountBot.Api.Requests;
using MediatR;
using TheCountBot.Core.Queries;
using TheCountBot.Core.Commands;
using TheCountBot.Api.ApiResponses;
using TheCountBot.Core.DataModels;

namespace TheCountBot.Api.Controllers
{
    [Route( "api/[controller]" )]
    public class MessageController : Controller
    {
        private readonly IMediator _mediator;

        public MessageController( IMediator mediator )
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<NewMessageResponse> NewCountingMessage( [FromBody] NewMessageRequest requestBody )
        {
            bool isCorrect = await _mediator.Send( new IsValidEntryQuery
            {
                Number = requestBody.Number,
                Username = requestBody.Username
            });

            int number = 0;
            NumberFacts numberFacts = new NumberFacts();

            if ( isCorrect )
            {
                number = int.Parse( requestBody.Number );

                numberFacts = await _mediator.Send( new NumberFactsQuery { Number = number } );
            }

            await _mediator.Send( new AddNewEntryCommand
            {
                Username = requestBody.Username,
                Number = number,
                IsCorrect = isCorrect,
                Timestamp = requestBody.Timestamp
            });

            return new NewMessageResponse 
            { 
                UserName = requestBody.Username,
                IsCorrect = isCorrect,
                IsPowerOf10 = numberFacts.IsPowerOf10,
                IsPalindrome = numberFacts.IsPalindrome,
                IsAllSameDigits = numberFacts.IsAllSameDigits
            };
        }
    }
}
