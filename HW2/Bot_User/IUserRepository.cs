
using HW2.User;

namespace HW2.Bot_User
{
    public interface IUserRepository
    {
        Task<ToDoUser?> GetUser(Guid userId, CancellationToken cancelToken);
        Task<ToDoUser?> GetUserByTelegramUserId(long telegramUserId, CancellationToken cancelToken);
        Task Add(ToDoUser user, CancellationToken cancelToken);
    }
}
