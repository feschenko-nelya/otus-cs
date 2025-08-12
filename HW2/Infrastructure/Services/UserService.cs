using Core.Entity;
using Core.Services;
using Core.DataAccess;

namespace Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _users;

        public UserService(IUserRepository usersRepository)
        {
            _users = usersRepository;
        }
        public async Task<ToDoUser?> GetUser(long telegramUserId, CancellationToken cancelToken)
        {
            return await _users.GetUserByTelegramUserId(telegramUserId, cancelToken);
        }
        public async Task<ToDoUser> RegisterUser(long telegramUserId, string telegramUserName, CancellationToken cancelToken)
        {
            var user = await GetUser(telegramUserId, cancelToken);
            if (user != null)
            {
                return user;
            }

            user = new ToDoUser(telegramUserId, telegramUserName);
            await _users.Add(user, cancelToken);

            return user;
        }
    }
}
