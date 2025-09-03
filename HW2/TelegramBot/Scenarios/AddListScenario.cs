
using Core.Entity;
using Core.Services;
using HW2.Core.Services;
using HW2.Infrastructure.Services;
using HW2.TelegramBot.Scenario;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace HW2.TelegramBot.Scenarios
{
    internal class AddListScenario : IScenario
    {
        private readonly IUserService _userService;
        private readonly IToDoListService _toDoListService;

        public AddListScenario(IUserService userService, IToDoListService toDoListService)
        {
            _userService = userService;
            _toDoListService = toDoListService;
        }

        public bool CanHandle(ScenarioType scenarioType)
        {
            return scenarioType == ScenarioType.AddList;
        }

        public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient bot, ScenarioContext context, Update update, CancellationToken ct)
        {
            Message? message = update.Message;

            if (message == null)
                message = update.CallbackQuery?.Message;

            if (message == null)
                throw new Exception("Не обнаружен объект сообщения.");

            Chat? chat = message.Chat;

            if (chat == null)
                throw new Exception("Не обнаружен чат.");

                switch (context.CurrentStep)
                {
                    case null:
                        {
                            ReplyKeyboardMarkup cancelKeyboard = new();
                            cancelKeyboard.ResizeKeyboard = true;
                            cancelKeyboard.Keyboard = [[new KeyboardButton("/cancel")]];

                            await bot.SendMessage(chat.Id,
                                                    "Введите название списка:",
                                                    cancellationToken: ct,
                                                    replyMarkup: cancelKeyboard);

                            context.CurrentStep = "Name";

                            return ScenarioResult.Transition;
                        }
                    case "Name":
                        {
                            if (string.IsNullOrEmpty(message.Text))
                            {
                                context.CurrentStep = null;
                                return ScenarioResult.Transition;
                            }

                            ToDoUser? toDoUser = await _userService.GetUser(context.UserId, ct);
                            if (toDoUser == null)
                                throw new Exception("Объект пользователя не найден по id.");

                            await _toDoListService.Add(toDoUser, message.Text, ct);

                            return ScenarioResult.Completed;
                        }
                }

            return ScenarioResult.Completed;
        }
    }
}
