using HW2.Item;
using HW2.User;
using HW3;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;

public class TaskMaxLengthCommand : AbstractCommand
{
    private readonly int _minLength = 50;

    public TaskMaxLengthCommand(UserService userService, ToDoService toDoService) : base(userService, toDoService)
	{
	}
    public override string GetCode()
    {
        return "/taskmaxlength";
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

        List<string> args = GetArguments(botMessage.Text);
        if (args.Count == 0)
        {
            botClient.SendMessage(botMessage.Chat, $"Введите максимально допустимую длину задачи (от {_minLength}.");
            return;
        }

        short length = -1;
        if (!short.TryParse(args.ElementAt(0), out length))
        {
            botClient.SendMessage(botMessage.Chat, "Значение неверно.");
            return;
        }

        if (length < _minLength)
        {
            botClient.SendMessage(botMessage.Chat, "Введено неоптимальное значение длины задачи.");
            return;
        }

        if (_toDoService.SetMaxLength(toDoUser.UserId, length))
        {
            botClient.SendMessage(botMessage.Chat, $"Установлена максимальная длина задачи: {length}.");
        }
        else
        {
            botClient.SendMessage(botMessage.Chat, "Максимальная длина задачи не установлена.");
        }
    }

    public override string GetInfo()
    {
        return "Максимально допустимая длина задачи";
    }
}
