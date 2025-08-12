using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW2.User
{
    internal interface IUserService
    {
        Task<ToDoUser> RegisterUser(long telegramUserId, string telegramUserName, CancellationToken cancelToken);
        Task<ToDoUser?> GetUser(long telegramUserId, CancellationToken cancelToken);
    }
}
