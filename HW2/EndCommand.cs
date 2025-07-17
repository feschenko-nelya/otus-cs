using System;
using System.Xml.Linq;
using HW2.User;
using Otus.ToDoList.ConsoleBot.Types;
using Otus.ToDoList.ConsoleBot;
using System.Text;

namespace HW3
{
	public class EndCommand : AbstractCommand
    {
        public EndCommand(UserService userService) : base(userService)
		{
		}
        public override string GetCode()
        {
            return "/exit";
        }

        public override void Execute(ITelegramBotClient botClient, Message botMessage)
        {
            if (_userService == null)
            {
                return;
            }

            ToDoUser? user = _userService.GetUser(botMessage.From.Id);

            StringBuilder str = new();
            str.Append("До свидания");

            if (user != null)
            {
                str.Append($", {user.TelegramUserName}.");
                _userService.UnregisterUser(user.TelegramUserId);
            }
            else
            {
                str.Append(".");
            }

            botClient.SendMessage(botMessage.Chat, str.ToString());
        }

        public override string GetInfo()
        {
            return "Завершение работы программы";
        }

        public override bool IsEnabled(long telegramUserId)
        {
            return true;
        }
    }
}