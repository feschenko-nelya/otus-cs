using System;
using HW2.Item;
using HW2.User;
using HW3;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;

namespace HW4
{
	public class UserItemsSetMaxNumberCommand : AbstractCommand
	{
        private readonly short _minNumber = 1;
        private readonly short _maxNumber = 100;

        public UserItemsSetMaxNumberCommand(UserService userService, ToDoService toDoService) : base(userService, toDoService)
        {
		}
        public override string GetCode()
        {
            return "/tasksmaxnumber";
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
                botClient.SendMessage(botMessage.Chat, 
                                      $"Введите максимально допустимое количество задач в пределе [{_minNumber}, {_maxNumber}].");
                return;
            }

            short number = -1;
            if (!short.TryParse(args.ElementAt(0), out number))
            {
                botClient.SendMessage(botMessage.Chat,
                                      $"Введите максимально допустимое количество задач в пределе [{_minNumber}, {_maxNumber}].");
                return;
            }

            if (number < _minNumber || number > _maxNumber)
            {
                botClient.SendMessage(botMessage.Chat, $"Введите максимально допустимое количество задач в пределе [{_minNumber}, {_maxNumber}].");
                return;
            }

            if (_toDoService.SetMaxNumber(toDoUser.UserId, number))
            {
                botClient.SendMessage(botMessage.Chat, $"Установлено максимально допустимое количество задач: {number}.");
            }
            else
            {
                botClient.SendMessage(botMessage.Chat, "Максимально допустимое количество задач не установлено.");
            }
        }

        public override string GetInfo()
        {
            return "Максимально допустимое количество задач";
        }
    }
}
