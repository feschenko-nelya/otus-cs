using HW2.Item;
using HW2.User;
using HW3;
using HW4;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;

namespace HW6
{
	public class CommandInvoker
	{
        private UserService _userService;
        private ToDoService _toDoService;
        private CommandContainer _mainCommands = new();

        public CommandInvoker(UserService userService, ToDoService toDoService)
		{
            _userService = userService;
            _toDoService = toDoService;

            _mainCommands.Add(new StartCommand(_userService));
            _mainCommands.Add(new TasksMaxNumberCommand(_userService, _toDoService));
            _mainCommands.Add(new TaskMaxLengthCommand(_userService, _toDoService));
            _mainCommands.Add(new AddTaskCommand(_userService, _toDoService));
            _mainCommands.Add(new RemoveTaskCommand(_userService, _toDoService));
            _mainCommands.Add(new ShowTasksCommand(_userService, _toDoService));
            _mainCommands.Add(new InfoCommand());
            _mainCommands.Add(new HelpCommand(_mainCommands));
            _mainCommands.Add(new EndCommand(_userService));
        }

		public void Invoke(ITelegramBotClient botClient, Message message)
		{
            if (string.IsNullOrEmpty(message.Text))
            {
                return;
            }

            string[] args = message.Text.Split(' ');
            AbstractCommand? command = _mainCommands.Get(args[0]);

            if (command == null)
            {
                throw new Exception("Отсутствует объект команды: " + args[0]);
            }

            if (!command.IsEnabled(message.From.Id))
            {
                throw new Exception($"Команда '{command.GetCode()}' недоступна.");
            }

            command.Execute(botClient, message);
        }
	}
}
