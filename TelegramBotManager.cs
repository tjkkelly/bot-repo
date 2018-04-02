using System.Threading.Tasks;
using System.Threading;
using Telegram.Bot;
using TheCountBot.Configuration;
using Telegram.Bot.Args;
using System.Linq;
using System.Collections.Generic;
using System;


namespace TheCountBot
{
    internal class TelegramBotManager
    {
        private ITelegramBotClient _botClient;

        private int? lastNumber = null;

        private Timer _stateTimer;

        private List<string> _insultList;

        private Random _rng = new Random();

        internal TelegramBotManager()
        {
            _botClient = new TelegramBotClient( Settings.BotIdSecret );

            _botClient.OnMessage += OnMessageReceivedAsync;

            _stateTimer = new Timer(TimerFunc, null, Settings.TimerWaitTime, Settings.TimerWaitTime);

            _insultList = Settings.InsultsForMessingUpTheNumber;
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
                            await SendMessageAsync( GetRandomInsultMessageForUser( e.Message.From.Username ) ).ConfigureAwait(false);
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
                    await SendMessageAsync( GetRandomInsultMessageForUser( e.Message.From.Username ) ).ConfigureAwait(false);
                    lastNumber = null;
                }

                _stateTimer.Change(Settings.TimerWaitTime, Settings.TimerWaitTime);
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