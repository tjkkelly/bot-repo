using System;
using System.Threading.Tasks;
using System.Threading;
using Telegram.Bot;
using System.Linq;
using System.Collections.Generic;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.Options;
using TheCountBot.Application.Models.Enums;
using TheCountBot.Application.Models;
using TheCountBot.Data.Models;
using TheCountBot.Data.Repositories;

namespace TheCountBot
{
    internal class StatsManager : IStatsManager
    {
        private readonly ITelegramBotClient _botClient;
        private readonly Settings _settings;
        private readonly INumberStoreRepository _numberStoreRepository;

        public StatsManager( ITelegramBotClient botClient, INumberStoreRepository numberStoreRepository, IOptions<Settings> settingsOptions )
        {
            _botClient = botClient;
            _numberStoreRepository = numberStoreRepository;
            _settings = settingsOptions.Value;
        }

        public async Task HandleStatsCommandAsync( BotCommand command, string user, IServiceProvider serviceProvider )
        {
            switch( command.commandType )
            {
                case BotCommandEnum.fullStats:
                    await CalculateAndSendFullStatsPerPersonAsync( await _numberStoreRepository.GetHistoryAsync( serviceProvider ) );
                    break;
                case BotCommandEnum.limitedStats:
                    await CalculateAndSendLimitedStatsPerPersonAsync( await _numberStoreRepository.GetHistoryAsync( serviceProvider ) );
                    break;
                case BotCommandEnum.individualStats:
                    await CalculateAndSendIndividualStats( await _numberStoreRepository.GetHistoryAsync( serviceProvider ), user );
                    break;
            }
        }

        private async Task SendMessageAsync( string message, ParseMode mode = ParseMode.Default )
        {
            await _botClient.SendTextMessageAsync( _settings.MetaCountingChatId, message, mode );
        }

        private async Task CalculateAndSendLimitedStatsPerPersonAsync( List<MessageEntry> list )
        {
            var totalMistakesByUser = GetMistakesByUser(list);
            var totalMessagesByUser = GetMessagesByUser(list);

            string messageToSend = String.Format( $"```\n{"Username", -20} -- {"Total Messages Sent", -20} -- {"Number Of Mistakes", -20} -- {"Error Rate", -20}\n" );
            totalMistakesByUser.Keys.ToList().ForEach( username => {
                int totalMessagesSent = totalMessagesByUser[username];
                int totalMistakes = totalMistakesByUser[username];
                double errorRate = ((double) totalMistakes) / totalMessagesSent * 100;

                messageToSend += String.Format( $"{username, -20} -- {totalMessagesSent, -20} -- {totalMistakes, -20} -- {errorRate,-20:0.##}\n" );
            } );
            messageToSend += "```";

            await SendMessageAsync( messageToSend, ParseMode.Markdown );
        }

        private async Task CalculateAndSendFullStatsPerPersonAsync( List<MessageEntry> list )
        {
            var totalMistakesByUser = GetMistakesByUser(list);
            var totalMessagesByUser = GetMessagesByUser(list);
            var mistakeRatesByUser = new Dictionary<String, double>();

            int countOfTotalMistakes = totalMistakesByUser.Keys.ToList().Aggregate(0, (acc, key) => acc + totalMistakesByUser[key]);
            int countOfTotalMessages = totalMessagesByUser.Keys.ToList().Aggregate(0, (acc, key) => acc + totalMessagesByUser[key]);

            string messageToSend = String.Format( $"```\n{"Username",-20} -- {"Total Messages Sent",-20} -- {"Number Of Mistakes",-20} -- {"Error Rate",-20}\n" );
            totalMistakesByUser.Keys.ToList().ForEach(username => {
                int totalMessagesSent = totalMessagesByUser[username];
                int totalMistakes = totalMistakesByUser[username];
                double errorRate = ((double)totalMistakes) / totalMessagesSent * 100;
                mistakeRatesByUser.Add(username, errorRate);

                messageToSend += String.Format( $"{username,-20} -- {totalMessagesSent,-20} -- {totalMistakes,-20} -- {errorRate,-20:0.##}\n" );
            });
            messageToSend += "\n";
            var relativeErrorRates = CalculateRelativeMistakeRatesByUser(mistakeRatesByUser);
            messageToSend += String.Format( $"\n{"Username",-20} -- {"Total Message Percentage",-30} -- {"Total Error Rate",-20} -- {"Relative Error Rate",-20}\n" );
            totalMistakesByUser.Keys.ToList().ForEach(username => {
                int totalMessagesSent = totalMessagesByUser[username];
                int totalMistakes = totalMistakesByUser[username];
                double totalMessagePercentage = ((double)totalMessagesSent) / countOfTotalMessages * 100;
                double totalErrorRate = ((double)totalMistakes) / countOfTotalMistakes * 100;
                double relativeError = relativeErrorRates[username];

                messageToSend += String.Format( $"{username,-20} -- {totalMessagePercentage,-30:0.##} -- {totalErrorRate,-20:0.##} -- {relativeError,-20:0.##}\n" );
            });
            messageToSend += "```";

            await SendMessageAsync( messageToSend, ParseMode.Markdown );
        }

