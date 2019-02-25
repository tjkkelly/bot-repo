using System.Threading.Tasks;
using System.Threading;
using Telegram.Bot.Args;
using Telegram.Bot;
using System.Linq;
using System.Collections.Generic;
using System;
using TheCountBot.Data.Models;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.Options;
using TheCountBot.Application.Models;
using TheCountBot.Application.Models.Enums;
using TheCountBot.Core.Commands;

namespace TheCountBot
{
    internal class TelegramBotManager : ITelegramBotManager
    {
        private ITelegramBotClient _botClient;

        private Timer _stateTimer;

        private List<string> _insultList;

        private Random _rng = new Random();

        private readonly Settings _settings;

        public TelegramBotManager( IOptions<Settings> settingsOptions, ITelegramBotClient telegramBotClient )
        {
            _settings = settingsOptions.Value;
            _botClient = telegramBotClient;
            _botClient.OnMessage += OnMessageReceivedAsync;
            _stateTimer = new Timer( TimerFunc, null, _settings.TimerWaitTime, _settings.TimerWaitTime );
            _insultList = _settings.InsultsForMessingUpTheNumber;
        }

        public async Task RunAsync()
        {
            await SendMessageAsync( "Welcome me, heathens" );
            _botClient.StartReceiving();

            Thread.Sleep( Timeout.Infinite );

            _botClient.StopReceiving();
        }

        private void TimerFunc( object stateInfo )
        {
            SendMessageAsync( "I'm lonely..." ).Wait();
        }

        private async Task SendMessageAsync( string message, ParseMode mode = ParseMode.Default )
        {
            await _botClient.SendTextMessageAsync( _settings.MetaCountingChatId, message, mode );
        }
       

        private async Task HandleStatsCommandAsync( BotCommand command, string user )
        {
            switch( command.commandType )
            {
                case BotCommandEnum.fullStats:

                    break;
                case BotCommandEnum.individualStats:

                    break;
            }
        }

        private async void OnMessageReceivedAsync( object sender, MessageEventArgs e )
        {
            Console.WriteLine( "Message Received" );
            if ( e.Message.Chat.Id == _settings.MetaCountingChatId )
            {
                BotCommand command = new BotCommand( e.Message.Text );
                if ( command.commandType != BotCommandEnum.noCommand )
                {
                    await HandleStatsCommandAsync( command, e.Message.From.Username );
                    return;
                }
            }

            if ( e.Message.Chat.Id == _settings.CountingChatId )
            {
            }
        }

        //private async Task HandleCoolNumbersAsync( int x, string user )
        //{
        //    if ( IsSameDigits( x ) )
        //    {
        //        await SendMessageAsync( $"YO @{user}, {x} is made up of all {x % 10}s!" );
        //    }
        //    else if ( IsPalindrome( x ) )
        //    {
        //        await SendMessageAsync( $"Hey, @{user}! {x} is a palindrome!" );
        //    }
        //    else if ( Is1000( x ) )
        //    {
        //        await SendMessageAsync( $"AYYYYYY @{user}" );
        //    }
        //}
    }
}
