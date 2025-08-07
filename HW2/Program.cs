
using HW2.Item;
using HW2.User;
using Otus.ToDoList.ConsoleBot;

namespace HW2
{
    internal class Program
    {
        static void Main()
        {
            UserService userService = new();
            ToDoService toDoService = new();
            
            using var cts = new CancellationTokenSource();
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
            Console.WriteLine("Началась обработка сообщения '{message}'");
        }

        private static void OnUpdateHandlerComplete(string message)
        {
            Console.WriteLine("Закончилась обработка сообщения '{message}'");
        }
    }
}
