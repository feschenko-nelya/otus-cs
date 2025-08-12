
using Core.Entity;

namespace Core.DataAccess
{
    public interface IUserRepository
    {
        Task<ToDoUser?> GetUser(Guid userId, CancellationToken cancelToken);
        Task<ToDoUser?> GetUserByTelegramUserId(long telegramUserId, CancellationToken cancelToken);
        Task Add(ToDoUser user, CancellationToken cancelToken);
    }
}
