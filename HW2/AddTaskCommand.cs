using System;
using HW2.User;
using Otus.ToDoList.ConsoleBot.Types;
using Otus.ToDoList.ConsoleBot;

namespace HW3
{
    public class AddTaskCommand : AbstractCommand
    {
        public AddTaskCommand(UserService userService) : base(userService)
        {
	    }

        public override string GetCode()
        {
            return "/addtask";
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
                botClient.SendMessage(botMessage.Chat, "Введите название задачи.");
                return;
            }

            string commandName = string.Join(" ", args);

            if (_userService.AddUserCommand(botMessage.From.Id, string.Join(" ", args)))
            {
                botClient.SendMessage(botMessage.Chat, "Задача добавлена.");
            }
            else
            {
                botClient.SendMessage(botMessage.Chat, "Задача не добавлена.");
            }
        }
        public override string GetInfo()
        {
            return "Добавление новой задачи.";
        }
    }

    public class TaskCountLimitException : Exception
    {
        public TaskCountLimitException(int taskCountLimit) 
               : base($"Превышено максимальное количество задач равное {taskCountLimit}")
        {
        }
    }
    public class TaskLengthLimitException : Exception
    {
        public TaskLengthLimitException(int taskLength, int taskLengthLimit)
               : base($"Длина задачи ‘{taskLength}’ превышает максимально допустимое значение {taskLengthLimit}.")
        {
        }
    }
    public class DuplicateTaskException : Exception
    {
        public DuplicateTaskException(string task)
               : base($"Задача ‘{task}’ уже существует.")
        {
        }
    }
}