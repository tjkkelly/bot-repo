using System;
using System.Windows;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using System.Collections.Generic;

namespace theCountBot
{
    class InternalMessage
    {
        public int messageInteger;
        public String SenderUsername;
    }

    class Program
    {
        static TelegramBotClient botClient = new TelegramBotClient("564251024:AAH2xC9SeCOf7JuXpdZXifPJ0koAgzwOURo");
        static int chat1Id = -197800826;
        //static int lastNumber = 0;
        static List<InternalMessage> messageHistory = new List<InternalMessage>();

        static void Main(string[] args)
        {
            RunTelegram().Wait();

            Console.ReadLine();

            botClient.StopReceiving();
        }

        static async Task RunTelegram()
        {
            Telegram.Bot.Types.User me = await botClient.GetMeAsync().ConfigureAwait(false);
            System.Console.WriteLine($"Hello! My name is {me.FirstName}");

            await botClient.SendTextMessageAsync(-197800826, "Muwahaha, I Live!!!!").ConfigureAwait(false);

            botClient.OnMessage += OnMessageReceived;
            botClient.OnMessageEdited += OnMessageEdited;

            botClient.StartReceiving();
            
            return;
        }

        static async Task SendMessage(string message)
        {
            await botClient.SendTextMessageAsync(chat1Id, message).ConfigureAwait(false);
        }

        static async Task ValidateList(Telegram.Bot.Types.Message message)
        {
            if (messageHistory.Count > 1)
            {
                int previousNumber = messageHistory[0].messageInteger;
                foreach (var internalMessage in messageHistory)
                {
                    int currentNumber = internalMessage.messageInteger;
                    if (currentNumber != previousNumber)
                    {
                        await botClient.SendTextMessageAsync(chat1Id, $"@{message.From.Username} {message.Text} is the wrong fucking number!").ConfigureAwait(false);
                        break;
                    }
                }
            }
        }

        static async void OnMessageReceived(object sender, MessageEventArgs e)
        {
            if (int.TryParse(e.Message.Text, out int number))
            {
                // if ( number != lastNumber + 1 )
                // {
                //     await SendMessage("Wrong").ConfigureAwait(false);
                // }

                messageHistory.Add( new InternalMessage
                {
                    messageInteger = number,
                    SenderUsername = e.Message.From.Username
                });
            }
            else
            {
                await SendMessage($"Holy shit \"{e.Message.Text}\" isn't a fucking number!").ConfigureAwait(false);
            }
        }

        static async void OnMessageEdited(object sender, MessageEventArgs e)
        {
            await SendMessage($"A message was edited").ConfigureAwait(false);
        }
    }
}
