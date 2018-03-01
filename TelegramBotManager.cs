using System.Threading.Tasks;
using Telegram.Bot;
using TheCountBot.Configuration;
using Telegram.Bot.Args;

namespace TheCountBot
{
    internal class TelegramBotManager
    {
        ITelegramBotClient _botClient;

        internal TelegramBotManager()
        {
            _botClient = new TelegramBotClient( Settings.BotIdSecret );

            _botClient.OnMessage += OnMessageReceivedAsync;
            _botClient.OnMessageEdited += OnMessageEditedRecievedAysnc;
        }

        internal void Startup()
        {
            _botClient.StartReceiving();
        }

        internal void Shutdown()
        {
            _botClient.StopReceiving();
        }

        private async Task SendMessageAsync( string message )
        {
            await _botClient.SendTextMessageAsync( Settings.CountingChatId, message ).ConfigureAwait( false );
        }

        private async void OnMessageReceivedAsync(object sender, MessageEventArgs e)
        {
            await SendMessageAsync( $"{e.Message.Text}" ).ConfigureAwait( false );
        }
        
        private async void OnMessageEditedRecievedAysnc(object sender, MessageEventArgs e)
        {
            await SendMessageAsync( $"{e.Message.Text} was edited" ).ConfigureAwait( false );
        }
    }
}