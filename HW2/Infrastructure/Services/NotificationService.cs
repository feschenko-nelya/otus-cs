
using Core.Entity;
using HW2.Core.DataAccess;
using HW2.Core.Entities;
using HW2.Core.Services;
using HW2.Infrastructure.DataAccess;
using HW2.Infrastructure.DataAccess.Models;
using LinqToDB;

namespace HW2.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IDataContextFactory<ToDoDataContext> _contextFactory;

        public NotificationService(IDataContextFactory<ToDoDataContext> factory)
        {
            _contextFactory = factory;
        }

        public async Task<IReadOnlyList<Notification>> GetScheduledNotification(DateTime scheduledBefore, CancellationToken ct)
        {
            List<Notification> result = new();

            using (var dbContext = _contextFactory.CreateDataContext())
            {
                var notfModels = await dbContext.GetTable<NotificationModel>()
                                       .Where(sqlItem => sqlItem.ScheduledAt <= scheduledBefore)
                                       .ToListAsync();

                await Task.Run(() =>
                {
                    foreach (var notfModel in notfModels)
                    {
                        result.Add(ModelMapper.MapFromModel(notfModel));
                    }
                });
            }

            return result;
        }

        public async Task MarkNotified(Guid notificationId, CancellationToken ct)
        {
            using (var dbContext = _contextFactory.CreateDataContext())
            {
                await dbContext.GetTable<NotificationModel>()
                      .Where(sqlItem => sqlItem.Id == notificationId)
                      .Set(sqlItem => sqlItem.IsNotified, true)
                      .Set(sqlItem => sqlItem.NotifiedAt, DateTime.UtcNow)
                      .UpdateAsync(token: ct);
            }
        }

        // Создает нотификацию. Если запись с userId и type уже есть, то вернуть false и не добавлять запись, иначе вернуть true
        public async Task<bool> ScheduleNotification(Guid userId, string type, string text, DateTime scheduledAt, CancellationToken ct)
        {
            bool result = false;

            using (var dbContext = _contextFactory.CreateDataContext())
            {
                NotificationModel? ntfm = dbContext.GetTable<NotificationModel>()
                                            .LoadWith(n => n.User)
                                            .FirstOrDefault(n => (n.UserId == userId) && (n.Type == type));

                if (ntfm == null)
                {
                    Notification ntf = new()
                    {
                        Id = Guid.NewGuid(),
                        User = new ToDoUser(),
                        Type = type,
                        Text = text,
                        ScheduledAt = scheduledAt,
                        IsNotified = false,
                        NotifiedAt = null
                    };

                    await dbContext.InsertAsync(ModelMapper.MapToModel(ntf), token: ct);

                    result = true;
                }
            }

            return result;
        }
    }
}
