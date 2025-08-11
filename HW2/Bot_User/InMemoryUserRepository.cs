using HW2.User;

namespace HW2.Bot_User
{
    internal class InMemoryUserRepository : IUserRepository
    {
        private readonly List<ToDoUser> _toDoUsers = new();
        public void Add(ToDoUser user)
        {
            if (GetUser(user.UserId) != null)
            {
                return;
            }

            _toDoUsers.Add(user);
        }

        public ToDoUser? GetUser(Guid userId)
        {
            foreach (ToDoUser user in _toDoUsers)
            {
                if (user.UserId == userId)
                {
                    return user;
                }
            }

            return null;
        }

        public ToDoUser? GetUserByTelegramUserId(long telegramUserId)
        {
            foreach (ToDoUser user in _toDoUsers)
            {
                if (user.TelegramUserId == telegramUserId)
                {
                    return user;
                }
            }

            return null;
        }
    }
}
