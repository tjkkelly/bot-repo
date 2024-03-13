using System;
using System.Threading.Tasks;
using TheCountBot.Application.Models;

namespace TheCountBot
{
    public interface IStatsManager
    {
        Task HandleStatsCommandAsync(BotCommand command, string user, IServiceProvider serviceProvider);
    }
}