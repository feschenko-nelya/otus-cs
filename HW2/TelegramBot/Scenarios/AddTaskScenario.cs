using Core.Entity;
using Core.Services;
using HW2.TelegramBot.Scenario;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

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
            return scenario == ScenarioType.AddTask;
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
                        ReplyKeyboardMarkup cancelKeyboard = new();
                        cancelKeyboard.ResizeKeyboard = true;
                        cancelKeyboard.Keyboard = [[new KeyboardButton("/cancel")]];

                        context.Data[toDoUser.TelegramUserId.ToString()] = toDoUser;
                        
                        await bot.SendMessage(update.Message.Chat.Id, 
                                              "Введите название задачи:", 
                                              cancellationToken: ct,
                                              replyMarkup: cancelKeyboard);
                        
                        context.CurrentStep = "Name";

                        return ScenarioResult.Transition;
                    }
                    break;
                }
                case "Name":
                {
                    if (update.Message == null || update.Message.From == null || update.Message.From.Username == null)
                        break;
                    
                    object? toDoUserObject = null;
                    context.Data.TryGetValue(update.Message.From.Id.ToString(), out toDoUserObject);

                    if (toDoUserObject == null)
                        break;

                    ToDoUser? toDoUser = (ToDoUser)toDoUserObject;
                    if (toDoUser == null)
                        break;
                            
                    ToDoItem newToDoItem = await _toDoService.Add(toDoUser.UserId, update.Message.Text, null, null, ct);
                    context.Data[update.Message.From.Id.ToString()] = newToDoItem;

                    await bot.SendMessage(update.Message.Chat.Id, "Введите срок задачи (dd.MM.yyyy):", cancellationToken: ct);
                    context.CurrentStep = "Deadline";

                    return ScenarioResult.Transition;
                }
                case "Deadline":
                {
                    if (update.Message == null || update.Message.From == null || update.Message.From.Username == null)
                        break;

                    object? toDoItemObject = null;
                    context.Data.TryGetValue(update.Message.From.Id.ToString(), out toDoItemObject);

                    if (toDoItemObject == null)
                        break;

                    ToDoItem? toDoItem = (ToDoItem?)toDoItemObject;
                    if (toDoItem == null)
                        break;

                    DateTime? deadline = null;
                    try
                    {
                            deadline = DateTime.ParseExact(update.Message.Text, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        await bot.SendMessage(update.Message.Chat.Id, "Дата введена неверно. Введите срок задачи (dd.MM.yyyy):", cancellationToken: ct);
                        context.CurrentStep = "Deadline";
                        return ScenarioResult.Transition;
                    }

                    await _toDoService.Delete(toDoItem.UserId, toDoItem.Id, ct);
                    await _toDoService.Add(toDoItem.UserId, toDoItem.Name, deadline, null, ct);

                    await bot.SendMessage(update.Message.Chat.Id, $"Задача {toDoItem.Name} добавлена", cancellationToken: ct);

                    return ScenarioResult.Completed;
                }
            }

            return ScenarioResult.Completed;
        }
    }
}
