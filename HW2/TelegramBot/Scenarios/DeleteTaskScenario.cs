
using Core.Entity;
using Core.Services;
using HW2.Core.Entities;
using HW2.TelegramBot.Dto;
using HW2.TelegramBot.Scenario;
using Infrastructure.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace HW2.TelegramBot.Scenarios
{
    internal class DeleteTaskScenario : IScenario
    {
        private readonly IUserService _userService;
        private readonly IToDoService _toDoService;
        public DeleteTaskScenario(IUserService userService, IToDoService toDoService) 
        {
            _userService = userService;
            _toDoService = toDoService;
        }
        public bool CanHandle(ScenarioType scenarioType)
        {
            return scenarioType == ScenarioType.DeleteTask;
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
                        if (query.Data == null)
                            return ScenarioResult.Completed;

                        var toDoItemCallback = ToDoItemCallbackDto.FromString(query.Data);
                        if (toDoItemCallback.ToDoItemId == null)
                        {
                            await botClient.SendMessage(chat.Id, "Информация по задаче не найдена.", cancellationToken: ct);
                            return ScenarioResult.Completed;
                        }

                        ToDoItem? toDoItem = await _toDoService.Get((Guid)toDoItemCallback.ToDoItemId, ct);
                        if (toDoItem == null)
                        {
                            await botClient.SendMessage(chat.Id, "Информация по задаче не найдена.", cancellationToken: ct);
                            return ScenarioResult.Completed;
                        }

                        InlineKeyboardMarkup keyboard = new();
                        keyboard.AddButtons(
                            [
                                InlineKeyboardButton.WithCallbackData("✅ Да", "yes"),
                                InlineKeyboardButton.WithCallbackData("❌ Нет", "no")
                            ]
                        );

                        await botClient.SendMessage(chat.Id, $"Подтвердите удаление '{toDoItem.Name}'.", 
                                                    replyMarkup:keyboard, cancellationToken: ct);

                        context.Data["toDoItemUserId"] = toDoItem.User.UserId;
                        context.Data["toDoItemId"] = toDoItem.Id;

                        context.CurrentStep = "Approve";

                        return ScenarioResult.Transition;
                    }
                case "Approve":
                    {
                        if (update.CallbackQuery?.Data == "yes")
                        {
                            Guid guid;
                            if (!Guid.TryParse(context.Data["toDoItemId"].ToString(), out guid))
                            {
                                await botClient.SendMessage(chat.Id,
                                                "Информация по задаче не найдена. Удаление отменено.",
                                                cancellationToken: ct);
                                return ScenarioResult.Completed;
                            }

                            Guid toDoItemId = guid;

                            if (!Guid.TryParse(context.Data["toDoItemUserId"].ToString(), out guid))
                            {
                                await botClient.SendMessage(chat.Id,
                                                "Информация по пользователю не найдена. Удаление отменено.",
                                                cancellationToken: ct);
                                return ScenarioResult.Completed;
                            }

                            Guid toDoUserId = guid;

                            ToDoUser? toDoUser = await _userService.GetUser(context.UserId, ct);
                            if (toDoUser == null)
                                throw new Exception("Объект пользователя не найден по id.");

                            await _toDoService.Delete(toDoUserId, toDoItemId, ct);
                            await botClient.SendMessage(chat.Id, "Задача удалена.", cancellationToken: ct);
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
