using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using TheCountBot;
using TheCountBot.Application.Models.Enums;
using TheCountBot.Data.Models;
using TheCountBot.Data.Repositories;

public class UpdateHandler : IUpdateHandler
{
    private readonly IServiceProvider serviceProvider;
    private readonly IStatsManager statsManager;
    private readonly INumberStoreRepository numberStoreRepository;
    private readonly Settings settings;
    private readonly ILogger<UpdateHandler> logger;



    private static int? _lastNumber = null;
    private static string _lastUserToSendCorrect = null;
    private static Timer stateTimer;

    public UpdateHandler(IServiceProvider serviceProvider, IOptions<Settings> settings, IStatsManager statsManager, INumberStoreRepository numberStoreRepository, ILogger<UpdateHandler> logger)
    {
        this.serviceProvider = serviceProvider;
        this.statsManager = statsManager;
        this.numberStoreRepository = numberStoreRepository;
        this.settings = settings.Value;
        this.logger = logger;

        if (stateTimer == null)
        {
            stateTimer = new Timer(TimerFunc, serviceProvider, this.settings.TimerWaitTime, this.settings.TimerWaitTime);
        }
    }

    private static void TimerFunc(object stateInfo)
    {
        if (stateInfo is IServiceProvider serviceProvider)
        {
            ITelegramBotClient botClient = serviceProvider.GetRequiredService<ITelegramBotClient>();
            Settings settings = serviceProvider.GetRequiredService<IOptions<Settings>>().Value;
            botClient.SendTextMessageAsync(settings.MetaCountingChatId, "I'm lonely...").Wait();
        }
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        logger.LogInformation("Message Received chatId: {0} msg: \"{1}\"", update.Message.Chat.Id, update.Message.Text);
        if (update.Message.Chat.Id == settings.MetaCountingChatId)
        {
            var command = new TheCountBot.Application.Models.BotCommand(update.Message.Text);
            if (command.commandType != BotCommandEnum.noCommand)
            {
                await statsManager.HandleStatsCommandAsync(command, update.Message.From.Username, serviceProvider);
                return;
            }
        }

        if (update.Message.Chat.Id == settings.CountingChatId)
        {
            MessageEntry messageEntry = new MessageEntry
            {
                Username = update.Message.From.Username == null ? update.Message.From.FirstName : update.Message.From.Username,
                Timestamp = DateTime.UtcNow
            };

            bool isNumberValue = int.TryParse(update.Message.Text, out int number);
            if (update.Message.Text != null)
            {
                isNumberValue &= MoreRobustNumberCheck(update.Message.Text);
            }

            if (!isNumberValue
                    || (_lastUserToSendCorrect != null && (_lastUserToSendCorrect == update.Message.From.Username))
                    || ((_lastNumber != null) && number != _lastNumber + 1))
            {
                _lastUserToSendCorrect = null;
                _lastNumber = null;

                messageEntry.Correct = false;
                messageEntry.Number = -1;

                await botClient.SendTextMessageAsync(settings.MetaCountingChatId, await GetRandomInsultMessageForUserAsync(update.Message.From.Username));
            }
            else
            {
                _lastNumber = number;
                _lastUserToSendCorrect = update.Message.From.Username;

                messageEntry.Correct = true;
                messageEntry.Number = number;

                await HandleCoolNumbersAsync(botClient, number, update.Message.From.Username);
            }

            stateTimer.Change(settings.TimerWaitTime, settings.TimerWaitTime);
            await numberStoreRepository.AddNewMessageEntryAsync(serviceProvider, messageEntry);
        }
        else
        {
            string messageText = update.Message.Text;
            if (messageText.Equals("insult me", StringComparison.InvariantCultureIgnoreCase))
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, await GetRandomInsultMessageForUserAsync(update.Message.From.Username));
            }
        }
    }

    private bool MoreRobustNumberCheck(string x)
    {
        if (x.StartsWith("0")) return false;

        // potentially other checks...

        return true;
    }

    private bool IsSameDigits(int x)
    {
        //not counting numbers less than 10
        if (x < 10) return false;
        int firstDigit = x % 10;

        while (x > 0)
        {
            if (x % 10 != firstDigit) return false;
            x /= 10;
        }
        return true;
    }

    private bool IsPalindromeSetup(int x)
    {
        return IsPalindrome(x + 1);

    }

    private bool IsPalindrome(int x)
    {
        //not counting numbers less than 10
        if (x < 10) return false;

        int original = x, reverse = 0;

        while (x > 0)
        {
            reverse *= 10;
            reverse += x % 10;
            x /= 10;
        }

        return original == reverse;
    }

    private bool Is1000(int x)
    {
        return x > 1000 && x % 1000 == 0;
    }

    private bool IsNice(int x)
    {
        return x % 100 == 69;
    }

    private bool IsEvil(int x)
    {
        return x % 1000 == 666;
    }

    private bool IsDank(int x)
    {
        return x % 1000 == 420;
    }

    private bool IsSelfDescribing(int x)
    {
        Dictionary<int, int> counter = new Dictionary<int, int>();
        while (x > 0)
        {
            var lastDigit = x % 10;
            x /= 10;

            if (!counter.ContainsKey(lastDigit))
            {
                counter[lastDigit] = 0;
            }
            counter[lastDigit] += 1;
        }

        foreach (KeyValuePair<int, int> pair in counter)
        {
            if (pair.Key != pair.Value)
            {
                return false;
            }
        }
        return true;
    }

    private async Task HandleCoolNumbersAsync(ITelegramBotClient botClient, int x, string user)
    {
        if (IsSameDigits(x))
        {
            await botClient.SendTextMessageAsync(settings.MetaCountingChatId, $"YO @{user}, {x} is made up of all {x % 10}s!");
        }
        else if (IsPalindrome(x))
        {
            await botClient.SendTextMessageAsync(settings.MetaCountingChatId, $"Hey, @{user}! {x} is a palindrome!");
        }
        else if (Is1000(x))
        {
            await botClient.SendTextMessageAsync(settings.MetaCountingChatId, $"AYYYYYY @{user}");
        }
        else if (IsPalindromeSetup(x))
        {
            await botClient.SendTextMessageAsync(settings.MetaCountingChatId, $"Yo @{user} thanks for setting us up for a palindrome!");
        }
        else if (IsEvil(x))
        {
            await botClient.SendTextMessageAsync(settings.MetaCountingChatId, $"@{user} you devil, you!");
        }
        else if (IsNice(x))
        {
            await botClient.SendTextMessageAsync(settings.MetaCountingChatId, $"@{user} Nice.");
        }
        else if (IsDank(x))
        {
            await botClient.SendTextMessageAsync(settings.MetaCountingChatId, $"@{user} blaze it!");
        }
        else if (IsSelfDescribing(x))
        {
            await botClient.SendTextMessageAsync(settings.MetaCountingChatId, $"Yo @{user} that number is kinda cool!");
        }
        // else if (IsChaotic())
        // {
        //     await botClient.SendTextMessageAsync( settings.MetaCountingChatId, $"AYO @{user}! That's a nice number you wrote there <3");
        // }
    }
    private async Task<string> GetRandomInsultMessageForUserAsync(string user)
    {
        return (await numberStoreRepository.GetRandomUserInsultAsync(serviceProvider)).Value.Replace("{username}", user);
    }
}