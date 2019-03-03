using System;
namespace TheCountBot.Application.Clients.Models
{
    public static class UserStatisticsExtensions
    {
        public static TheCountBot.Core.DataModels.UserStatistics ToCoreUserStatitics( this UserStatistics userStatistics )
        {
            return new Core.DataModels.UserStatistics
            {
                Username = userStatistics.Username,
                MessagesSentCount = userStatistics.MessagesSentCount ?? 0,
                TotalMessagePercentage = userStatistics.TotalMessagePercentage ?? 0,
                TotalErrorRate = userStatistics.TotalErrorRate ?? 0,
                ErrorRate = userStatistics.ErrorRate ?? 0,
                RelativeErrorRate = userStatistics.RelativeErrorRate ?? 0,
                MistakeCount = userStatistics.MistakeCount ?? 0
            };
        }
    }
}
