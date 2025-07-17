using System;
using HW2.User;
using HW4;
using Otus.ToDoList.ConsoleBot.Types;
using Otus.ToDoList.ConsoleBot;

namespace HW3
{
	public class RemoveTaskCommand : AbstractCommand
	{
		public RemoveTaskCommand(UserService userService) : base(userService)
        {
        }
        public override string GetCode()
        {
            return "/removetask";
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

            if (!_userService.RemoveUserCommand(botMessage.From.Id, number - 1))
            {
                botClient.SendMessage(botMessage.Chat, $"Команда №{number} не удалена.");
                return;
            }

            botClient.SendMessage(botMessage.Chat, "Команда удалена.");
		}

        public override string GetInfo()
		{
			return "Удаление задачи пользователя.";
		}
    }
}
