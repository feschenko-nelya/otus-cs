
using Core.DataAccess;
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
        }
    }
}
