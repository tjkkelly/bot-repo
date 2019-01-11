using System.Collections.Generic;
using System.Linq;
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
            IReadOnlyList<UserStatistics> usersStatistics = await GetAllUsersStatisticAsync();

            return new AllUserStatisticsResponse
            {
                UsersStatistics = usersStatistics
            };
        }

        [HttpGet, Route( "{username}" )]
        public async Task<SingleUserStatisticsResponse> NewUserStatsCommandAsync( [FromRoute] string username )
        {
            IReadOnlyList<UserStatistics> usersStatistics = await GetAllUsersStatisticAsync();

            return new SingleUserStatisticsResponse
            {
                UserStatistics = usersStatistics.FirstOrDefault( us => us.Username == username )
            };
        }

        private async Task<IReadOnlyList<UserStatistics>> GetAllUsersStatisticAsync()
        {
            IReadOnlyList<UserMessage> allUserMessages = await _mediator.Send(new AllMessageHistoryQuery());
            IReadOnlyList<UserStatistics> usersStatistics = await _mediator.Send(new StatsByUserQuery
            {
                UsersMessages = allUserMessages
            });

            return usersStatistics;
        }
    }
}
