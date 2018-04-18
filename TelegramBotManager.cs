using System.Threading.Tasks;
using System.Threading;
using Telegram.Bot;
using TheCountBot.Configuration;
using Telegram.Bot.Args;
using System.Linq;
using System.Collections.Generic;
using System;
using TheCountBot.Models;


namespace TheCountBot
{
    internal class TelegramBotManager
    {
        private ITelegramBotClient _botClient;

        private int? _lastNumber = null;

        private string _lastUserToSendCorrect = null;

        private Timer _stateTimer;

        private List<string> _insultList;

        private Random _rng = new Random();

        private NumberStoreContext _context;

        internal TelegramBotManager()
        {
            _botClient = new TelegramBotClient( Settings.BotIdSecret );

            _botClient.OnMessage += OnMessageReceivedAsync;

            _stateTimer = new Timer(TimerFunc, null, Settings.TimerWaitTime, Settings.TimerWaitTime);

            _insultList = Settings.InsultsForMessingUpTheNumber;

            _context = new NumberStoreContext( Settings.ConnectionString );
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

        public void TimerFunc(object stateInfo)
        {
            SendMessageAsync("I'm lonely...").Wait();
        }

        private async Task SendMessageAsync( string message )
        {
            await _botClient.SendTextMessageAsync( Settings.MetaCountingChatId, message ).ConfigureAwait( false );
        }

        private async void OnMessageReceivedAsync(object sender, MessageEventArgs e)
        {
            System.Console.WriteLine("Message Received");
            if ( e.Message.Text == "/stats" )
            {
                await SendMessageAsync( await _context.GetHistoryAsync().ConfigureAwait( false ) ).ConfigureAwait( false );
                return;
            } 

            if (e.Message.Chat.Id == Settings.CountingChatId)
            {
                NumberStore record = new NumberStore 
                {
                    Username = e.Message.From.Username,
                    Timestamp = DateTime.UtcNow.ToString()
                };

                bool isNumberValue = int.TryParse(e.Message.Text, out int number);

                if ( !isNumberValue || ( _lastUserToSendCorrect != null && ( _lastUserToSendCorrect == e.Message.From.Username )) || ((_lastNumber != null) && number != _lastNumber + 1 ) )
                {
                    record.Correct = false;
                    record.Number = -1;
                    
                    await SendMessageAsync( GetRandomInsultMessageForUser( e.Message.From.Username ) ).ConfigureAwait( false );

                    _lastUserToSendCorrect = null;
                    _lastNumber = null;
                }
                else
                {
                    record.Correct = true;
                    record.Number = number;

                    _lastNumber = number;
                    _lastUserToSendCorrect = e.Message.From.Username;
                }

                _stateTimer.Change(Settings.TimerWaitTime, Settings.TimerWaitTime);
                await _context.AddRecordAsync( record ).ConfigureAwait( false );
            }
        }

        private string GetRandomInsultMessageForUser( string user )
        {
            int _randInt = _rng.Next( 0, _insultList.Count );
            string message = _insultList[_randInt].Replace("{username}", user);

            return message;
        }
    }
}