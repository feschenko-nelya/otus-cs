
using HW2.Core.Entities;
using HW2.Core.Services;
using Telegram.Bot;

namespace HW2.BackgroundTasks
{
    public class NotificationBackgroundTask(INotificationService notificationService, ITelegramBotClient botClient) : BackgroundTask(TimeSpan.FromMinutes(1), nameof(NotificationBackgroundTask))
    {
        protected override async Task Execute(CancellationToken ct)
        {
            var notifies = await notificationService.GetScheduledNotification(DateTime.UtcNow, ct);

            if (notifies.Count == 0)
                return;

            var updates = await botClient.GetUpdates();

            foreach (Notification notify in notifies)
            {
                foreach (Telegram.Bot.Types.Update upd in updates)
                {
                    if (upd.Message != null)
                    {
                        await botClient.SendMessage(upd.Message.Chat, notify.Text);
                    }
                }

                await notificationService.MarkNotified(notify.Id, ct);
            }
        }
    }
}
