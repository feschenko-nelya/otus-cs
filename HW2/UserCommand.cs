using System;
using Otus.ToDoList.ConsoleBot.Types;
using Otus.ToDoList.ConsoleBot;

namespace HW3
{
	public class UserCommand : AbstractCommand
	{
		private string _name = string.Empty;
		public UserCommand(string name) : base(null)
		{
            _name = name;
		}

        public override string GetCode()
		{
            return _name;
        }
        public override void Execute(ITelegramBotClient botClient, Message botMessage)
		{
            botClient.SendMessage(botMessage.Chat, _name);
        }
        public override string GetInfo()
		{
			return _name;
		}

        public override bool IsEnabled(long telegramUserId)
        {
            return true;
        }
    }
}
