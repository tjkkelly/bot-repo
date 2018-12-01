using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TheCountBot.Core.Factories;
using TheCountBot.Core.Interfaces;

namespace TheCountBot.Core.Commands
{
    public class SendIncorrectNumberCommandHandler : IRequestHandler<SendIncorrectNumberCommand>
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly CustomizedInsultFactory _customizedInsultFactory;

        public SendIncorrectNumberCommandHandler( ITelegramBotClient telegramBotClient, CustomizedInsultFactory customizedInsultFactory )
        {
            _telegramBotClient = telegramBotClient;
            _customizedInsultFactory = customizedInsultFactory;
        }

        public async Task<Unit> Handle( SendIncorrectNumberCommand request, CancellationToken cancellationToken )
        {
            string insultingMessage = _customizedInsultFactory.GetInsultForUser( request.Username );
            await _telegramBotClient.SendRawMessageAsync( insultingMessage );

            return Unit.Value;
        }
    }
}
