using System.Text;
using Core.Entity;
using Core.Services;
using HW2.Core.Services;
using HW2.Infrastructure.Services;
using HW2.TelegramBot.Dto;
using HW2.TelegramBot.Scenario;
using HW2.TelegramBot.Scenarios;
using Infrastructure.DataAccess;
using Infrastructure.Services;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Net.Mime.MediaTypeNames;

namespace HW2
{
    internal class UpdateHandler : IUpdateHandler
    {
        private readonly IUserService _userService;
        private readonly IToDoService _toDoService;
        private readonly IToDoListService _toDoListService;
        private readonly IEnumerable<IScenario> _scenarios;
        private readonly IScenarioContextRepository _contextRepository;

        private static readonly Version _version = new Version(1, 0);
        private static readonly DateTime _creationDate = new DateTime(2025, 3, 27);
        private List<Chat> _chats = new();

        delegate Task CommandExecutionHandler(ITelegramBotClient botClient, Update update, CancellationToken ct);
        struct CommandData
        {
            public string code;
            public CommandExecutionHandler execute;
            public string help;
            public bool isUsers = false;

            public CommandData(string code, CommandExecutionHandler command, string help, bool isUsers = false)
            {
                this.code = code;
                this.help = help;
                this.execute = command;
                this.isUsers = isUsers;
            }
        }
        List<CommandData> _commands = new();

        public delegate void MessageEventHandler(string message);
        event MessageEventHandler OnHandleUpdateStarted = delegate { };
        event MessageEventHandler OnHandleUpdateCompleted = delegate { };

        public UpdateHandler(IUserService userService, IToDoService toDoService, IToDoListService toDoListService, 
                             IEnumerable<IScenario> scenarios, IScenarioContextRepository contextRepository)
        {
            _userService = userService;
            _toDoService = toDoService;
            _toDoListService = toDoListService;
            _scenarios = scenarios;
            _contextRepository = contextRepository;

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
            _commands.Add(new CommandData("/show",
                                          UserItemsShowCommand,
                                          "Список активных задач пользователя.",
                                          true));
            _commands.Add(new CommandData("/taskmaxlength", 
                                          UserItemsSetMaxLengthCommand, 
                                          "Максимально допустимая длина задачи.", 
                                          true));
            _commands.Add(new CommandData("/tasksmaxnumber", 
                                          UserItemsSetMaxNumberCommand, 
                                          "Максимально допустимое количество задач.", 
                                          true));
            _commands.Add(new CommandData("/report",
                                          UserItemsReportCommand,
                                          "Отчет по задачам.",
                                          true));
            _commands.Add(new CommandData("/find",
                                          UserItemsFindCommand,
                                          "Все задачи пользователя, которые начинаются на введенный текст.",
                                          true));
            _commands.Add(new CommandData("/cancel",
                                          CancelCommand,
                                          "Для остановки сценариев.",
                                          true));
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (update is null)
            {
                return;
            }

            try
            {
                if (ct.IsCancellationRequested)
                {
                    Message botMessage = update.Message;
                    if (botMessage is null)
                        return;

                    await botClient.SendMessage(botMessage.Chat, "", 
                                                replyMarkup: new ReplyKeyboardRemove());
                }

                switch (update.Type)
                {
                case UpdateType.Message:
                {
                    await BotOnMessageReceived(botClient, update, ct);
                    return;
                }
                case UpdateType.CallbackQuery:
                {
                    await OnCallbackQuery(botClient, update, ct);
                    return;
                }
                }
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, HandleErrorSource.FatalError, ct);
            }
        }

