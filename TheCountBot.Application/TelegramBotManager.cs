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

        public TelegramBotManager( IOptions<Settings> settingsOptions, ITelegramBotClient telegramBotClient, INumberStoreRepository numberStoreRepository )
        {
            _settings = settingsOptions.Value;
            _botClient = telegramBotClient;
            _botClient.OnMessage += OnMessageReceivedAsync;
            _stateTimer = new Timer(TimerFunc, null, _settings.TimerWaitTime, _settings.TimerWaitTime);
            _insultList = _settings.InsultsForMessingUpTheNumber;
            _numberStoreRepository = numberStoreRepository;
        }

        public async Task RunAsync( IServiceProvider serviceProvider )
        {
            _serviceProvider = serviceProvider;

            await SendMessageAsync( "Welcome me, heathens" );
            _botClient.StartReceiving();

            Thread.Sleep(Timeout.Infinite);

            _botClient.StopReceiving();
        }

        private void TimerFunc(object stateInfo)
        {
            SendMessageAsync("I'm lonely...").Wait();
        }

        private async Task SendMessageAsync( string message, ParseMode mode = ParseMode.Default )
        {
            await _botClient.SendTextMessageAsync( _settings.MetaCountingChatId, message, mode );
        }

        private async Task CalculateAndSendLimitedStatsPerPersonAsync( List<MessageEntry> list )
        {
            var totalMistakesByUser = getMistakesByUser(list);
            var totalMessagesByUser = getMessagesByUser(list);

            string messageToSend = String.Format($"```\n{"Username", -20} -- {"Total Messages Sent", -20} -- {"Number Of Mistakes", -20} -- {"Error Rate", -20}\n");
            totalMistakesByUser.Keys.ToList().ForEach( username => {
                int totalMessagesSent = totalMessagesByUser[username];
                int totalMistakes = totalMistakesByUser[username];
                double errorRate = ((double) totalMistakes) / totalMessagesSent * 100;

                messageToSend += String.Format($"{username, -20} -- {totalMessagesSent, -20} -- {totalMistakes, -20} -- {errorRate,-20:0.##}\n");
            } );
            messageToSend += "```";

            await SendMessageAsync( messageToSend, ParseMode.Markdown );
        }

        private async Task CalculateAndSendFullStatsPerPersonAsync( List<MessageEntry> list )
        {
            var totalMistakesByUser = getMistakesByUser(list);
            var totalMessagesByUser = getMessagesByUser(list);
            var mistakeRatesByUser = new Dictionary<String, double>();

            int countOfTotalMistakes = totalMistakesByUser.Keys.ToList().Aggregate(0, (acc, key) => acc + totalMistakesByUser[key]);
            int countOfTotalMessages = totalMessagesByUser.Keys.ToList().Aggregate(0, (acc, key) => acc + totalMessagesByUser[key]);

            string messageToSend = String.Format($"```\n{"Username",-20} -- {"Total Messages Sent",-20} -- {"Number Of Mistakes",-20} -- {"Error Rate",-20}\n");
            totalMistakesByUser.Keys.ToList().ForEach(username => {
                int totalMessagesSent = totalMessagesByUser[username];
                int totalMistakes = totalMistakesByUser[username];
                double errorRate = ((double)totalMistakes) / totalMessagesSent * 100;
                mistakeRatesByUser.Add(username, errorRate);

                messageToSend += String.Format($"{username,-20} -- {totalMessagesSent,-20} -- {totalMistakes,-20} -- {errorRate,-20:0.##}\n");
            });
            messageToSend += "\n";
            var relativeErrorRates = calculateRelativeMistakeRatesByUser(mistakeRatesByUser);
            messageToSend += String.Format($"\n{"Username",-20} -- {"Total Message Percentage",-30} -- {"Total Error Rate",-20} -- {"Relative Error Rate",-20}\n");
            totalMistakesByUser.Keys.ToList().ForEach(username => {
                int totalMessagesSent = totalMessagesByUser[username];
                int totalMistakes = totalMistakesByUser[username];
                double totalMessagePercentage = ((double)totalMessagesSent) / countOfTotalMessages * 100;
                double totalErrorRate = ((double)totalMistakes) / countOfTotalMistakes * 100;
                double relativeError = relativeErrorRates[username];

                messageToSend += String.Format($"{username,-20} -- {totalMessagePercentage,-30:0.##} -- {totalErrorRate,-20:0.##} -- {relativeError,-20:0.##}\n");
            });
            messageToSend += "```";

            await SendMessageAsync(messageToSend, ParseMode.Markdown);
        }

        private async Task CalculateAndSendIndividualStats( List<MessageEntry> list, String username )
        {
            var totalMistakesByUser = getMistakesByUser(list);
            var totalMessagesByUser = getMessagesByUser(list);

            String messageToSend = "";
            if(!totalMessagesByUser.ContainsKey(username))
            {
                messageToSend = "You haven't counted yet!";
            }
            else
            {
                messageToSend = BuildIndividualStatsMessage(totalMistakesByUser, totalMessagesByUser, username);
            }

            await SendMessageAsync(messageToSend, ParseMode.Markdown);
        }

        private String BuildIndividualStatsMessage( Dictionary<String, int> totalMistakesByUser, Dictionary<String, int> totalMessagesByUser, String username )
        {
            int countOfTotalMistakes = totalMistakesByUser.Keys.ToList().Aggregate(0, (acc, key) => acc + totalMistakesByUser[key]);

            string messageToSend = String.Format($"```\n{"Username",-20} -- {"Total Messages",-20} -- {"Number Of Mistakes",-20} -- {"Error Rate",-20} -- {"Total Error Percentage",-20}\n");

            int totalMessagesSent = totalMessagesByUser[username];
            int totalMistakes = totalMistakesByUser[username];
            double errorRate = ((double)totalMistakes) / totalMessagesSent * 100;
            double totalErrorShare = ((double)totalMistakes) / countOfTotalMistakes * 100;

            messageToSend += String.Format($"{username,-20} -- {totalMessagesSent,-20} -- {totalMistakes,-20} -- {errorRate,-20:0.##} -- {totalErrorShare,-20:0.##}\n");
            messageToSend += "```";
            return messageToSend;
        }

        private async Task HandleStatsCommandAsync(BotCommand command, String user)
        {
            switch(command.commandType)
            {
                case BotCommandEnum.fullStats:
                    await CalculateAndSendFullStatsPerPersonAsync(await _numberStoreRepository.GetHistoryAsync(_serviceProvider));
                    break;
                case BotCommandEnum.limitedStats:
                    await CalculateAndSendLimitedStatsPerPersonAsync(await _numberStoreRepository.GetHistoryAsync(_serviceProvider));
                    break;
                case BotCommandEnum.individualStats:
                    await CalculateAndSendIndividualStats(await _numberStoreRepository.GetHistoryAsync(_serviceProvider), user);
                    break;
            }
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
                    await HandleStatsCommandAsync(command, e.Message.From.Username);
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

                bool isNumberValue = int.TryParse(e.Message.Text, out int number);
                if(e.Message.Text != null)
                {
                    isNumberValue &= MoreRobustNumberCheck(e.Message.Text);
                }

                if ( !isNumberValue
                        || ( _lastUserToSendCorrect != null && ( _lastUserToSendCorrect == e.Message.From.Username ))
                        || ((_lastNumber != null) && number != _lastNumber + 1 ) )
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

                _stateTimer.Change(_settings.TimerWaitTime, _settings.TimerWaitTime);
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

        private bool Is1000( int x )
        {
            return x > 1000 && x % 1000 == 0;
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
        }

        private Dictionary<String,int> getMistakesByUser(List<MessageEntry> list)
        {
            var totalMistakesByUser = new Dictionary<String, int>();

            foreach (MessageEntry record in list)
            {
                if (!totalMistakesByUser.ContainsKey(record.Username))
                {
                    totalMistakesByUser[record.Username] = 0;
                }
                if (!record.Correct)
                {
                    totalMistakesByUser[record.Username] += 1;
                }
            }
            return totalMistakesByUser;
        }

        private Dictionary<String,int> getMessagesByUser(List<MessageEntry> list)
        {
            var totalMessagesByUser = new Dictionary<String, int>();

            foreach (MessageEntry record in list)
            {
                if (!totalMessagesByUser.ContainsKey(record.Username))
                {
                    totalMessagesByUser[record.Username] = 0;
                }
                totalMessagesByUser[record.Username] += 1;
            }
            return totalMessagesByUser;
        }

        private Dictionary<String, double> calculateRelativeMistakeRatesByUser( Dictionary<String, double> mistakeRates )
        {
            Dictionary<String, double> relativeRates = new Dictionary<string, double>();
            double minimumError = getMinimumErrorRate(mistakeRates);
            mistakeRates.Keys.ToList().ForEach(username =>
            {
                relativeRates.Add(username, mistakeRates[username] / minimumError);
            });
            return relativeRates;
        }

        private double getMinimumErrorRate( Dictionary<String, double> mistakeRates )
        {
            double minimumErrorPercentage = 101;
            mistakeRates.Keys.ToList().ForEach (username =>
            {
                if(mistakeRates[username] < minimumErrorPercentage && mistakeRates[username] < 0.000001 )
                {
                    minimumErrorPercentage = mistakeRates[username];
                }
            }) ;
            return minimumErrorPercentage;
        }

        private string GetRandomInsultMessageForUser( string user )
        {
            int _randInt = _rng.Next( 0, _insultList.Count );
            string message = _insultList[_randInt].Replace( "{username}", user );

            return message;
        }
    }
}
