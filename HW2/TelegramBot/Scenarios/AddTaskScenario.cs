using Core.Entity;
using Core.Services;
using HW2.TelegramBot.Scenario;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace HW2.TelegramBot.Scenarios
{
    internal class AddTaskScenario : IScenario
    {
        private readonly IUserService _userService;
        private readonly IToDoService _toDoService;
        internal AddTaskScenario(IUserService userService, IToDoService toDoService)
        {
            _userService = userService;
            _toDoService = toDoService;
        }
        public bool CanHandle(ScenarioType scenario)
        {
            throw new NotImplementedException();
        }

        public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient bot, ScenarioContext context, Update update, CancellationToken ct)
        {
            switch (context.CurrentStep)
            {
                case null:
                {
                    ToDoUser? toDoUser = await _userService.GetUser(context.UserId, ct);
                    if (toDoUser != null && update.Message != null)
                    {
                        context.Data.Add(toDoUser.TelegramUserId.ToString(), toDoUser);
                        await bot.SendMessage(update.Message.Chat.Id, "Введите название задачи:", cancellationToken: ct);
                        context.CurrentStep = "Name";

                        return ScenarioResult.Transition;
                    }
                    break;
                }
                case "Name":
                {
                    if (update.Message != null && update.Message.From != null && update.Message.From.Username != null)
                    {
                        object? toDoUserObject = null;
                        context.Data.TryGetValue(update.Message.From.Id.ToString(), out toDoUserObject);

                        if (toDoUserObject != null)
                        {
                            ToDoUser? toDoUser = (ToDoUser)toDoUserObject;
                            if (toDoUser != null)
                            {
                                ToDoItem newToDoItem = await _toDoService.Add(toDoUser.UserId, update.Message.Text, null, ct);
                                context.Data.Add(newToDoItem.Id.ToString(), newToDoItem);
                                await bot.SendMessage(update.Message.Chat.Id, "Введите срок задачи (dd.MM.yyyy):", cancellationToken: ct);
                                context.CurrentStep = "Deadline";
                                return ScenarioResult.Transition;
                            }
                        }
                    }
                    break;
                }
                case "Deadline":
                {
                    if (update.Message != null && update.Message.From != null && update.Message.From.Username != null)
                    {
                        object? toDoUserObject = null;
                        context.Data.TryGetValue(update.Message.From.Username, out toDoUserObject);

                        if (toDoUserObject == null)
                            break;
                        
                        ToDoUser? toDoUser = (ToDoUser)toDoUserObject;
                        if (toDoUser == null)
                            break;

                        object? toDoItemObject = null;
                        context.Data.TryGetValue(update.Message.From.Username, out toDoItemObject);

                        ToDoItem? toDoItem = (ToDoItem)toDoItemObject;
                        if (toDoItem == null)
                            break;

                        DateTime? deadline = null;
                        try
                        {
                            DateTime.ParseExact(update.Message.Text, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            await bot.SendMessage(update.Message.Chat.Id, "Дата введена неверно. Введите срок задачи (dd.MM.yyyy):", cancellationToken: ct);
                            context.CurrentStep = "Deadline";
                            return ScenarioResult.Transition;
                        }

                        await _toDoService.Delete(toDoUser.UserId, toDoItem.Id, ct);
                        await _toDoService.Add(toDoUser.UserId, toDoItem.Name, deadline, ct);

                        return ScenarioResult.Completed;
                        
                    }
                    break;
                }
            }

            return ScenarioResult.Completed;
        }
    }
}
