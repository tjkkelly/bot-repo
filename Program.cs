using System;
using System.Threading.Tasks;

namespace theCountBot
{
    class Program
    {
        static void Main(string[] args)
        {
            RunTelegram().Wait();
        }

        static async Task RunTelegram()
        {
                var botClient = new Telegram.Bot.TelegramBotClient("564251024:AAH2xC9SeCOf7JuXpdZXifPJ0koAgzwOURo");
                Telegram.Bot.Types.User me = await botClient.GetMeAsync();
                System.Console.WriteLine($"Hello! My name is {me.FirstName}");

                return;
        }
    }
}
