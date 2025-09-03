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
                    if (update.Message == null)
                        break;

                    ReplyKeyboardMarkup cancelKeyboard = new();
                    cancelKeyboard.ResizeKeyboard = true;
                    cancelKeyboard.Keyboard = [[new KeyboardButton("/cancel")]];
                        
                    await bot.SendMessage(update.Message.Chat.Id, 
                                            "Введите название задачи:", 
                                            cancellationToken: ct,
                                            replyMarkup: cancelKeyboard);
                        
                    context.CurrentStep = "Name";

                    return ScenarioResult.Transition;
                }
                case "Name":
                {
                    Message? botMessage = update.Message;
                    if (botMessage == null || botMessage.From == null || botMessage.From.Username == null)
                        break;

                    if (string.IsNullOrEmpty(botMessage.Text))
                    {
                        context.CurrentStep = null;
                        return ScenarioResult.Transition;
                    }

                    context.Data[context.CurrentStep] = botMessage.Text;

                    await bot.SendMessage(botMessage.Chat.Id, "Введите срок задачи (dd.MM.yyyy):", cancellationToken: ct);
                    context.CurrentStep = "Deadline";

                    return ScenarioResult.Transition;
                }
                case "Deadline":
                {
                    Message? botMessage = update.Message;
                    if (botMessage == null || botMessage.From == null || botMessage.From.Username == null)
                        break;

                    DateTime? deadline = null;
                    try
                    {
                            deadline = DateTime.ParseExact(update.Message.Text, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        await bot.SendMessage(botMessage.Chat.Id, "Дата введена неверно. Введите срок задачи (dd.MM.yyyy):", cancellationToken: ct);
                        context.CurrentStep = "Deadline";
                        return ScenarioResult.Transition;
                    }

                    string? toDoItemName = context.Data["Name"].ToString();
                    if (string.IsNullOrEmpty(toDoItemName))
                        throw new Exception("Название задачи не сохранилось.");

                    ToDoUser? toDoUser = await _userService.GetUser(context.UserId, ct);
                    if (toDoUser == null)
                        throw new Exception("Объект пользователя не найден.");

                    await _toDoService.Add(toDoUser.UserId, toDoItemName, deadline, ct);

                    await bot.SendMessage(update.Message.Chat.Id, $"Задача {toDoItemName} добавлена", cancellationToken: ct);

                    return ScenarioResult.Completed;
                }
            }

            return ScenarioResult.Completed;
        }
    }
}
