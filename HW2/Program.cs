
using HW2.User;
using Otus.ToDoList.ConsoleBot;

namespace HW2
{
    internal class Program
    {
        static void Main()
        {
            UserService userService = new();

            var handler = new UpdateHandler(userService);
            var botClient = new ConsoleBotClient();
            botClient.StartReceiving(handler);
        }
    }
}
