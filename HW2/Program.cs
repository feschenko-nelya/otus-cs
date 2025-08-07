
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

            var botClient = new ConsoleBotClient();
            botClient.StartReceiving(handler, cts.Token);
        }
    }
}
