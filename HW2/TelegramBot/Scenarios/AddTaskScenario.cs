using System;
using Core.Entity;
using Core.Services;
using HW2.Core.Entities;
using HW2.Core.Services;
using HW2.Infrastructure.Services;
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
        private readonly IToDoListService _toDoListService;
        internal AddTaskScenario(IUserService userService, IToDoService toDoService, IToDoListService toDoListService)
        {
            _userService = userService;
            _toDoService = toDoService;
            _toDoListService = toDoListService;
        }
        public bool CanHandle(ScenarioType scenario)
        {
            return scenario == ScenarioType.AddTask;
        }

        public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient botClient, ScenarioContext context, Update update, CancellationToken ct)
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
                        
                    await botClient.SendMessage(update.Message.Chat.Id, 
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

                    await botClient.SendMessage(botMessage.Chat.Id, "Введите срок задачи (dd.MM.yyyy):", cancellationToken: ct);
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
                        deadline = DateTime.ParseExact(botMessage.Text, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        await botClient.SendMessage(botMessage.Chat.Id, "Дата введена неверно. Введите срок задачи (dd.MM.yyyy):", cancellationToken: ct);
                        context.CurrentStep = "Deadline";
                        return ScenarioResult.Transition;
                    }

                    context.Data["Deadline"] = botMessage.Text;

                        ToDoUser? toDoUser = await _userService.GetUser(context.UserId, ct);
                        if (toDoUser == null)
                            throw new Exception("Объект пользователя не найден по id.");

                        InlineKeyboardMarkup listsKeyboard = new();

                        var toDoLists = await _toDoListService.GetUserLists(toDoUser.UserId, ct);

                        if (toDoLists.Count == 0)
                        {
                            context.Data["list"] = String.Empty;

                            context.CurrentStep = "Add";
                            return ScenarioResult.Transition;
                        }

                        foreach (ToDoList toDoList in toDoLists)
                        {
                            if (string.IsNullOrEmpty(toDoList.Name))
                                continue;

                            listsKeyboard.AddNewRow(InlineKeyboardButton.WithCallbackData(toDoList.Name, toDoList.Id.ToString()));
                        }

                        ReplyKeyboardMarkup cancelKeyboard = new();
                        cancelKeyboard.ResizeKeyboard = true;
                        cancelKeyboard.Keyboard = [[new KeyboardButton("/cancel")]];

                        await botClient.SendMessage(botMessage.Chat, "Выберите список:",
                                                    replyMarkup: listsKeyboard, cancellationToken: ct);

                        context.CurrentStep = "Add";
                        return ScenarioResult.Transition;
                }
                case "Add":
                    {
                        CallbackQuery? query = update.CallbackQuery;
                        if (query == null)
                            throw new Exception("Выбор списка не получен");

                        await botClient.AnswerCallbackQuery(query.Id, "");
                        
                        ToDoList? toDoList = null;

                        if (!string.IsNullOrEmpty(query.Data))
                        {
                            Guid id;
                            if (Guid.TryParse(query.Data, out id))
                            {
                                toDoList = await _toDoListService.Get(id, ct);
                            }
                        }

                        string? toDoItemName = context.Data["Name"].ToString();
                        if (string.IsNullOrEmpty(toDoItemName))
                            throw new Exception("Название задачи не сохранилось.");

                        ToDoUser? toDoUser = await _userService.GetUser(context.UserId, ct);
                        if (toDoUser == null)
                            throw new Exception("Объект пользователя не найден.");

                        DateTime deadline;
                        DateTime.TryParse(context.Data["Deadline"].ToString(), out deadline);

                        await _toDoService.Add(toDoUser, toDoItemName, deadline, toDoList, ct);

                        await botClient.SendMessage(query.Message.Chat.Id, $"Задача {toDoItemName} добавлена", cancellationToken: ct);

                        return ScenarioResult.Completed;
                    }
            }

            return ScenarioResult.Completed;
        }
    }
}
