using System;
using HW2.User;
using Otus.ToDoList.ConsoleBot.Types;
using Otus.ToDoList.ConsoleBot;
using System.Text;

namespace HW3
{
	public class ShowTasksCommand : AbstractCommand
	{
        public ShowTasksCommand(UserService userService) : base(userService)
        {
		}
        public override string GetCode()
        {
            return "/showtasks";
        }

        public override void Execute(ITelegramBotClient botClient, Message botMessage)
		{
			if (_userService == null)
			{
                botClient.SendMessage(botMessage.Chat, "Команда не доступна. Внутренняя ошибка.");

                return;
            }

			if (!IsEnabled(botMessage.From.Id))
			{
				botClient.SendMessage(botMessage.Chat, "Команда не доступна. Зарегистрируйтесь.");

                return;
			}

			List<string>? cmdNames = _userService.GetUserCommandsNames(botMessage.From.Id);

			if ((cmdNames == null) || ((cmdNames != null) && (cmdNames.Count == 0)))
			{
                botClient.SendMessage(botMessage.Chat, "Список задач пуст.");

				return;
            }

			StringBuilder str = new();
			for (int i = 0; i < cmdNames.Count; ++i)
			{
				str.AppendLine($"{i + 1}. {cmdNames.ElementAt(i)}");
			}

            botClient.SendMessage(botMessage.Chat, str.ToString());
        }
        public override string GetInfo()
		{
			return "Список задач пользователя.";
		}
    }
}
