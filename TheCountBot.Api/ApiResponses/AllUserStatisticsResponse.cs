using System.Collections.Generic;
using TheCountBot.Core.DataModels;

namespace TheCountBot.Api.ApiResponses
{
    public class AllUserStatisticsResponse
    {
        public IReadOnlyList<UserStatistics> UsersStatistics { get; set; }
    }
}
