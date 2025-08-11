
using HW2.Bot_Item;
using HW2.Bot_User;
using HW2.Item;
using HW2.User;
using Otus.ToDoList.ConsoleBot;

namespace HW2
{
    internal class Program
    {
        static void Main()
        {
            IUserRepository usersRepository = new InMemoryUserRepository();
            UserService userService = new(usersRepository);

            IToDoRepository toDoRepository = new InMemoryToDoRepository();
            ToDoService toDoService = new(toDoRepository);

            var handler = new UpdateHandler(userService, toDoService);
            var botClient = new ConsoleBotClient();
            botClient.StartReceiving(handler);
        }
    }
}
