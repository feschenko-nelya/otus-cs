using System.Text;
using HW2.Item;
using HW2.User;
using HW3;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;

public class UserItemsShowAllCommand : AbstractCommand
{
	public UserItemsShowAllCommand(UserService userService, ToDoService toDoService) : base(userService, toDoService)
	{
	}

    public override void Execute(ITelegramBotClient botClient, Message botMessage)
    {
        string errorMessage;
        ToDoUser? toDoUser = GetToDoUser(botMessage.From.Id, out errorMessage);

        if (toDoUser == null)
        {
            botClient.SendMessage(botMessage.Chat, "Ошибка: " + errorMessage);

            return;
        }

        if (_toDoService == null)
        {
            botClient.SendMessage(botMessage.Chat, "Нет доступа к задачам пользователя.");

            return;
        }

        var userCommands = _toDoService.GetActiveByUserId(toDoUser.UserId);

        if (userCommands.Count == 0)
        {
            botClient.SendMessage(botMessage.Chat, "Список задач пуст.");

            return;
        }

        StringBuilder str = new();
        for (int i = 0; i < userCommands.Count; ++i)
        {
            ToDoItem item = userCommands.ElementAt(i);
            if (item != null)
            {
                str.AppendLine($" ({item.GetStateName()}) {item.toString()}");
            }
        }

        botClient.SendMessage(botMessage.Chat, str.ToString());
    }

    public override string GetCode()
    {
        return "/showalltasks";
    }

    public override string GetInfo()
    {
        return "Список всех задач пользователя.";
    }
}
