using System.Threading.Tasks;
using Telegram.Bot;
using TheCountBot.Configuration;
using Telegram.Bot.Args;

namespace TheCountBot
{
    internal class TelegramBotManager
    {
        private ITelegramBotClient _botClient;

        private int? lastNumber = null;

        internal TelegramBotManager()
        {
            _botClient = new TelegramBotClient( Settings.BotIdSecret );

            _botClient.OnMessage += OnMessageReceivedAsync;
            //_botClient.OnMessageEdited += OnMessageEditedRecievedAysnc;
        }

        internal async Task StartupAsync()
        {
            await SendMessageAsync("Welcome me, heathens").ConfigureAwait(false);
            _botClient.StartReceiving();
        }

        internal async Task ShutdownAsync()
        {
            await SendMessageAsync("Goodbye cruel world").ConfigureAwait(false);
            _botClient.StopReceiving();
        }

        private async Task SendMessageAsync( string message )
        {
            await _botClient.SendTextMessageAsync( Settings.MetaCountingChatId, message ).ConfigureAwait( false );
        }

        private async void OnMessageReceivedAsync(object sender, MessageEventArgs e)
        {
            if (e.Message.Chat.Id == Settings.CountingChatId)
            {
                if (int.TryParse(e.Message.Text, out int number))
                {                
                    if (lastNumber != null)
                    {
                        if (number == lastNumber + 1)
                        {
                            lastNumber = number;
                        }
                        else
                        {
                            await SendMessageAsync($"@{e.Message.From.Username}, wrong fucking number, fix it...or else").ConfigureAwait(false);
                            lastNumber = null;
                        }
                    }
                    else
                    {
                        lastNumber = number;   
                    }
                }
                else
                {
                    await SendMessageAsync($"@{e.Message.From.Username}, holy shit, that's not even a number!").ConfigureAwait(false);
                    lastNumber = null;
                }
            }
        }
        
        // private async void OnMessageEditedRecievedAysnc(object sender, MessageEventArgs e)
        // {
        //     //await SendMessageAsync( $"{e.Message.Text} was edited" ).ConfigureAwait( false );
        // }
    }
}