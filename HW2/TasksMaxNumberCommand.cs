using System;
using HW2.User;
using HW3;
using Otus.ToDoList.ConsoleBot.Types;
using Otus.ToDoList.ConsoleBot;

namespace HW4
{
	public class TasksMaxNumberCommand : AbstractCommand
	{
        private readonly short _minNumber = 1;
        private readonly short _maxNumber = 100;

        public TasksMaxNumberCommand(UserService userService) : base(userService)
        {
		}
        public override string GetCode()
        {
            return "/tasksmaxnumber";
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

            if (_userService.SetUserCommandsMaxNumber(botMessage.From.Id, number))
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
