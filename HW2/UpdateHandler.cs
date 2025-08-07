using System.Text;
using HW2.Item;
using HW2.User;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;

namespace HW2
{
    internal class UpdateHandler : IUpdateHandler
    {
        private IUserService _userService;
        private IToDoService _toDoService;

        private static readonly Version _version = new Version(1, 0);
        private static readonly DateTime _creationDate = new DateTime(2025, 3, 27);

        delegate void dlgtDoCommand(ITelegramBotClient botClient, Message botMessage);
        struct CommandData
        {
            public string code;
            public dlgtDoCommand execute;
            public string help;
            public bool isUsers = false;

            public CommandData(string code, dlgtDoCommand command, string help, bool isUsers = false)
            {
                this.code = code;
                this.help = help;
                this.execute = command;
                this.isUsers = isUsers;
            }
        }
        List<CommandData> _commands = new();

        public UpdateHandler(IUserService userService, IToDoService toDoService)
        {
            _userService = userService;
            _toDoService = toDoService;

            _commands.Add(new CommandData("/start", 
                                          StartCommand, 
                                          "Начало работы бота: регистрация пользователя."));
            _commands.Add(new CommandData("/info", 
                                          InfoCommand, 
                                          "Информация о версии и дате создания программы."));
            _commands.Add(new CommandData("/exit", 
                                          EndCommand, 
                                          "Завершение работы программы.",
                                          true));
            _commands.Add(new CommandData("/help", 
                                          HelpCommand, 
                                          "Краткая информация о программе."));

            _commands.Add(new CommandData("/addtask", 
                                          UserItemsAddCommand, 
                                          "Добавление новой задачи.", 
                                          true));
            _commands.Add(new CommandData("/removetask",
                                          UserItemsRemoveCommand,
                                          "Удаление задачи пользователя.",
                                          true));
            _commands.Add(new CommandData("/completetask", 
                                          UserItemsCompleteCommand, 
                                          "Перевести состояние задачи пользователя в выполненное.", 
                                          true));            
            _commands.Add(new CommandData("/showactivetasks",
                                          UserItemsShowActiveCommand,
                                          "Список активных задач пользователя.",
                                          true));
            _commands.Add(new CommandData("/showalltasks",
                                          UserItemsShowAllCommand,
                                          "Список всех задач пользователя.",
                                          true));
            _commands.Add(new CommandData("/taskmaxlength", 
                                          UserItemsSetMaxLengthCommand, 
                                          "Максимально допустимая длина задачи.", 
                                          true));
            _commands.Add(new CommandData("/tasksmaxnumber", 
                                          UserItemsSetMaxNumberCommand, 
                                          "Максимально допустимое количество задач.", 
                                          true));
        }

        public void HandleUpdateAsync(ITelegramBotClient botClient, Update update)
        {
            Message botMessage = update.Message;

            try
            {
                if (string.IsNullOrEmpty(botMessage.Text))
                {
                    return;
                }

                string[] args = update.Message.Text.Split(' ');
                CommandData command = _commands.Find( command => command.code == args[0]);

                if (command.Equals(default(CommandData)))
                {
                    botClient.SendMessage(botMessage.Chat, $"Команда '{args[0]}' не поддерживается. Введите другую.");
                    return;
                }

                if (command.code.Length == 0)
                {
                    throw new Exception("Отсутствует объект команды: " + args[0]);
                }

                if (command.isUsers && !IsEnabled(botMessage.From.Id))
                {
                    throw new Exception($"Команда '{command.code}' недоступна.");
                }

                command.execute(botClient, botMessage);
            }
            catch (Exception exception)
            {
                ProcessException(botClient, botMessage.Chat, exception);
            }
        }

        private void StartCommand(ITelegramBotClient botClient, Message botMessage)
        {
            string userName = "_";
            if (botMessage.From.Username != null)
            {
                userName = botMessage.From.Username;
            }

            ToDoUser? toDoUser = _userService.RegisterUser(botMessage.From.Id, userName);

            if (toDoUser == null)
            {
                botClient.SendMessage(botMessage.Chat, $"Здравствуйте, {userName}. Вы не зарегистрированы.");
            }
            else
            {
                botClient.SendMessage(botMessage.Chat, $"{toDoUser.TelegramUserName}, Вы зарегистрированы.");
            }
        }

        private void InfoCommand(ITelegramBotClient botClient, Message botMessage)
        {
            botClient.SendMessage(botMessage.Chat, $"Version: {_version.ToString()}");
            botClient.SendMessage(botMessage.Chat, $"Creation date: {_creationDate.ToString("dd.MM.yyyy")}");
        }

