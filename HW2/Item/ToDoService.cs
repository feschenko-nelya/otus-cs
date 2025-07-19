
using System.Collections.Generic;
using HW2.User;
using Otus.ToDoList.ConsoleBot.Types;

namespace HW2.Item
{
    public class ToDoService : IToDoService
    {
        private Dictionary<Guid, ToDoItems> _usersItems = new();
        
        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            ToDoItems? result = _usersItems.GetValueOrDefault(userId);

            if (result == null)
            {
                result = [];
            }
            else
            {
                result.GetByState(ToDoItemState.Active);
            }

            return result;
        }

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            ToDoItems? result = _usersItems.GetValueOrDefault(userId);

            if (result == null)
            {
                result = [];
            }

            return result;
        }

        public bool MarkCompleted(Guid userId, int itemNumber)
        {
            ToDoItems? userItems = _usersItems.GetValueOrDefault(userId);

            if (userItems == null)
            {
                return false;
            }

            if (itemNumber < 0 || itemNumber >= userItems.Count)
            {
                return false;
            }

            userItems.ElementAt(itemNumber).SetCompleted();

            return true;
        }

        public ToDoItem Add(Guid userId, string name)
        {
            ToDoItems? userItems = _usersItems.GetValueOrDefault(userId);

            if (userItems == null)
            {
                userItems = new();
                _usersItems.Add(userId, userItems);
            }

            ToDoItem newItem = new ToDoItem(name);
            userItems.Add(newItem);

            return newItem;
        }

        public bool Delete(Guid userId, int itemNumber)
        {
            ToDoItems? userItems = _usersItems.GetValueOrDefault(userId);

            if (userItems == null)
            {
                return false;
            }

            if (itemNumber < 0 || itemNumber >= userItems.Count)
            {
                return false;
            }

            userItems.RemoveAt(userItems.Count);

            return true;
        }
        public bool SetMaxNumber(Guid userId, short maxNumber)
        {
            ToDoItems? userItems = _usersItems.GetValueOrDefault(userId);

            if (userItems == null)
            {
                return false;
            }

            userItems.MaxNumber = maxNumber;

            return true;
        }
        public bool SetMaxLength(Guid userId, short maxLength)
        {
            ToDoItems? userItems = _usersItems.GetValueOrDefault(userId);

            if (userItems == null)
            {
                return false;
            }

            userItems.MaxLength = maxLength;

            return true;
        }
    }
}
