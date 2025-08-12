using Core.Entity;
using Core.DataAccess;

namespace Infrastructure.DataAccess
{
    internal class InMemoryUserRepository : IUserRepository
    {
        private readonly List<ToDoUser> _toDoUsers = new();
        public async Task Add(ToDoUser user, CancellationToken cancelToken)
        {
            ToDoUser? existedUser = await GetUser(user.UserId, cancelToken);

            if (existedUser == null)
            {
                _toDoUsers.Add(user);
            }
        }

        public async Task<ToDoUser?> GetUser(Guid userId, CancellationToken cancelToken)
        {
            return await Task<ToDoUser?>.Run(() =>
                   {
                        foreach (ToDoUser user in _toDoUsers)
                        {
                           if (cancelToken.IsCancellationRequested)
                           {
                               return null;
                           }

                           if (user.UserId == userId)
                           {
                               return user;
                           }
                        }

                        return null;
                   });
        }

        public async Task<ToDoUser?> GetUserByTelegramUserId(long telegramUserId, CancellationToken cancelToken)
        {
            return await Task<ToDoUser?>.Run(() =>
            { 
                foreach (ToDoUser user in _toDoUsers)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return null;
                    }

                    if (user.TelegramUserId == telegramUserId)
                    {
                        return user;
                    }
                }

                return null;
            });
        }
    }
}
