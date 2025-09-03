
using Core.DataAccess;
using HW2.Core.DataAccess;
using HW2.Core.Services;
using HW2.Infrastructure.DataAccess;
using HW2.Infrastructure.Services;
using HW2.TelegramBot.Scenario;
using HW2.TelegramBot.Scenarios;
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

            IUserRepository usersRepository = new FileUserRepository("ToDoUsers");
            UserService userService = new(usersRepository);

            IToDoRepository toDoRepository = new FileToDoRepository("ToDoItems");
            ToDoService toDoService = new(toDoRepository);

            IToDoListRepository toDoListRepository = new FileToDoListRepository("ToDoLists");
            IToDoListService toDoListService = new ToDoListService(toDoListRepository);

            IEnumerable<IScenario> scenarios = [new AddTaskScenario(userService, toDoService)];
            IScenarioContextRepository contextRepository = new InMemoryScenarioContextRepository();

            var handler = new UpdateHandler(userService, toDoService, toDoListService, scenarios, contextRepository);

            try
            {
                handler.OnStartedSubscribe(OnUpdateHandlerStart);
                handler.OnCompletedSubscribe(OnUpdateHandlerComplete);

                string? telegramBotToken = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");

                if (string.IsNullOrEmpty(telegramBotToken))
                {
                    Console.WriteLine("Отсутствует токен для активации телеграм-бота");
                    return;
                }

                var botOptions = new TelegramBotClientOptions(telegramBotToken);
                TelegramBotClient botClient = new TelegramBotClient(botOptions);

                var receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = [UpdateType.Message, UpdateType.CallbackQuery],
                    DropPendingUpdates = true
                };

                await botClient.SetMyCommands(handler.GetBotCommands(-1, cts.Token));

                botClient.StartReceiving(handler, receiverOptions: receiverOptions, cancellationToken: cts.Token);

                var me = await botClient.GetMe();
                Console.WriteLine($"{me.FirstName} запущен!");

                Console.WriteLine("Нажмите клавишу A для выхода");

                while (true)
                {
                    ConsoleKeyInfo key = Console.ReadKey();
                    if (key.KeyChar == 'A')
                    {
                        cts.Cancel();
                        await handler.RemoveKeyboard(botClient);
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
