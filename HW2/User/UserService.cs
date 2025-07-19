using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HW3;

namespace HW2.User
{
    public class UserService : IUserService
    {
        private List<ToDoUser> _users = new List<ToDoUser>();
        private Dictionary<long, UserCommands> _usersCommands = new();
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
        public bool IsUserRegistered(long telegramUserId)
        {
            return (GetUser(telegramUserId) != null);
        }

        public UserCommands? GetUserCommandsByState(long telegramUserId, ToDoItemState commandState)
        {
            UserCommands? userCommands = new();
            if (_usersCommands.TryGetValue(telegramUserId, out userCommands))
            {
                return userCommands.GetByState(commandState);
            }

            return null;
        }

        public bool AddUserCommand(long telegramUserId, string commandName)
        {
            UserCommands? userCommands;

            if (!_usersCommands.TryGetValue(telegramUserId, out userCommands))
            {
                return false;
            }

            if (userCommands == null)
            {
                return false;
            }

            return userCommands.TryAdd(commandName);
        }

        public bool RemoveUserCommand(long telegramUserId, int index)
        {
            UserCommands? userCommands;

            if (!_usersCommands.TryGetValue(telegramUserId, out userCommands))
            {
                return false;
            }

            if ((userCommands != null) && (userCommands.Count > 0))
            {
                userCommands.RemoveAt(index);
                return true;
            }            

            return false;
        }

        public bool SetUserCommandsMaxNumber(long telegramUserId, short maxNumber)
        {
            UserCommands? userCommands;

            if (!_usersCommands.TryGetValue(telegramUserId, out userCommands))
            {
                return false;
            }

            if ((userCommands != null) && (userCommands.Count > 0))
            {
                userCommands.MaxNumber = maxNumber;

                return true;
            }

            return false;
        }
        public bool SetUserCommandMaxLength(long telegramUserId, short maxLength)
        {
            UserCommands? userCommands;

            if (!_usersCommands.TryGetValue(telegramUserId, out userCommands))
            {
                return false;
            }

            if ((userCommands != null) && (userCommands.Count > 0))
            {
                userCommands.MaxLength = maxLength;

                return true;
            }

            return false;
        }
    }
}
