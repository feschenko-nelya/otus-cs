
using Core.DataAccess;
using HW2.Core.Entities;
using HW2.Core.Services;

namespace HW2.BackgroundTasks
{
    public class DeadlineBackgroundTask(INotificationService notificationService, IUserRepository userRepository, IToDoRepository toDoRepository) 
                    : BackgroundTask(TimeSpan.FromHours(1), nameof(DeadlineBackgroundTask))
    {
        protected override async Task Execute(CancellationToken ct)
        {
            var users = await userRepository.GetUsers(ct);

            foreach (var user in users)
            {
                var tasks = await toDoRepository.GetActiveWithDeadline(user.UserId, DateTime.UtcNow.AddDays(-1).Date, DateTime.UtcNow.Date, ct);

                foreach (var task in tasks)
                {
                    await notificationService.ScheduleNotification(user.UserId, 
                                                                   $"Dealine_{task.Id}", 
                                                                   $"Ой! Вы пропустили дедлайн по задаче {task.Name}", 
                                                                   DateTime.UtcNow, 
                                                                   ct);
                }
            }

            

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
