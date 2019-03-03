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
using MediatR;
using TheCountBot.Core.Commands;
using TheCountBot.Application.Clients;
using TheCountBot.Application.Clients.Models;

namespace TheCountBot
{
    internal class TelegramBotManager : ITelegramBotManager
    {
        private ITelegramBotClient _botClient;
        private readonly IMediator _mediator;
        private Timer _stateTimer;

        private List<string> _insultList;

        private Random _rng = new Random();

        private readonly Settings _settings;

        private readonly ICountBotApi _countBotApi;

        public TelegramBotManager( IOptions<Settings> settingsOptions, ITelegramBotClient telegramBotClient, IMediator mediator, ICountBotApi countBotApi )
        {
            _countBotApi = countBotApi;
            _settings = settingsOptions.Value;
            _botClient = telegramBotClient;
            _mediator = mediator;
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
            List<TheCountBot.Core.DataModels.UserStatistics> userStatistics;

            switch( command.commandType )
            {
                case BotCommandEnum.fullStats:
                    AllUserStatisticsResponse allUserStatisticsResponse = await _countBotApi.NewAllStatsCommandAsyncAsync();
                    userStatistics = allUserStatisticsResponse.UsersStatistics.Select( us => us.ToCoreUserStatitics() ).ToList();
                    break;
                case BotCommandEnum.individualStats:
                    SingleUserStatisticsResponse singleUserStatistics = await _countBotApi.NewUserStatsCommandAsyncAsync( user );
                    userStatistics = new List<Core.DataModels.UserStatistics> { singleUserStatistics.UserStatistics.ToCoreUserStatitics() };
                    break;
                default:
                    return;
            }

            await _mediator.Send( new SendFormattedStatsByUserCommand
            {
                UsersStatistics = userStatistics
            });
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
                NewMessageResponse newMessageResponse = await _countBotApi.NewCountingMessageAsync( new NewMessageRequest
                {
                    Username = e.Message.From.Username,
                    Number = e.Message.Text,
                    Timestamp = e.Message.Date
                });

                if ( !newMessageResponse.IsCorrect ?? false )
                {
                    await _mediator.Send( new SendIncorrectNumberCommand
                    {
                        Username = e.Message.From.Username
                    });
                }
                else
                { 
                    await HandleCoolNumbersAsync( newMessageResponse );   
                }
            }
        }

        private async Task HandleCoolNumbersAsync( NewMessageResponse newMessageResponse )
        {
            if ( newMessageResponse.IsAllSameDigits ?? false )
            {
                await SendMessageAsync( $"YO @{newMessageResponse.UserName}, {newMessageResponse.Number} is made up of all {newMessageResponse.Number % 10}s!" );
            }
            else if ( newMessageResponse.IsPalindrome ?? false )
            {
                await SendMessageAsync( $"Hey, @{newMessageResponse.UserName}! {newMessageResponse.Number} is a palindrome!" );
            }
            else if ( newMessageResponse.IsPowerOf10 ?? false )
            {
                await SendMessageAsync( $"AYYYYYY @{newMessageResponse.UserName}" );
            }
        }
    }
}