        private async Task OnCallbackQuery(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (update == null)
                return;

            var query = update.CallbackQuery;
            if (query == null)
                return;

            await botClient.AnswerCallbackQuery(query.Id, "");

            if (string.IsNullOrEmpty(query.Data))
                return;

            User? user = query.From;
            if (user == null)
                return;

            if (!await IsEnabled(user.Id, ct))
                return;

            var commonCallback = CallbackDto.FromString(query.Data);

            if (commonCallback.Action == "show")
            {
                ToDoUser? toDoUser = await _userService.GetUser(user.Id, ct);
                if (toDoUser == null)
                    return;

                var toDoListCallback = ToDoListCallbackDto.FromString(query.Data);

                var items = await _toDoService.GetByUserIdAndList(toDoUser.UserId, toDoListCallback.ToDoListId, ct);

                StringBuilder str = new();
                for (int i = 0; i < items.Count; ++i)
                {
                    ToDoItem item = items.ElementAt(i);
                    if (item != null)
                    {
                        str.AppendLine($"{i + 1}. {item.ToString()}");
                    }
                }

                Message? botMessage = query.Message;
                if (botMessage == null)
                    return;

                await botClient.SendMessage(botMessage.Chat, str.ToString(), cancellationToken: ct);
            }
            else if (commonCallback.Action == "addlist")
            {
                ScenarioContext addScenario = new(ScenarioType.AddList, user.Id);
                await ProcessScenario(botClient, addScenario, update, ct);
            }
        }

        private async Task BotOnMessageReceived(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (update == null)
            {
                return;
            }

            Message botMessage = update.Message;
            if (botMessage is null)
            {
                return;
            }

            if (botMessage.Type != MessageType.Text)
            {
                return;
            }

            if (botMessage.From == null)
                return;

            string? botMessageText = botMessage.Text;

            if (string.IsNullOrEmpty(botMessageText))
            {
                return;
            }

            OnHandleUpdateStarted.Invoke(botMessageText);

            if (string.IsNullOrEmpty(botMessageText))
            {
                OnHandleUpdateCompleted.Invoke(botMessageText);
                return;
            }

            string[] args = botMessageText.Split(' ');
            CommandData command = _commands.Find(command => command.code == args[0]);

            if (command.code == "/cancel")
            {
                await CancelCommand(botClient, update, ct);
                return;
            }

            ScenarioContext? scenarioContext = await _contextRepository.GetContext(botMessage.From.Id, ct);
            if (scenarioContext != null)
            {
                await ProcessScenario(botClient, scenarioContext, update, ct);
                return;
            }

            if (command.Equals(default(CommandData)))
            {
                await botClient.SendMessage(botMessage.Chat, $"Команда '{args[0]}' не поддерживается. Введите другую.", cancellationToken: ct);

                OnHandleUpdateCompleted.Invoke(botMessageText);

                return;
            }

            if (command.code.Length == 0)
            {
                await HandleErrorAsync(botClient, new Exception("Отсутствует объект команды: " + args[0]), HandleErrorSource.HandleUpdateError, ct);

                OnHandleUpdateCompleted.Invoke(botMessageText);

                return;
            }

            if (botMessage.From == null)
            {
                OnHandleUpdateCompleted.Invoke(botMessageText);
                return;
            }

            if (command.isUsers && !await IsEnabled(botMessage.From.Id, ct))
            {
                await HandleErrorAsync(botClient, new Exception($"Команда '{command.code}' недоступна."), HandleErrorSource.HandleUpdateError, ct);

                OnHandleUpdateCompleted.Invoke(botMessageText);

                return;
            }

            await command.execute(botClient, update, ct);

            OnHandleUpdateCompleted.Invoke(botMessageText);
        }

        private async Task StartCommand(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (update == null)
                return;

            Message botMessage = update.Message;
            if (botMessage == null)
                return;

            if (botMessage.From == null)
            {
                return;
            }

            string userName = "_";
            if (botMessage.From.Username != null)
            {
                userName = botMessage.From.Username;
            }

            Task<ToDoUser>? toDoUser = _userService.RegisterUser(botMessage.From.Id, userName, ct);

            if (toDoUser == null)
            {
                await botClient.SendMessage(botMessage.Chat, $"Здравствуйте, {userName}. Вы не зарегистрированы.", cancellationToken: ct);
            }
            else
            {
                await botClient.SetMyCommands(GetBotCommands(botMessage.From.Id, ct));

                var keyboard = await GetReplyKeyboardMarkup(botMessage.From.Id, ct);

                await botClient.SendMessage(botMessage.Chat, $"{toDoUser.Result.TelegramUserName}, Вы зарегистрированы.",
                                            replyMarkup: keyboard, cancellationToken: ct);

                if (_chats.Find(chat => chat.Id == botMessage.Chat.Id ) == null)
                {
                    _chats.Add(botMessage.Chat);
                }
            }
        }

