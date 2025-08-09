using HW2.Bot_User;

namespace HW2.User
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _users;

        public UserService(IUserRepository usersRepository)
        {
            _users = usersRepository;
        }
        public ToDoUser? GetUser(long telegramUserId)
        {
            return _users.GetUserByTelegramUserId(telegramUserId);
        }
        public ToDoUser RegisterUser(long telegramUserId, string telegramUserName)
        {
            var user = GetUser(telegramUserId);
            if (user != null)
            {
                return user;
            }

            user = new ToDoUser(telegramUserId, telegramUserName);
            _users.Add(user);

            return user;
        }
    }
}
