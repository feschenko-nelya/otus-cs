using System;
using Otus.ToDoList.ConsoleBot.Types;
using Otus.ToDoList.ConsoleBot;
using HW2.User;

namespace HW3
{
	public abstract class AbstractCommand
	{
		protected UserService? _userService;

		public AbstractCommand(UserService? userService)
		{
			_userService = userService;

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