        private async Task CalculateAndSendIndividualStats( List<MessageEntry> list, String username )
        {
            var totalMistakesByUser = GetMistakesByUser( list );
            var totalMessagesByUser = GetMessagesByUser( list );

            String messageToSend = "";
            if( !totalMessagesByUser.ContainsKey( username ) )
            {
                messageToSend = "You haven't counted yet!";
            }
            else
            {
                messageToSend = BuildIndividualStatsMessage( totalMistakesByUser, totalMessagesByUser, username );
            }

            await SendMessageAsync( messageToSend, ParseMode.Markdown );
        }

        private static string BuildIndividualStatsMessage( Dictionary<String, int> totalMistakesByUser, Dictionary<String, int> totalMessagesByUser, String username )
        {
            int countOfTotalMistakes = totalMistakesByUser.Keys.ToList().Aggregate(0, ( acc, key ) => acc + totalMistakesByUser[key]);

            string messageToSend = String.Format( $"```\n{"Username",-20} -- {"Total Messages",-20} -- {"Number Of Mistakes",-20} -- {"Error Rate",-20} -- {"Total Error Percentage",-20}\n" );

            int totalMessagesSent = totalMessagesByUser[username];
            int totalMistakes = totalMistakesByUser[username];
            double errorRate = ((double)totalMistakes) / totalMessagesSent * 100;
            double totalErrorShare = ((double)totalMistakes) / countOfTotalMistakes * 100;

            messageToSend += String.Format( $"{username,-20} -- {totalMessagesSent,-20} -- {totalMistakes,-20} -- {errorRate,-20:0.##} -- {totalErrorShare,-20:0.##}\n" );
            messageToSend += "```";
            return messageToSend;
        }

        private static Dictionary<String,int> GetMistakesByUser( List<MessageEntry> list )
        {
            var totalMistakesByUser = new Dictionary<String, int>();

            foreach ( MessageEntry record in list )
            {
                if ( !totalMistakesByUser.ContainsKey( record.Username ) )
                {
                    totalMistakesByUser[record.Username] = 0;
                }
                if ( !record.Correct )
                {
                    totalMistakesByUser[record.Username] += 1;
                }
            }
            return totalMistakesByUser;
        }

        private static Dictionary<String,int> GetMessagesByUser( List<MessageEntry> list )
        {
            var totalMessagesByUser = new Dictionary<String, int>();

            foreach  ( MessageEntry record in list )
            {
                if ( !totalMessagesByUser.ContainsKey( record.Username ) )
                {
                    totalMessagesByUser[record.Username] = 0;
                }
                totalMessagesByUser[record.Username] += 1;
            }
            return totalMessagesByUser;
        }

        private static Dictionary<String, double> CalculateRelativeMistakeRatesByUser( Dictionary<String, double> mistakeRates )
        {
            Dictionary<String, double> relativeRates = new Dictionary<string, double>();
            double minimumError = GetPositiveMinimumNonZeroErrorRate( mistakeRates );
            mistakeRates.Keys.ToList().ForEach( username =>
            {
                relativeRates.Add( username, mistakeRates[username] / minimumError );
            });
            return relativeRates;
        }

        private static double GetPositiveMinimumNonZeroErrorRate( Dictionary<String, double> mistakeRates )
        {
            double minimumErrorPercentage = 101;
            mistakeRates.Keys.ToList().ForEach (username =>
            {
                if( mistakeRates[username] < minimumErrorPercentage && mistakeRates[username] > 0.0001 )
                {
                    minimumErrorPercentage = mistakeRates[username];
                }
            }) ;
            return minimumErrorPercentage;
        }
    }
}