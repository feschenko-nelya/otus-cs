using System;
using System.Xml.Linq;
using HW2.User;
using Otus.ToDoList.ConsoleBot.Types;
using Otus.ToDoList.ConsoleBot;

namespace HW3
{
	public class StartCommand : AbstractCommand
	{
        public StartCommand(UserService userService) : base(userService)
		{
        }
        public override string GetCode()
        {
            return "/start";
        }

        public override void Execute(ITelegramBotClient botClient, Message botMessage)
		{
            ToDoUser? toDoUser = _userService.RegisterUser(botMessage.From.Id, botMessage.From.Username);

            if (toDoUser == null)
            {
                botClient.SendMessage(botMessage.Chat, $"Здравствуйте, {botMessage.From.Username}. Вы не зарегистрированы.");
            }
            else 
            { 
                botClient.SendMessage(botMessage.Chat, $"Здравствуйте, {toDoUser.TelegramUserName}. Вы зарегистрированы.");
            }
        }
        public override string GetInfo()
		{
			return "Начало работы бота: регистрация пользователя;";
		}
        public override bool IsEnabled(long telegramUserId)
        {
            return true;
        }
    }
}
