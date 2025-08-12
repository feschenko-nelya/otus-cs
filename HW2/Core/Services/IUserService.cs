using Core.Entity;

namespace Core.Services
{
    internal interface IUserService
    {
        Task<ToDoUser> RegisterUser(long telegramUserId, string telegramUserName, CancellationToken cancelToken);
        Task<ToDoUser?> GetUser(long telegramUserId, CancellationToken cancelToken);
    }
}
