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
                        context.Data.Add(toDoUser.TelegramUserName, toDoUser);
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
                        ToDoUser? toDoUser = null;
                        context.Data.TryGetValue(update.Message.From.Username, out toDoUser);

                        if (toDoUser != null)
                        {
                            await _toDoService.Add(toDoUser.UserId, update.Message.Text, ct);

                            return ScenarioResult.Completed;
                        }
                    }
                    break;
                }
            }

            return ScenarioResult.Completed;
        }
    }
}
