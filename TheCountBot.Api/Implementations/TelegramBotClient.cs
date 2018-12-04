using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TheCountBot.Core.Interfaces;
using ITelegramBotClientExternal = Telegram.Bot.ITelegramBotClient;

namespace TheCountBot.Api.Implementations
{
    public class TelegramBotClient : ITelegramBotClient
    {
        private readonly ITelegramBotClientExternal _telegramBotClient;
        private readonly Settings _settings;

        public TelegramBotClient( ITelegramBotClientExternal telegramBotClient, IOptions<Settings> settingsOptions )
        {
            _telegramBotClient = telegramBotClient;
            _settings = settingsOptions.Value;
        }

        public async Task SendRawMessageAsync( string message )
        {
            await _telegramBotClient.SendTextMessageAsync( _settings.MetaCountingChatId, message, Telegram.Bot.Types.Enums.ParseMode.Default );
        }

        public async Task SendMonospacedMessageAsync( string message )
        {
            await _telegramBotClient.SendTextMessageAsync( _settings.MetaCountingChatId, message, Telegram.Bot.Types.Enums.ParseMode.Markdown );
        }
    }
}
