
using HW2.Item;
using HW2.User;
using HW3;
using HW4;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;

namespace HW2
{
    internal class UpdateHandler : IUpdateHandler
    {
        private UserService _userService = new UserService();
        private ToDoService _toDoService = new ToDoService();
        private CommandContainer _mainCommands = new();

        public UpdateHandler()
        {
            _mainCommands.Add(new StartCommand(_userService));
            _mainCommands.Add(new UserItemsSetMaxNumberCommand(_userService, _toDoService));
            _mainCommands.Add(new UserItemsSetMaxLengthCommand(_userService, _toDoService));
            _mainCommands.Add(new UserItemsAddCommand(_userService, _toDoService));
            _mainCommands.Add(new UserItemsRemoveCommand(_userService, _toDoService));
            _mainCommands.Add(new UserItemsSetCompleteCommand(_userService, _toDoService));
            _mainCommands.Add(new UserItemsShowAllCommand(_userService, _toDoService));
            _mainCommands.Add(new UserTasksShowActiveCommand(_userService, _toDoService));
            _mainCommands.Add(new InfoCommand());
            _mainCommands.Add(new EndCommand(_userService));
            _mainCommands.Add(new HelpCommand(_mainCommands));
        }

        public void HandleUpdateAsync(ITelegramBotClient botClient, Update update)
        {
            try
            {
                Message botMesage = update.Message;

                if (string.IsNullOrEmpty(botMesage.Text))
                {
                    return;
                }

                string[] args = update.Message.Text.Split(' ');
                AbstractCommand? command = _mainCommands.Get(args[0]);

                if (command == null)
                {
                    throw new Exception("Отсутствует объект команды: " + args[0]);
                }

                if (!command.IsEnabled(botMesage.From.Id))
                {
                    throw new Exception($"Команда '{command.GetCode()}' недоступна.");
                }

                command.Execute(botClient, botMesage);
            }
            catch (Exception exception)
            {
                ProcessException(botClient, update.Message.Chat, exception);
            }
        }

        private static void ProcessException(ITelegramBotClient botClient, Chat botChat, Exception exception)
        {
            if ((exception.GetType() == typeof(ArgumentException))
                || (exception.GetType() == typeof(TaskCountLimitException))
                || (exception.GetType() == typeof(TaskLengthLimitException))
                || (exception.GetType() == typeof(DuplicateTaskException))
               )
            {
                ShowException(botClient, botChat, exception.Message);
            }
            else
            {
                ShowException(botClient, botChat, 
                        $"""
                        Произошла непредвиденная ошибка.
                        Детальная информация:

                        Type: {exception.GetType}
                        Message: {exception.Message}
                        Stack trace: 
                        {exception.StackTrace}
                        Inner exception: {exception.InnerException}
                        """);
            }
        }

        private static void ShowException(ITelegramBotClient botClient, Chat botChat, string message)
        {
            botClient.SendMessage(botChat, "");
            botClient.SendMessage(botChat, message);
        }
    }
}
