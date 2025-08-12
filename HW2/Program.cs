
using Otus.ToDoList.ConsoleBot;
using Core.DataAccess;
using Infrastructure.DataAccess;
using Infrastructure.Services;

namespace HW2
{
    internal class Program
    {
        static void Main()
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

                var botClient = new ConsoleBotClient();
                botClient.StartReceiving(handler, cts.Token);
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
