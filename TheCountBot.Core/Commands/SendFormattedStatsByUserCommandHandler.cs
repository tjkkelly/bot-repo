using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TheCountBot.Core.DataModels;
using TheCountBot.Core.Interfaces;

namespace TheCountBot.Core.Commands
{
    public class SendFormattedStatsByUserCommandHandler : IRequestHandler<SendFormattedStatsByUserCommand>
    {
        private readonly ITelegramBotClient _telegramBotClient;

        public SendFormattedStatsByUserCommandHandler(ITelegramBotClient telegramBotClient)
        {
            _telegramBotClient = telegramBotClient;
        }

        public async Task<Unit> Handle( SendFormattedStatsByUserCommand request, CancellationToken cancellationToken )
        {
            string messageToSend = GetFormattedMessageString( request.UsersStatistics );

            await _telegramBotClient.SendMonospacedMessageAsync( messageToSend );

            return Unit.Value;
        }

        private string GetFormattedMessageString( IList<UserStatistics> usersStatistics )
        {
            string messageToSend = $"```\n{"Username",-20} -- {"Total Messages Sent",-20} -- {"Number Of Mistakes",-20} -- {"Error Rate",-20}\n";

            foreach ( UserStatistics userStatistics in usersStatistics )
            {
                messageToSend += $"{userStatistics.Username,-20} -- {userStatistics.MessagesSentCount,-20} -- {userStatistics.MistakeCount,-20} -- {userStatistics.ErrorRate,-20:0.##}\n";
            }

            messageToSend += "\n";
            messageToSend += $"\n{"Username",-20} -- {"Total Message Percentage",-30} -- {"Total Error Rate",-20} -- {"Relative Error Rate",-20}\n";

            foreach ( UserStatistics userStatistics in usersStatistics )
            {
                messageToSend += $"{userStatistics.Username,-20} -- {userStatistics.TotalMessagePercentage,-30:0.##} -- {userStatistics.TotalErrorRate,-20:0.##} -- {userStatistics.RelativeErrorRate,-20:0.##}\n";
            }

            messageToSend += "```";

            return messageToSend;
        }
    }
}
