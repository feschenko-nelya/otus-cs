
using HW2.User;
using HW3;
using HW6;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;

namespace HW2
{
    internal class UpdateHandler : IUpdateHandler
    {
        private UserService _userService = new UserService();
        private CommandInvoker _commandInvoker;

        public UpdateHandler()
        {
            _commandInvoker = new CommandInvoker(_userService);
        }

        public void HandleUpdateAsync(ITelegramBotClient botClient, Update update)
        {
            try
            {
                _commandInvoker.Invoke(botClient, update.Message);
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
