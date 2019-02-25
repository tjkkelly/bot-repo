using System.Threading.Tasks;
using System;

namespace TheCountBot
{
    internal interface ITelegramBotManager
    {
        Task RunAsync();
    }
}
