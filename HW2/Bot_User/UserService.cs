
namespace HW2.User
{
    public class UserService : IUserService
    {
        private List<ToDoUser> _users = new List<ToDoUser>();

        public ToDoUser? GetUser(long telegramUserId)
        {
            foreach (ToDoUser user in _users)
            {
                if (user.TelegramUserId == telegramUserId)
                {
                    return user;
                }
            }

            return null;
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
        public void UnregisterUser(long telegramUserId)
        {
            foreach (ToDoUser user in _users)
            {
                if (user.TelegramUserId == telegramUserId)
                {
                    _users.Remove(user);
                    break;
                }
            }
        }
    }
}
