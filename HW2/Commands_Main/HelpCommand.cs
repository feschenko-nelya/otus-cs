using System;
using System.Text;
using Otus.ToDoList.ConsoleBot.Types;
using Otus.ToDoList.ConsoleBot;

namespace HW3
{
	public class HelpCommand : AbstractCommand
	{
        private readonly CommandContainer _cmdsContainer;

        public HelpCommand(CommandContainer commands) : base(null)
		{
            _cmdsContainer = commands;
        }

        public override string GetCode()
        {
            return "/help";
        }

        public override void Execute(ITelegramBotClient botClient, Message botMessage)
		{
            StringBuilder str = new();
            str.AppendLine(GetInfo());

            foreach (var command in _cmdsContainer)
            {
                str.Append(command.GetCode());
                str.Append(" - ");
                str.AppendLine(command.GetInfo());
            }

            botClient.SendMessage(botMessage.Chat, str.ToString());
        }
        public override string GetInfo()
		{
			return "Краткая информация о программе";
		}

        public override bool IsEnabled(long telegramUserId)
        {
            return true;
        }
    }
}