
using Core.Entity;
using Core.Services;
using HW2.Core.Entities;
using HW2.Core.Services;
using HW2.TelegramBot.Dto;
using HW2.TelegramBot.Scenario;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace HW2.TelegramBot.Scenarios
{
    internal class DeleteListScenario : IScenario
    {
        private readonly IUserService _userService;
        private readonly IToDoListService _toDoListService;
        private readonly IToDoService _toDoService;

        public DeleteListScenario(IUserService userService, IToDoListService toDoListService, IToDoService toDoService)
        {
            _userService = userService;
            _toDoListService = toDoListService;
            _toDoService = toDoService;
        }
        public bool CanHandle(ScenarioType scenarioType)
        {
            return scenarioType == ScenarioType.DeleteList;
        }

        public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient botClient, ScenarioContext context, Update update, CancellationToken ct)
        {
            var query = update.CallbackQuery;
            if (query == null)
                throw new Exception("Не обнаружен query.");

            if (query != null)
            {
                await botClient.AnswerCallbackQuery(query.Id, "");
            }

            Message? message = update.Message;

            if (message == null)
                message = query?.Message;

            if (message == null)
                throw new Exception("Не обнаружен объект сообщения.");

            Chat? chat = message.Chat;

            if (chat == null)
                throw new Exception("Не обнаружен чат.");

            switch (context.CurrentStep)
            {
                case null:
                    {
                        ToDoUser? toDoUser = await _userService.GetUser(context.UserId, ct);
                        if (toDoUser == null)
                            throw new Exception("Объект пользователя не найден по id.");

                        InlineKeyboardMarkup listsKeyboard = new();

                        var toDoLists = await _toDoListService.GetUserLists(toDoUser.UserId, ct);

                        if (toDoLists.Count == 0)
                        {
                            await botClient.SendMessage(chat.Id, "Список задач пуст.", cancellationToken: ct);
                            return ScenarioResult.Completed;
                        }

                        foreach (ToDoList toDoList in toDoLists)
                        {
                            if (string.IsNullOrEmpty(toDoList.Name))
                                continue;

                            listsKeyboard.AddNewRow(InlineKeyboardButton.WithCallbackData(toDoList.Name, "deletelist|" + toDoList.Id.ToString()));
                        }

                        ReplyKeyboardMarkup cancelKeyboard = new();
                        cancelKeyboard.ResizeKeyboard = true;
                        cancelKeyboard.Keyboard = [[new KeyboardButton("/cancel")]];

                        await botClient.SendMessage(chat.Id, "Выберите список для удаления:",
                                                    replyMarkup: listsKeyboard, cancellationToken: ct);

                        await botClient.SendMessage(chat.Id, "Для отмены нажмите /cancel",
                                                    replyMarkup: new ReplyKeyboardRemove(), cancellationToken: ct);

                        context.CurrentStep = "Approve";

                        return ScenarioResult.Transition;
                    }
                case "Approve":
                    {
                        if (string.IsNullOrEmpty(query?.Data))
                        {
                            context.CurrentStep = null;
                            return ScenarioResult.Transition;
                        }

                        ToDoListCallbackDto dto = ToDoListCallbackDto.FromString(query?.Data);

                        if (dto.ToDoListId == null)
                        {
                            await botClient.SendMessage(chat.Id,
                                                "Информация о списке утеряна. Удаление отменено.",
                                                cancellationToken: ct);
                            return ScenarioResult.Completed;
                        }

                        ToDoList? toDoList = await _toDoListService.Get((Guid)dto.ToDoListId, ct);
                        if (toDoList == null)
                        {
                            await botClient.SendMessage(chat.Id,
                                                "Информация по списку не найдена. Удаление отменено.",
                                                cancellationToken: ct);
                            return ScenarioResult.Completed;
                        }

                        context.Data["toDoListId"] = toDoList.Id;
                        await botClient.SendMessage(chat.Id,
                                              $"Подтверждаете удаление списка {toDoList.Name} и всех его задач",
                                              cancellationToken: ct,
                                              replyMarkup: new InlineKeyboardMarkup 
                                              { 
                                                  InlineKeyboard = [[InlineKeyboardButton.WithCallbackData("✅ Да", "yes"),
                                                                     InlineKeyboardButton.WithCallbackData("❌ Нет", "no")]] 
                                              });

                        context.CurrentStep = "Delete";

                        return ScenarioResult.Transition;
                    }
                case "Delete":
                    {
                        if (update.CallbackQuery?.Data == "yes")
                        {
                            Guid listId;
                            if (!Guid.TryParse(context.Data["toDoListId"].ToString(), out listId))
                            {
                                await botClient.SendMessage(chat.Id,
                                                "Информация по списку не найдена. Удаление отменено.",
                                                cancellationToken: ct);
                                return ScenarioResult.Completed;
                            }

                            ToDoUser? toDoUser = await _userService.GetUser(context.UserId, ct);
                            if (toDoUser == null)
                                throw new Exception("Объект пользователя не найден по id.");

                            var toDoItems = await _toDoService.GetByUserIdAndList(toDoUser.UserId, listId, ct);
                            foreach (var toDoItem in toDoItems)
                            {
                                if (ct.IsCancellationRequested)
                                {
                                    return ScenarioResult.Completed;
                                }

                                await _toDoService.Delete(toDoUser.UserId, toDoItem.Id, ct);
                            }

                            await _toDoListService.Delete(listId, ct);

                            await botClient.SendMessage(chat.Id, "Список удалён.", cancellationToken: ct);
                        }
                        else if (update.CallbackQuery?.Data == "no")
                        {
                            await botClient.SendMessage(chat.Id, "Удаление отменено.", cancellationToken: ct);
                        }

                        return ScenarioResult.Completed;
                    }
            }

            return ScenarioResult.Completed;
        }
    }
}
