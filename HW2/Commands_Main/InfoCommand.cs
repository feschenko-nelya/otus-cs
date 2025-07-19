using System;
using Otus.ToDoList.ConsoleBot.Types;
using Otus.ToDoList.ConsoleBot;

namespace HW3
{
	public class InfoCommand : AbstractCommand
	{
        private static readonly Version _version = new Version(1, 0);
        private static readonly DateTime _creationDate = new DateTime(2025, 3, 27);

        public InfoCommand() : base(null)
		{
		}

        public override string GetCode()
        {
            return "/info";
        }

        public override void Execute(ITelegramBotClient botClient, Message botMessage)
		{
            botClient.SendMessage(botMessage.Chat, $"Version: {_version.ToString()}");
            botClient.SendMessage(botMessage.Chat, $"Creation date: {_creationDate.ToString("dd.MM.yyyy")}");
        }

        public override string GetInfo()
		{
			return "Информация о версии и дате создания программы.";
		}
        public override bool IsEnabled(long telegramUserId)
        {
            return true;
        }
    }
}