        public async Task RemoveKeyboard(ITelegramBotClient botClient)
        {
            foreach (var chat in _chats)
            {
                await botClient.SendMessage(chat.Id, "Бот завершил работу", replyMarkup: new ReplyKeyboardRemove());
            }
        }

        private async Task<ReplyKeyboardMarkup> GetReplyKeyboardMarkup(long telegramUserId, CancellationToken ct)
        {
            var replyCommands = _commands.FindAll(c => 
                                    c.code == "/addtask"
                                    || c.code == "/show"
                                    || c.code == "/report"
                                );

            ReplyKeyboardMarkup keyboard = new();
            keyboard.ResizeKeyboard = true;

            foreach (CommandData command in replyCommands)
            {
                if (command.isUsers && !await IsEnabled(telegramUserId, ct))
                {
                    continue;
                }

                keyboard.AddNewRow(new KeyboardButton(command.code));
            }

            if (replyCommands.Count == 0)
            {
                keyboard.AddButton("/start");
            }
            
            return keyboard;
        }

        public List<BotCommand> GetBotCommands(long telegramUserId, CancellationToken ct)
        {
            List<BotCommand> botCommands = new();
            
            foreach (CommandData command in _commands)
            {
                if (command.isUsers && !IsEnabled(telegramUserId, ct).Result)
                {
                    continue;
                }

                botCommands.Add(new BotCommand(command.code, command.help));
            }

            return botCommands;
        }

        private async Task InfoCommand(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (update == null)
                return;
            
            Message botMessage = update.Message;
            if (botMessage == null) 
                return;

            await botClient.SendMessage(botMessage.Chat, 
                $"""
                Version: {_version.ToString()}
                Creation date: {_creationDate.ToString("dd.MM.yyyy")}
                """,
                cancellationToken: ct);
        }