        private void EndCommand(ITelegramBotClient botClient, Message botMessage)
        {
            StringBuilder str = new();
            str.Append("Команда окончания, которая ничего не делает.");

            botClient.SendMessage(botMessage.Chat, str.ToString());
        }
        private void HelpCommand(ITelegramBotClient botClient, Message botMessage)
        {
            StringBuilder str = new();
            str.AppendLine("Краткая информация о программе:");

            foreach (CommandData command in _commands)
            {
                // пропустить неактивные команды
                if (command.isUsers && !IsEnabled(botMessage.From.Id))
                {
                    continue;
                }

                str.Append(command.code);
                str.Append(" - ");
                str.AppendLine(command.help);
            }

            botClient.SendMessage(botMessage.Chat, str.ToString());
        }
        private void UserItemsAddCommand(ITelegramBotClient botClient, Message botMessage)
        {
            string errorMessage;
            ToDoUser? toDoUser = GetToDoUser(botMessage.From.Id, out errorMessage);

            if (toDoUser == null)
            {
                botClient.SendMessage(botMessage.Chat, "Ошибка: " + errorMessage);

                return;
            }

            List<string> args = GetArguments(botMessage.Text);

            if (args.Count == 0)
            {
                botClient.SendMessage(botMessage.Chat, "Введите название задачи.");
                return;
            }

            string commandName = string.Join(" ", args);

            ToDoItem? newItem = _toDoService.Add(toDoUser.UserId, string.Join(" ", args));
            if (newItem != null)
            {
                botClient.SendMessage(botMessage.Chat, $"Задача '{newItem.Name}' добавлена.");
            }
            else
            {
                botClient.SendMessage(botMessage.Chat, "Задача не добавлена.");
            }
        }
        private void UserItemsCompleteCommand(ITelegramBotClient botClient, Message botMessage)
        {
            string errorMessage;
            ToDoUser? toDoUser = GetToDoUser(botMessage.From.Id, out errorMessage);

            if (toDoUser == null)
            {
                botClient.SendMessage(botMessage.Chat, "Ошибка: " + errorMessage);

                return;
            }

            List<string> args = GetArguments(botMessage.Text);
            if (args.Count == 0)
            {
                botClient.SendMessage(botMessage.Chat, "Введите Guid команды, которую нужно удалить.");
                return;
            }

            Guid guid;
            if (!Guid.TryParse(args.ElementAt(0), out guid))
            {
                botClient.SendMessage(botMessage.Chat, "Guid команды неверный.");
                return;
            }

            if (_toDoService.MarkCompleted(toDoUser.UserId, guid))
            {
                botClient.SendMessage(botMessage.Chat, "Команда отмечена как выполненная.");
            }
            else
            {
                botClient.SendMessage(botMessage.Chat, "Команда не отмечена как выполненная.");
            }
        }
        private void UserItemsRemoveCommand(ITelegramBotClient botClient, Message botMessage)
        {
            string errorMessage;
            ToDoUser? toDoUser = GetToDoUser(botMessage.From.Id, out errorMessage);

            if (toDoUser == null)
            {
                botClient.SendMessage(botMessage.Chat, "Ошибка: " + errorMessage);

                return;
            }

            List<string> args = GetArguments(botMessage.Text);
            if (args.Count == 0)
            {
                botClient.SendMessage(botMessage.Chat, "Введите Guid команды, которую нужно удалить.");
                return;
            }

            Guid guid;
            if (!Guid.TryParse(args.ElementAt(0), out guid))
            {
                botClient.SendMessage(botMessage.Chat, "Guid команды неверный.");
                return;
            }

            if (_toDoService.Delete(toDoUser.UserId, guid))
            {
                botClient.SendMessage(botMessage.Chat, "Команда удалена.");
            }
            else
            {
                botClient.SendMessage(botMessage.Chat, "Команда не удалена.");
            }
        }
        private void UserItemsSetMaxLengthCommand(ITelegramBotClient botClient, Message botMessage)
        {
            string errorMessage;
            ToDoUser? toDoUser = GetToDoUser(botMessage.From.Id, out errorMessage);

            if (toDoUser == null)
            {
                botClient.SendMessage(botMessage.Chat, "Ошибка: " + errorMessage);

                return;
            }

            int minLength = 50;
            List<string> args = GetArguments(botMessage.Text);
            if (args.Count == 0)
            {
                botClient.SendMessage(botMessage.Chat, $"Введите максимально допустимую длину задачи (от {minLength}).");
                return;
            }

            short length = -1;
            if (!short.TryParse(args.ElementAt(0), out length))
            {
                botClient.SendMessage(botMessage.Chat, $"Введите максимально допустимую длину задачи (от {minLength}).");
                return;
            }

            if (length < minLength)
            {
                botClient.SendMessage(botMessage.Chat, $"Введите максимально допустимую длину задачи (от {minLength}).");
                return;
            }

            var toDoService = (ToDoService)_toDoService;
            if (toDoService != null)
            {
                toDoService.SetMaxLength(toDoUser.UserId, length);

                botClient.SendMessage(botMessage.Chat, $"Установлена максимальная длина задачи: {length}.");
            }
        }
        private void UserItemsSetMaxNumberCommand(ITelegramBotClient botClient, Message botMessage)
        {
            string errorMessage;
            ToDoUser? toDoUser = GetToDoUser(botMessage.From.Id, out errorMessage);

            if (toDoUser == null)
            {
                botClient.SendMessage(botMessage.Chat, "Ошибка: " + errorMessage);

                return;
            }

            int minNumber = 1;
            int maxNumber = 100;

            List<string> args = GetArguments(botMessage.Text);
            if (args.Count == 0)
            {
                botClient.SendMessage(botMessage.Chat,
                                      $"Введите максимально допустимое количество задач в пределе [{minNumber}, {maxNumber}].");
                return;
            }

            short number = -1;
            if (!short.TryParse(args.ElementAt(0), out number))
            {
                botClient.SendMessage(botMessage.Chat,
                                      $"Введите максимально допустимое количество задач в пределе [{minNumber}, {maxNumber}].");
                return;
            }

            if (number < minNumber || number > maxNumber)
            {
                botClient.SendMessage(botMessage.Chat, $"Введите максимально допустимое количество задач в пределе [{minNumber}, {maxNumber}].");
                return;
            }

            var toDoService = (ToDoService)_toDoService;
            if (toDoService != null)
            {
                toDoService.SetMaxNumber(toDoUser.UserId, number);

                botClient.SendMessage(botMessage.Chat, $"Установлено максимально допустимое количество задач: {number}.");
            }
        }
        private void UserItemsShowActiveCommand(ITelegramBotClient botClient, Message botMessage)
        {
            string errorMessage;
            ToDoUser? toDoUser = GetToDoUser(botMessage.From.Id, out errorMessage);

            if (toDoUser == null)
            {
                botClient.SendMessage(botMessage.Chat, "Ошибка: " + errorMessage);

                return;
            }

            var userCommands = _toDoService.GetActiveByUserId(toDoUser.UserId);

            if (userCommands.Count == 0)
            {
                botClient.SendMessage(botMessage.Chat, "Список задач пуст.");

                return;
            }

            StringBuilder str = new();
            for (int i = 0; i < userCommands.Count; ++i)
            {
                ToDoItem item = userCommands.ElementAt(i);
                if (item != null)
                {
                    str.AppendLine($"{i + 1}. {item.ToString()}");
                }
            }

            botClient.SendMessage(botMessage.Chat, str.ToString());
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
        private void UserItemsShowAllCommand(ITelegramBotClient botClient, Message botMessage)
        {
            string errorMessage;
            ToDoUser? toDoUser = GetToDoUser(botMessage.From.Id, out errorMessage);

            if (toDoUser == null)
            {
                botClient.SendMessage(botMessage.Chat, "Ошибка: " + errorMessage);

                return;
            }

            var userCommands = _toDoService.GetAllByUserId(toDoUser.UserId);

            if (userCommands.Count == 0)
            {
                botClient.SendMessage(botMessage.Chat, "Список задач пуст.");

                return;
            }

            StringBuilder str = new();
            for (int i = 0; i < userCommands.Count; ++i)
            {
                ToDoItem item = userCommands.ElementAt(i);
                if (item != null)
                {
                    str.AppendLine($" ({item.GetStateName()}) {item.ToString()}");
                }
            }

            botClient.SendMessage(botMessage.Chat, str.ToString());
        }

        private static void ShowException(ITelegramBotClient botClient, Chat botChat, string message)
        {
            botClient.SendMessage(botChat, message);
        }
        private List<string> GetArguments(string line)
        {
            List<string> argsList = new();

            string[] args = line.Split(' ');
            if (args.Length > 1)
            {
                for (int i = 1; i < args.Length; ++i)
                {
                    argsList.Add(args[i]);
                }
            }

            return argsList;
        }
        private ToDoUser? GetToDoUser(long telegramUserId, out string errorMessage)
        {
            errorMessage = "";

            if (_userService == null || !IsEnabled(telegramUserId))
            {
                errorMessage = "Команда не доступна.";
                return null;
            }

            ToDoUser? toDoUser = _userService.GetUser(telegramUserId);

            if (toDoUser == null)
            {
                errorMessage = "Пользователь не найден.";
                return null;
            }

            return toDoUser;
        }
        private bool IsEnabled(long telegramUserId)
        {
            if (_userService == null)
            {
                return false;
            }

            return (_userService.GetUser(telegramUserId) != null);
        }
    }
}
