using System;
using HW2.User;
using HW4;
using Otus.ToDoList.ConsoleBot.Types;
using Otus.ToDoList.ConsoleBot;
using HW2.Item;

namespace HW3
{
	public class RemoveTaskCommand : AbstractCommand
	{
		public RemoveTaskCommand(UserService userService, ToDoService toDoService) : base(userService, toDoService)
        {
        }
        public override string GetCode()
        {
            return "/removetask";
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
                botClient.SendMessage(botMessage.Chat, "Введите номер команды, начиная с 1, которую нужно удалить.");
                return;
            }

            int number = -1;
            if (!int.TryParse(args.ElementAt(0), out number))
            {
                botClient.SendMessage(botMessage.Chat, "Номер команды неверный.");
                return;
            }
            
            if (number < 1)
            {
                botClient.SendMessage(botMessage.Chat, "Номер команды неверный.");
                return;
            }

            if (_toDoService.Delete(toDoUser.UserId, number - 1))
            {
                botClient.SendMessage(botMessage.Chat, $"Команда №{number} удалена.");
            }
            else
            { 
                botClient.SendMessage(botMessage.Chat, $"Команда №{number} не удалена.");
            }
    	}

        public override string GetInfo()
		{
			return "Удаление задачи пользователя.";
		}
    }
}
