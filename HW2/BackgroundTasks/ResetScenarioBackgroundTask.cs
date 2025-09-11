
using HW2.TelegramBot.Scenario;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace HW2.BackgroundTasks
{
    public class ResetScenarioBackgroundTask(TimeSpan resetScenarioTimeout, IScenarioContextRepository scenarioRepository, ITelegramBotClient botClient) : BackgroundTask(TimeSpan.FromHours(1), nameof(ResetScenarioBackgroundTask))
    {
        private TimeSpan _resetScenarioTimeout = resetScenarioTimeout;
        private IScenarioContextRepository _scenarioRepository = scenarioRepository;
        private ITelegramBotClient _botClient = botClient;

        protected override async Task Execute(CancellationToken ct)
        {
            var contexts = await _scenarioRepository.GetContexts(ct);

            DateTime now = DateTime.UtcNow;
            var updates = await _botClient.GetUpdates();

            foreach (var context in contexts)
            {
                TimeSpan differ = now - context.CreatedAt;
                
                if (differ >= _resetScenarioTimeout)
                {
                    await _scenarioRepository.ResetContext(context.UserId, ct);

                    foreach (Telegram.Bot.Types.Update upd in updates)
                    {
                        if (upd.Message != null)
                        {
                            await _botClient.SendMessage(upd.Message.Chat, 
                                                         $"Сценарий отменен, так как не поступил ответ в течение {_resetScenarioTimeout}",
                                                         replyMarkup: new ReplyKeyboardMarkup { 
                                                             Keyboard = [["/addtask"], ["/show"], ["/report"]],
                                                             ResizeKeyboard = true
                                                         }   );
                        }
                    }
                }
            }
        }
    }
}
