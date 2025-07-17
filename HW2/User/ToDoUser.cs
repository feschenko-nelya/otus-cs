using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW2.User
{
    public class ToDoUser
    {
        public Guid UserId { get; init; }
        public long TelegramUserId { get; init; }
        public string TelegramUserName { get; init; }
        public DateTime RegisteredAt { get; init; }

        public ToDoUser(long telegramUserId, string telegramUserName)
        {
            UserId = Guid.NewGuid();
            TelegramUserId = telegramUserId;
            TelegramUserName = telegramUserName;
            RegisteredAt = DateTime.Now;
        }
    }
}
