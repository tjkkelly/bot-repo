using System.Threading.Tasks;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using System.Linq;
using System.Collections.Generic;
using System;
using TheCountBot.Data.Models;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.Options;
using TheCountBot.Application.Models;
using TheCountBot.Application.Models.Enums;
using TheCountBot.Data.Repositories;

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

        private readonly Settings _settings;

        private IServiceProvider _serviceProvider;

        private readonly INumberStoreRepository _numberStoreRepository;

        private readonly IStatsManager _statsManager;

        public TelegramBotManager( IOptions<Settings> settingsOptions, ITelegramBotClient telegramBotClient, INumberStoreRepository numberStoreRepository, IStatsManager statsManager )
        {
            _settings = settingsOptions.Value;
            _botClient = telegramBotClient;
            _botClient.OnMessage += OnMessageReceivedAsync;
            _stateTimer = new Timer( TimerFunc, null, _settings.TimerWaitTime, _settings.TimerWaitTime );
            _insultList = _settings.InsultsForMessingUpTheNumber;
            _numberStoreRepository = numberStoreRepository;
            _statsManager = statsManager;
        }

        public async Task RunAsync( IServiceProvider serviceProvider )
        {
            _serviceProvider = serviceProvider;

            await SendMessageAsync( "Welcome me, heathens" );
            _botClient.StartReceiving();

            Thread.Sleep( Timeout.Infinite );

            _botClient.StopReceiving();
        }

        private void TimerFunc(object stateInfo)
        {
            SendMessageAsync( "I'm lonely..." ).Wait();
        }

        private async Task SendMessageAsync( string message, ParseMode mode = ParseMode.Default )
        {
            await _botClient.SendTextMessageAsync( _settings.MetaCountingChatId, message, mode );
        }

        private bool MoreRobustNumberCheck( string x )
        {
            if ( x.StartsWith( "0" ) ) return false;

            //potentially other checks...

            return true;
        }

        private async void OnMessageReceivedAsync( object sender, MessageEventArgs e )
        {
            System.Console.WriteLine("Message Received");
            if ( e.Message.Chat.Id == _settings.MetaCountingChatId )
            {
                BotCommand command = new BotCommand(e.Message.Text);
                if (command.commandType != BotCommandEnum.noCommand)
                {
                    await _statsManager.HandleStatsCommandAsync( command, e.Message.From.Username, _serviceProvider );
                    return;
                }
            }

            if (e.Message.Chat.Id == _settings.CountingChatId)
            {
                MessageEntry messageEntry = new MessageEntry
                {
                    Username = e.Message.From.Username == null ? e.Message.From.FirstName : e.Message.From.Username,
                    Timestamp = DateTime.UtcNow
                };

                bool isNumberValue = int.TryParse( e.Message.Text, out int number );
                if( e.Message.Text != null )
                {
                    isNumberValue &= MoreRobustNumberCheck( e.Message.Text );
                }

                if ( !isNumberValue
                        || ( _lastUserToSendCorrect != null && ( _lastUserToSendCorrect == e.Message.From.Username ) )
                        || ( ( _lastNumber != null) && number != _lastNumber + 1 ) )
                {
                    _lastUserToSendCorrect = null;
                    _lastNumber = null;

                    messageEntry.Correct = false;
                    messageEntry.Number = -1;

                    await SendMessageAsync( GetRandomInsultMessageForUser( e.Message.From.Username ) );
                }
                else
                {
                    _lastNumber = number;
                    _lastUserToSendCorrect = e.Message.From.Username;

                    messageEntry.Correct = true;
                    messageEntry.Number = number;

                    await HandleCoolNumbersAsync( number, e.Message.From.Username );
                }

                _stateTimer.Change( _settings.TimerWaitTime, _settings.TimerWaitTime );
                await _numberStoreRepository.AddNewMessageEntryAsync( _serviceProvider, messageEntry );
            }
        }

        private bool IsSameDigits( int x )
        {
            //not counting numbers less than 10
            if ( x < 10 ) return false;
            int firstDigit=x%10;

            while ( x > 0 ){
                if ( x % 10 != firstDigit ) return false;
                x /= 10;
            }
            return true;
        }

        private bool IsPalindromeSetup( int x )
        {
            return IsPalindrome( x + 1 );

        }

        private bool IsPalindrome( int x )
        {
            //not counting numbers less than 10
            if ( x < 10 ) return false;

            int original=x, reverse=0;

            while ( x > 0 )
            {
                reverse*=10;
                reverse+=x%10;
                x /= 10;
            }

            return original == reverse;
        }
        
        private bool IsACoolThing( int x )
        {
            /*
            Stuff like
            23233 (2 2s and 3 3s)
            333 (3 3's)
            2214444 (you get the point)
            */
            Dictionary<int, int> stuff = new Dictionary<int, int>();
            while ( x > 0 )
            {
                lastDigit=x%10;
                // python has defaultdicts wouldn't that be nice to have here
                if ( ! stuff.ContainsKey(lastDigit) ) {
                    stuff[lastDigit]=0;
                }
                stuff[lastDigit]+=1;
                x /= 10;
            }
            
            foreach (KeyValuePair<int, int> item in stuff) {
                if (item.Key != item.Value) {
                    return False;
                }
            }
            return True;
        }

        private bool Is1000( int x )
        {
            return x > 1000 && x % 1000 == 0;
        }

        private bool IsNice( int x )
        {
            return x % 100 == 69;
        }

        private bool IsEvil( int x )
        {
            return x % 1000 == 666;
        }

        private bool IsDank( int x )
        {
            return x % 1000 == 420;
        }

        private async Task HandleCoolNumbersAsync( int x, string user )
        {
            if ( IsSameDigits( x ) )
            {
                await SendMessageAsync( $"YO @{user}, {x} is made up of all {x % 10}s!" );
            }
            else if ( IsPalindrome( x ) )
            {
                await SendMessageAsync( $"Hey, @{user}! {x} is a palindrome!" );
            }
            else if ( Is1000( x ) )
            {
                await SendMessageAsync( $"AYYYYYY @{user}" );
            }
            else if ( IsPalindromeSetup( x ) )
            {
                await SendMessageAsync( $"Yo @{user} thanks for setting us up for a palindrome!" );
            }
            else if ( IsEvil( x ) )
            {
                await SendMessageAsync( $"@{user} you devil, you!" );
            }
            else if ( IsNice( x ) )
            {
                await SendMessageAsync( $"@{user} Nice." );
            }
            else if ( IsDank(x) )
            {
                await SendMessageAsync( $"@{user} blaze it!" );
            }
            else if ( IsACoolThing(x) )
            {
                await SendMessageAsync( $"@{user} this number sorta describes itself" );
            }
        }
        private string GetRandomInsultMessageForUser( string user )
        {
            int _randInt = _rng.Next( 0, _insultList.Count );
            string message = _insultList[_randInt].Replace( "{username}", user );

            return message;
        }
    }
}