        private async Task EndCommand(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (update == null)
                return;

            Message botMessage = update.Message;
            if (botMessage == null)
                return;

            StringBuilder str = new();
            str.Append("Команда окончания, которая ничего не делает.");

            await botClient.SendMessage(botMessage.Chat, str.ToString(), cancellationToken: ct, replyMarkup: new ReplyKeyboardRemove());
        }
        private async Task HelpCommand(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (update == null)
                return;

            Message botMessage = update.Message;
            if (botMessage == null)
                return;

            StringBuilder str = new();
            str.AppendLine("Краткая информация о программе:");

            foreach (CommandData command in _commands)
            {
                // пропустить неактивные команды
                if (command.isUsers && !await IsEnabled(botMessage.From.Id, ct))
                {
                    continue;
                }

                str.Append(command.code);
                str.Append(" - ");
                str.AppendLine(command.help);
            }

            await botClient.SendMessage(botMessage.Chat, str.ToString(), cancellationToken: ct);
        }
        private async Task UserItemsAddCommand(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (update == null)
                return;

            Message botMessage = update.Message;
            if (botMessage == null)
                return;

            var user = await GetToDoUser(botMessage.From.Id, ct);

            if (user.obj == null)
            {
                await botClient.SendMessage(botMessage.Chat, "Ошибка: " + user.error, cancellationToken: ct);

                return;
            }

            ScenarioContext addScenario = new(ScenarioType.AddTask, user.obj.TelegramUserId);
            await ProcessScenario(botClient, addScenario, update, ct);
        }
        private async Task UserItemsCompleteCommand(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (update == null)
                return;

            Message botMessage = update.Message;
            if (botMessage == null)
                return;

            var user = await GetToDoUser(botMessage.From.Id, ct);

            if (user.obj == null)
            {
                await botClient.SendMessage(botMessage.Chat, "Ошибка: " + user.error, cancellationToken: ct);

                return;
            }

            List<string> args = GetCommandArguments(botMessage.Text);
            if (args.Count == 0)
            {
                await botClient.SendMessage(botMessage.Chat, "Введите Guid команды, которую нужно удалить.", cancellationToken: ct);
                return;
            }

            Guid guid;
            if (!Guid.TryParse(args.ElementAt(0), out guid))
            {
                await botClient.SendMessage(botMessage.Chat, "Guid команды неверный.", cancellationToken: ct);
                return;
            }

            if (await _toDoService.MarkCompleted(user.obj.UserId, guid, ct))
            {
                await botClient.SendMessage(botMessage.Chat, "Команда отмечена как выполненная.", cancellationToken: ct);
            }
            else
            {
                await botClient.SendMessage(botMessage.Chat, "Команда не отмечена как выполненная.", cancellationToken: ct);
            }
        }
        private async Task UserItemsRemoveCommand(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (update == null)
                return;

            Message botMessage = update.Message;
            if (botMessage == null)
                return;

            var user = await GetToDoUser(botMessage.From.Id, ct);

            if (user.obj == null)
            {
                await botClient.SendMessage(botMessage.Chat, "Ошибка: " + user.error, cancellationToken: ct);

                return;
            }

            List<string> args = GetCommandArguments(botMessage.Text);
            if (args.Count == 0)
            {
                await botClient.SendMessage(botMessage.Chat, "Введите Guid команды, которую нужно удалить.", cancellationToken: ct);
                return;
            }

            Guid guid;
            if (!Guid.TryParse(args.ElementAt(0), out guid))
            {
                await botClient.SendMessage(botMessage.Chat, "Guid команды неверный.", cancellationToken: ct);
                return;
            }

            if (await _toDoService.Delete(user.obj.UserId, guid, ct))
            {
                await botClient.SendMessage(botMessage.Chat, "Команда удалена.", cancellationToken: ct);
            }
            else
            {
                await botClient.SendMessage(botMessage.Chat, "Команда не удалена.", cancellationToken: ct);
            }
        }
        private async Task UserItemsSetMaxLengthCommand(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (update == null)
                return;

            Message botMessage = update.Message;
            if (botMessage == null)
                return;

            var user = await GetToDoUser(botMessage.From.Id, ct);

            if (user.obj == null)
            {
                await botClient.SendMessage(botMessage.Chat, "Ошибка: " + user.error, cancellationToken: ct);

                return;
            }

            int minLength = 50;
            List<string> args = GetCommandArguments(botMessage.Text);
            if (args.Count == 0)
            {
                await botClient.SendMessage(botMessage.Chat, $"Введите максимально допустимую длину задачи (от {minLength}).", cancellationToken: ct);
                return;
            }

            short length = -1;
            if (!short.TryParse(args.ElementAt(0), out length))
            {
                await botClient.SendMessage(botMessage.Chat, $"Введите максимально допустимую длину задачи (от {minLength}).", cancellationToken: ct);
                return;
            }

            if (length < minLength)
            {
                await botClient.SendMessage(botMessage.Chat, $"Введите максимально допустимую длину задачи (от {minLength}).", cancellationToken: ct);
                return;
            }

            var toDoService = (ToDoService)_toDoService;
            if (toDoService != null)
            {
                toDoService.SetMaxLength(user.obj.UserId, length);

                await botClient.SendMessage(botMessage.Chat, $"Установлена максимальная длина задачи: {length}.", cancellationToken: ct);
            }
        }
        private async Task UserItemsSetMaxNumberCommand(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (update == null)
                return;

            Message botMessage = update.Message;
            if (botMessage == null)
                return;

            var user = await GetToDoUser(botMessage.From.Id, ct);

            if (user.obj == null)
            {
                await botClient.SendMessage(botMessage.Chat, "Ошибка: " + user.error, cancellationToken: ct);

                return;
            }

            int minNumber = 1;
            int maxNumber = 100;

            List<string> args = GetCommandArguments(botMessage.Text);
            if (args.Count == 0)
            {
                await botClient.SendMessage(botMessage.Chat,
                                            $"Введите максимально допустимое количество задач в пределе [{minNumber}, {maxNumber}].", cancellationToken: ct);
                return;
            }

            short number = -1;
            if (!short.TryParse(args.ElementAt(0), out number))
            {
                await botClient.SendMessage(botMessage.Chat,
                                            $"Введите максимально допустимое количество задач в пределе [{minNumber}, {maxNumber}].", cancellationToken: ct);
                return;
            }

            if (number < minNumber || number > maxNumber)
            {
                await botClient.SendMessage(botMessage.Chat, $"Введите максимально допустимое количество задач в пределе [{minNumber}, {maxNumber}].", cancellationToken: ct);
                return;
            }

            var toDoService = (ToDoService)_toDoService;
            if (toDoService != null)
            {
                toDoService.SetMaxNumber(user.obj.UserId, number);

                await botClient.SendMessage(botMessage.Chat, $"Установлено максимально допустимое количество задач: {number}.", cancellationToken: ct);
            }
        }
        private async Task UserItemsShowCommand(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (update == null)
                return;

            Message botMessage = update.Message;
            if (botMessage == null)
                return;

            var user = await GetToDoUser(botMessage.From.Id, ct);

            if (user.obj == null)
            {
                await botClient.SendMessage(botMessage.Chat, "Ошибка: " + user.error, cancellationToken: ct);

                return;
            }

            await botClient.SendMessage(botMessage.Chat, "Выберите список", cancellationToken: ct,
                                        replyMarkup: new InlineKeyboardMarkup
                                        {
                                            InlineKeyboard = [
                                                                [InlineKeyboardButton.WithCallbackData("\u2754 Без списка", "show|null"), 
                                                                 InlineKeyboardButton.WithCallbackData("\u2728 Со списком", "show|id")],
                                                                [InlineKeyboardButton.WithCallbackData("\u2795 Добавить", "addlist"), 
                                                                 InlineKeyboardButton.WithCallbackData("\u274C Удалить", "deletelist")]
                                                             ]
                                        });
        }
        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource errorSource, CancellationToken ct)
        {
            if ((exception.GetType() == typeof(ArgumentException))
                || (exception.GetType() == typeof(TaskCountLimitException))
                || (exception.GetType() == typeof(TaskLengthLimitException))
                || (exception.GetType() == typeof(DuplicateTaskException))
               )
            {
                Console.WriteLine($"Handle error: {exception.Message}");
            }
            else
            {
                Console.WriteLine($"""
                                            Handle error
                                            Произошла непредвиденная ошибка.
                                            Детальная информация:

                                            Type: {exception.GetType}
                                            Message: {exception.Message}
                                            Stack trace: 
                                            {exception.StackTrace}
                                            Inner exception: {exception.InnerException}
                                            """);
            }

            return Task.CompletedTask;
        }
        private async Task UserItemsReportCommand(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (update == null)
                return;

            Message botMessage = update.Message;
            if (botMessage == null)
                return;

            ToDoService? toDoService = (ToDoService)_toDoService;

            if (toDoService == null)
            {
                return;
            }

            ToDoUser? user = await _userService.GetUser(botMessage.From.Id, ct);

            if (user == null)
            {
                return;
            }

            ToDoReportService report = new(toDoService.toDoRepository);
            
            var reportResult = await report.GetUserStats(user.UserId, ct);

            await botClient.SendMessage(botMessage.Chat, 
                                        $"Статистика по задачам на {reportResult.generatedAt.ToString("dd.MM.yyyy HH:mm:ss")}. " +
                                        $"Всего: {reportResult.total}; Завершенных: {reportResult.completed}; Активных: {reportResult.active};",
                                        cancellationToken: ct);
        }
        public async Task UserItemsFindCommand(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (update == null)
                return;

            Message? botMessage = update.Message;
            if (botMessage == null)
                return;

            var user = await GetToDoUser(botMessage.From.Id, ct);

            if (user.obj == null)
            {
                await botClient.SendMessage(botMessage.Chat, "Ошибка: " + user.error, cancellationToken: ct);

                return;
            }

            List<string> commandArgs = GetCommandArguments(botMessage.Text);
            var userCommands = await _toDoService.Find(user.obj, string.Join(" ", commandArgs), ct);

            await botClient.SendMessage(botMessage.Chat, GetToDoItemsStringList(userCommands), cancellationToken: ct);
        }
        private async Task CancelCommand(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (update == null)
                return;

            Message? botMessage = update.Message;
            if (botMessage == null)
                return;

            User? botUser = botMessage.From;
            if (botUser == null)
                return;
            
            await _contextRepository.ResetContext(botUser.Id, ct);

            await botClient.SendMessage(botMessage.Chat, "Команда отменена", cancellationToken: ct,
                                        replyMarkup: await GetReplyKeyboardMarkup(botUser.Id, ct));
        }
        private List<string> GetCommandArguments(string line)
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
        private async Task<(ToDoUser? obj, string error)> GetToDoUser(long telegramUserId, CancellationToken cancelToken)
        {
            (ToDoUser? obj, string error) result = (obj: null, error: "");

            if (_userService == null || !await IsEnabled(telegramUserId, cancelToken))
            {
                result.Item2 = "Команда не доступна.";
                return result;
            }

            result.Item1 = await _userService.GetUser(telegramUserId, cancelToken);

            if (result.Item1 == null)
            {
                result.Item2 = "Пользователь не найден.";
                return result;
            }

            return result;
        }
        private async Task<bool> IsEnabled(long telegramUserId, CancellationToken cancelToken)
        {
            if (_userService == null)
            {
                return false;
            }

            return (await _userService.GetUser(telegramUserId, cancelToken) != null);
        }
        public void OnStartedSubscribe(MessageEventHandler handler)
        {
            OnHandleUpdateStarted += handler;
        }
        public void OnCompletedSubscribe(MessageEventHandler handler)
        {
            OnHandleUpdateCompleted += handler;
        }
        public void UnsubscribeAll()
        {
            foreach (MessageEventHandler d in OnHandleUpdateStarted.GetInvocationList())
            {
                OnHandleUpdateStarted -= d;
            }

            foreach (MessageEventHandler d in OnHandleUpdateCompleted.GetInvocationList())
            {
                OnHandleUpdateCompleted -= d;
            }
        }
        private string GetToDoItemsStringList(IReadOnlyList<ToDoItem> items)
        {
            StringBuilder str = new();

            if (items.Count == 0)
            {
                str.Append("Список задач пуст.");
            }
            else
            {
                for (int i = 0; i < items.Count; ++i)
                {
                    ToDoItem item = items.ElementAt(i);
                    if (item != null)
                    {
                        str.AppendLine($" ({item.GetStateName()}) {item.ToString()}");
                    }
                }
            }

            return str.ToString();
        }

        public IScenario? GetScenario(ScenarioType scenarioType)
        {
            foreach (var scenario in _scenarios)
            {
                if (scenario.CanHandle(scenarioType))
                {
                    return scenario;
                }
            }

            throw new Exception($"Сценарий с типом '{scenarioType}' не добавлен.");
        }

        public async Task ProcessScenario(ITelegramBotClient botClient, ScenarioContext context, Update update, CancellationToken ct)
        {
            IScenario? scenario = GetScenario(context.CurrentScenario);
            if (scenario == null)
                return;

            ScenarioResult scenarioResult = await scenario.HandleMessageAsync(botClient, context, update, ct);

            Chat? chat = null;

            if (update.Message != null)
            {
                chat = update.Message.Chat;
            }
            else if (update.CallbackQuery != null)
            {
                chat = update.CallbackQuery.Message?.Chat;
            }

            if (chat == null)
                throw new Exception("Чат не обнаружен.");

            if (scenarioResult == ScenarioResult.Completed)
            {
                await _contextRepository.ResetContext(context.UserId, ct);

                await botClient.SendMessage(chat, "Команда завершена", cancellationToken: ct,
                                        replyMarkup: await GetReplyKeyboardMarkup(context.UserId, ct));
            }
            else
            {
                await _contextRepository.SetContext(context.UserId, context, ct);
            }
        }
    }
}
