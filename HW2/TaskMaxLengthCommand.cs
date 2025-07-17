using System;
using HW3;
using HW4;
using HW2.User;
using Otus.ToDoList.ConsoleBot.Types;
using Otus.ToDoList.ConsoleBot;

public class TaskMaxLengthCommand : AbstractCommand
{
    private readonly int _minLength = 50;

    public TaskMaxLengthCommand(UserService userService) : base(userService)
	{
	}
    public override string GetCode()
    {
        return "/taskmaxlength";
    }

    public override void Execute(ITelegramBotClient botClient, Message botMessage)
    {
        if (_userService == null || !IsEnabled(botMessage.From.Id))
        {
            botClient.SendMessage(botMessage.Chat, "Команда не доступна.");
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

        if (_userService.SetUserCommandMaxLength(botMessage.From.Id, length))
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
