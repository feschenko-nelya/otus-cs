
using Core.DataAccess;
using Infrastructure.DataAccess;
using Infrastructure.Services;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace HW2
{
    internal class Program
    {
        static async Task Main()
        {
            using var cts = new CancellationTokenSource();

            IUserRepository usersRepository = new InMemoryUserRepository();
            UserService userService = new(usersRepository);

            IToDoRepository toDoRepository = new InMemoryToDoRepository();
            ToDoService toDoService = new(toDoRepository);

            var handler = new UpdateHandler(userService, toDoService);

            try
            {
                handler.OnStartedSubscribe(OnUpdateHandlerStart);
                handler.OnCompletedSubscribe(OnUpdateHandlerComplete);

                string? telegramBotToken = System.Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");

                if (string.IsNullOrEmpty(telegramBotToken))
                {
                    Console.WriteLine("Отсутствует токен для активации телеграм-бота");
                    return;
                }

                var botOptions = new TelegramBotClientOptions(telegramBotToken);
                TelegramBotClient botClient = new TelegramBotClient(botOptions);

                var receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = [UpdateType.Message],
                    DropPendingUpdates = true
                };

                botClient.StartReceiving(handler, receiverOptions: receiverOptions, cancellationToken: cts.Token);

                var me = await botClient.GetMe();
                Console.WriteLine($"{me.FirstName} запущен!");

                Console.WriteLine("Нажмите клавишу A для выхода");

                while (true)
                {
                    ConsoleKeyInfo key = Console.ReadKey();
                    if (key.KeyChar == 'A')
                    {
                        Console.WriteLine();
                        Console.WriteLine("Бот остановлен.");
                        break;
                    }
                }

                await Task.Delay(-1); // Устанавливаем бесконечную задержку
            }
            finally
            {
                handler.UnsubscribeAll();
            }

        }

        private static void OnUpdateHandlerStart(string message)
        {
            Console.WriteLine($"Началась обработка сообщения '{message}'");
        }

        private static void OnUpdateHandlerComplete(string message)
        {
            Console.WriteLine($"Закончилась обработка сообщения '{message}'");
        }
    }
}
