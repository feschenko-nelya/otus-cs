using System;
using Otus.ToDoList.ConsoleBot.Types;
using Otus.ToDoList.ConsoleBot;
using HW2.User;
using HW2.Item;

namespace HW3
{
	public abstract class AbstractCommand
	{
		protected UserService? _userService;
        protected ToDoService? _toDoService;

        public AbstractCommand(UserService? userService, ToDoService? toDoService = null)
		{
			_userService = userService;
			_toDoService = toDoService;
        }
		public abstract void Execute(ITelegramBotClient botClient, Message botMessage);
		public abstract string GetInfo();
		public abstract string GetCode();
		public virtual bool IsEnabled(long telegramUserId)
		{
			if (_userService == null)
			{
				return false;
			}

			return _userService.IsUserRegistered(telegramUserId);
		}
		protected ToDoUser? GetToDoUser(long telegramUserId, out string errorMessage)
		{
			errorMessage = "";

            if (_userService == null || !IsEnabled(telegramUserId))
            {
                errorMessage = "Команда не доступна.";
                return null;
            }

            ToDoUser? toDoUser = _userService.GetUser(telegramUserId);

            if (toDoUser == null)
            {
                errorMessage = "Пользователь не найден.";
                return null;
            }

			return toDoUser;
        }
		protected List<string> GetArguments(string line)
		{
			List<string> argsList = new();

			string[] args = line.Split(' ');
			if (args.Length > 1)
			{
				for (int i = 1; i < args.Length; ++i)
				{
					argsList.Add(args[i]);
				}
			}

			return argsList;
		}
	}
}
