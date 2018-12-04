using System.Threading.Tasks;

namespace TheCountBot.Core.Interfaces
{
    public interface ITelegramBotClient
    {
        Task SendRawMessageAsync( string message );
        Task SendMonospacedMessageAsync( string message );
    }
}
