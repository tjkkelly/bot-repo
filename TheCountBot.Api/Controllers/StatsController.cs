using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TheCountBot.Api.ApiResponses;
using TheCountBot.Core.DataModels;
using TheCountBot.Core.Queries;

namespace TheCountBot.Api.Controllers
{
    [Route( "api/[controller]" )]
    public class StatsController : Controller
    {
        private readonly IMediator _mediator;

        public StatsController( IMediator mediator )
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<AllUserStatisticsResponse> NewAllStatsCommandAsync()
        {
            IReadOnlyList<UserMessage> allUserMessages = await _mediator.Send( new AllMessageHistoryQuery() );
            IReadOnlyList<UserStatistics> usersStatistics = await _mediator.Send( new StatsByUserQuery
            {
                UsersMessages = allUserMessages
            });

            return new AllUserStatisticsResponse
            {
                UsersStatistics = usersStatistics
            };
        }

        [HttpGet, Route( "{username}" )]
        public Task<bool> NewUserStatsCommandAsync( [FromRoute] string username )
        {
            throw new NotImplementedException();
        }
    }
}
