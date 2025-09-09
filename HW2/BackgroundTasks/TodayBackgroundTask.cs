
using System.Text;
using Core.DataAccess;
using HW2.Core.Services;

namespace HW2.BackgroundTasks
{
    public class TodayBackgroundTask(INotificationService notificationService, IUserRepository userRepository, IToDoRepository toDoRepository) : BackgroundTask(TimeSpan.FromDays(1), nameof(TodayBackgroundTask))
    {
        protected override async Task Execute(CancellationToken ct)
        {
            var users = await userRepository.GetUsers(ct);

            foreach (var user in users)
            {
                var tasks = await toDoRepository.GetActiveByUserId(user.UserId, ct);

                if (tasks.Count == 0)
                    continue;

                StringBuilder strb = new();
                strb.AppendLine("Задачи на сегодня:");

                foreach (var task in tasks)
                {
                    strb.AppendLine(task.Name);
                }

                await notificationService.ScheduleNotification(user.UserId,
                                                               $"Today_{DateOnly.FromDateTime(DateTime.UtcNow)}",
                                                               strb.ToString(),
                                                               DateTime.UtcNow,
                                                               ct);
            }
        }
    }
}
