using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TheCountBot.Core.DataModels;
using System.Linq;

namespace TheCountBot.Core.Queries
{
    public class StatsByUserQueryHandler : IRequestHandler<StatsByUserQuery, IReadOnlyList<UserStatistics>>
    {
        public Task<IReadOnlyList<UserStatistics>> Handle( StatsByUserQuery request, CancellationToken cancellationToken )
        {
            int totalNumberOfMessagesSent = GetTotalNumberOfMessagesSent(request.UsersMessages);
            int totalNumberOfErrors = GetTotalErrorsSent( request.UsersMessages );

            List<UserStatistics> usersStatistics = InitializeStatisitcsStub( request.UsersMessages );
            usersStatistics = GetMistakesByUser( request.UsersMessages, usersStatistics );
            usersStatistics = GetMessagesByUser( request.UsersMessages, usersStatistics );
            usersStatistics = GetPercentOfTotalMessagesByUser( usersStatistics, totalNumberOfMessagesSent );
            usersStatistics = GetErrorRateByUser( usersStatistics, totalNumberOfMessagesSent );
            usersStatistics = GetRelativeMistakeRatesByUser( usersStatistics );
            usersStatistics = GetTotalErrorRateByUser( usersStatistics, totalNumberOfErrors );

            return Task.FromResult( (IReadOnlyList<UserStatistics>) usersStatistics );
        }

        private List<UserStatistics> GetMistakesByUser( IReadOnlyList<UserMessage> usersMessages, List<UserStatistics> usersStatistics )
        {
            foreach ( UserMessage record in usersMessages )
            {
                if ( !record.IsCorrect )
                {
                    usersStatistics.FirstOrDefault( us => us.Username == record.Username ).MistakeCount++;
                }
            }

            return usersStatistics;
        }

        private List<UserStatistics> GetMessagesByUser( IReadOnlyList<UserMessage> usersMessages, List<UserStatistics> usersStatistics )
        {
            foreach ( UserMessage record in usersMessages )
            {
                usersStatistics.FirstOrDefault( us => us.Username == record.Username ).MessagesSentCount++;
            }

            return usersStatistics;
        }

        private List<UserStatistics> GetPercentOfTotalMessagesByUser( List<UserStatistics> usersStatistics, int totalMessagesSent )
        {
            foreach ( UserStatistics userStatistics in usersStatistics )
            {
                userStatistics.TotalMessagePercentage = userStatistics.MessagesSentCount / totalMessagesSent;
            }

            return usersStatistics;
        }

        private List<UserStatistics> GetErrorRateByUser( List<UserStatistics> usersStatistics, int totalMessageCount )
        {
            foreach ( UserStatistics userStatistics in usersStatistics )
            {
                userStatistics.ErrorRate = userStatistics.MistakeCount / totalMessageCount;
            }

            return usersStatistics;
        }

        private List<UserStatistics> GetTotalErrorRateByUser( List<UserStatistics> usersStatistics, int totalCountOfErrors )
        {
            foreach (UserStatistics userStatistics in usersStatistics)
            {
                userStatistics.TotalErrorRate = userStatistics.MistakeCount / totalCountOfErrors;
            }

            return usersStatistics;
        }

        private List<UserStatistics> GetRelativeMistakeRatesByUser( List<UserStatistics> usersStatistics )
        {
            double minimumError = GetPositiveMinimumNonZeroErrorRate( usersStatistics );

            foreach ( UserStatistics userStatistics in usersStatistics )
            {
                userStatistics.RelativeErrorRate = userStatistics.ErrorRate / minimumError;
            }

            return usersStatistics;
        }

        private double GetPositiveMinimumNonZeroErrorRate( List<UserStatistics> usersStatistics )
        {
            double minimumErrorPercentage = 101;

            foreach ( UserStatistics userStatistics in usersStatistics )
            {
                if ( userStatistics.ErrorRate < minimumErrorPercentage && userStatistics.ErrorRate > 0.0001 )
                {
                    minimumErrorPercentage = userStatistics.ErrorRate;
                }
            }

            return minimumErrorPercentage;
        }

        private int GetTotalNumberOfMessagesSent( IReadOnlyList<UserMessage> usersMessages )
        {
            return usersMessages.Count;
        }

        private int GetTotalErrorsSent( IReadOnlyList<UserMessage> usersMessages )
        {
            return usersMessages.Count( um => um.IsCorrect == false );
        }

        private List<UserStatistics> InitializeStatisitcsStub( IReadOnlyList<UserMessage> usersMessages )
        {
            return usersMessages
                    .Select(um => um.Username)
                    .Distinct()
                    .Select(u => new UserStatistics
                    {
                        Username = u,
                        MistakeCount = 0,
                        MessagesSentCount = 0
                    })
                    .ToList();
        }
    }
}
