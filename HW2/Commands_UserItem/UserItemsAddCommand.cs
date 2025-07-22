using System;
using HW2.Item;
using HW2.User;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;

namespace HW3
{
    public class UserItemsAddCommand : AbstractCommand
    {
        public UserItemsAddCommand(UserService userService, ToDoService toDoService) : base(userService, toDoService)
        {
	    }

        public override string GetCode()
        {
            return "/addtask";
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
                botClient.SendMessage(botMessage.Chat, "Введите название задачи.");
                return;
            }

            string commandName = string.Join(" ", args);

            ToDoItem? newItem = _toDoService.Add(toDoUser.UserId, string.Join(" ", args));
            if (newItem != null)
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