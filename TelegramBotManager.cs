using System.Threading.Tasks;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using System.Linq;
using System.Collections.Generic;
using System;
using TheCountBot.Models;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.Options;

namespace TheCountBot
{
    internal class TelegramBotManager : ITelegramBotManager
    {
        private ITelegramBotClient _botClient;

        private int? _lastNumber = null;

        private string _lastUserToSendCorrect = null;

        private Timer _stateTimer;

        private List<string> _insultList;

        private Random _rng = new Random();

        private NumberStoreContext _context;

        private readonly Settings _settings;

        public TelegramBotManager( IOptions<Settings> settingsOptions, ITelegramBotClient telegramBotClient )
        {
            _settings = settingsOptions.Value;

            _botClient = telegramBotClient;

            _botClient.OnMessage += OnMessageReceivedAsync;

            _stateTimer = new Timer(TimerFunc, null, _settings.TimerWaitTime, _settings.TimerWaitTime);

            _insultList = _settings.InsultsForMessingUpTheNumber;

            _context = new NumberStoreContext( _settings.ConnectionString );
        }

        public async Task RunAsync()
        {
            await SendMessageAsync("Welcome me, heathens").ConfigureAwait(false);
            _botClient.StartReceiving();

            Thread.Sleep(Timeout.Infinite);

            _botClient.StopReceiving();
        }

        // internal async Task ShutdownAsync()
        // {
        //     await SendMessageAsync("Goodbye cruel world").ConfigureAwait(false);
        //     _botClient.StopReceiving();
        // }

        private void TimerFunc(object stateInfo)
        {
            SendMessageAsync("I'm lonely...").Wait();
        }

        private async Task SendMessageAsync( string message, ParseMode mode = ParseMode.Default )
        {
            await _botClient.SendTextMessageAsync( _settings.MetaCountingChatId, message, mode );
        }

        private async Task CalculateAndSendMistakesPerPersonAsync( List<NumberStore> list )
        {
            var totalMistakesByUser = new Dictionary<String, int>();
            var totalMessagesByUser = new Dictionary<String, int>();

            foreach( NumberStore record in list ) {
                if ( !totalMistakesByUser.ContainsKey( record.Username ) )
                {
                    totalMistakesByUser[record.Username] = 0;
                    totalMessagesByUser[record.Username] = 0;
                }
                if ( !record.Correct )
                {
                    totalMistakesByUser[record.Username] += 1;
                }
                totalMessagesByUser[record.Username] += 1;
            }

            int countOfTotalMistakes = totalMistakesByUser.Keys.ToList().Aggregate(0, (acc, key) => acc + totalMistakesByUser[key]);

            string messageToSend = String.Format($"```\n{"Username", -20} -- {"Total Messages Sent", -30} -- {"Number Of Mistakes", -30} -- {"Percent Of Total Mistakes", -30}\n");
            totalMistakesByUser.Keys.ToList().ForEach( username => {
                int totalMessagesSent = totalMessagesByUser[username];
                int totalMistakes = totalMistakesByUser[username];
                double percent = ((double) totalMistakes) / countOfTotalMistakes * 100;

                messageToSend += String.Format($"{username, -20} -- {totalMessagesSent, -30} -- {totalMistakes, -30} -- {percent, -30}\n");
            } );
            messageToSend += "```";

            await SendMessageAsync( messageToSend, ParseMode.Markdown );
        }

        private async Task HandleStatsCommandAsync()
        {
            await CalculateAndSendMistakesPerPersonAsync( await _context.GetHistoryAsync() );
        }

        private bool MoreRobustNumberCheck(string x)
        {
            if (x.StartsWith("0")) return false;

            //potentially other checks...

            return true;
        }

        private async void OnMessageReceivedAsync(object sender, MessageEventArgs e)
        {
            System.Console.WriteLine("Message Received");
            if ( e.Message.Chat.Id == _settings.MetaCountingChatId
                    && (e.Message.Text == "/stats" || e.Message.Text == "/stats@the_cnt_bot") )
            {
                await HandleStatsCommandAsync();
                return;
            }

            if (e.Message.Chat.Id == _settings.CountingChatId)
            {
                NumberStore record = new NumberStore 
                {
                    Username = e.Message.From.Username == null ? e.Message.From.FirstName : e.Message.From.Username,
                    Timestamp = DateTime.UtcNow.ToString()
                };

                bool isNumberValue = int.TryParse(e.Message.Text, out int number);
                isNumberValue &= MoreRobustNumberCheck(e.Message.Text);

                if ( !isNumberValue
                        || ( _lastUserToSendCorrect != null && ( _lastUserToSendCorrect == e.Message.From.Username ))
                        || ((_lastNumber != null) && number != _lastNumber + 1 ) )
                {
                    _lastUserToSendCorrect = null;
                    _lastNumber = null;

                    record.Correct = false;
                    record.Number = -1;

                    await SendMessageAsync( GetRandomInsultMessageForUser( e.Message.From.Username ) );

                }
                else
                {
                    _lastNumber = number;
                    _lastUserToSendCorrect = e.Message.From.Username;

                    record.Correct = true;
                    record.Number = number;

                    await HandleCoolNumbersAsync( number, e.Message.From.Username );
                }

                _stateTimer.Change(_settings.TimerWaitTime, _settings.TimerWaitTime);
                await _context.AddRecordAsync( record );
            }
        }

        private bool IsSameDigits(int x)
        {
            //not counting numbers less than 10
            if ( x < 10 ) return false;
            int firstDigit=x%10;
            while ( x > 0 ){
                if ( x % 10 != firstDigit ) return false;
                x/=10;
            }
            return true;
        }

        private bool IsPalindrome(int x)
        {
            //not counting numbers less than 10
            if ( x < 10 ) return false;

            int original=x, reverse=0;

            while ( x > 0 )
            {
                reverse*=10;
                reverse+=x%10;
                x/=10;
            }

            return original == reverse;
        }

        private bool Is1000(int x)
        {
            return x > 1000 && x % 1000 == 0;
        }

        private async Task HandleCoolNumbersAsync(int x, string user )
        {
            if (IsSameDigits(x))
                await SendMessageAsync($"YO {user}, {x} is made up of all {x%10}s!" );
            else if (IsPalindrome(x))
                await SendMessageAsync($"Hey, {user}! {x} is a palindrome!" );
            else if (Is1000(x))
                await SendMessageAsync($"AYYYYYY {user}" );
        }

        private string GetRandomInsultMessageForUser( string user )
        {
            int _randInt = _rng.Next( 0, _insultList.Count );
            string message = _insultList[_randInt].Replace("{username}", user);

            return message;
        }
    }
}
