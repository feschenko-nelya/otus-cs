
using HW2.User;

namespace HW2.Item
{
    public class ToDoService : IToDoService
    {
        private readonly Dictionary<Guid, List<ToDoItem>> _usersItems = new();
        private struct ItemLimit
        {
            public int number = 10;
            public int length = 250;

            public ItemLimit()
            {
            }
        }
        private Dictionary<Guid, ItemLimit> _usersLimits = new();
        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            List<ToDoItem>? items = _usersItems.GetValueOrDefault(userId);

            if (items == null)
            {
                items = [];
            }
            else
            {
                foreach (ToDoItem item in items)
                {
                    if (item.State == ToDoItemState.Active)
                    {
                        items.Add(item);
                    }
                }
            }

            return items;
        }

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            List<ToDoItem>? items = _usersItems.GetValueOrDefault(userId);

            if (items == null)
            {
                items = [];
            }

            return items;
        }

        public bool MarkCompleted(Guid userId, Guid itemId)
        {
            List<ToDoItem>? userItems = _usersItems.GetValueOrDefault(userId);

            if (userItems == null)
            {
                return false;
            }

            ToDoItem? item = GetItemByGuid(userItems, itemId);

            if (item == null)
            {
                return false;
            }

            item.SetCompleted();

            return true;
        }

        public ToDoItem Add(Guid userId, string name)
        {
            List<ToDoItem>? userItems = _usersItems.GetValueOrDefault(userId);

            if (userItems == null)
            {
                userItems = new();
                _usersItems.Add(userId, userItems);
            }

            ItemLimit userItemLimit = GetUserItemLimit(userId);

            if (userItems.Count == userItemLimit.number)
            {
                throw new TaskCountLimitException(userItemLimit.number);
            }

            if (name.Length > userItemLimit.length)
            {
                throw new TaskLengthLimitException(name.Length, userItemLimit.length);
            }

            if (HasItemDuplicate(userItems, name))
            {
                throw new DuplicateTaskException(name);
            }

            var newItem = new ToDoItem(name);
            userItems.Add(newItem);

            return newItem;
        }
        private ItemLimit GetUserItemLimit(Guid userId)
        {
            ItemLimit userItemLimit;
            if (_usersLimits.ContainsKey(userId))
            {
                userItemLimit = _usersLimits[userId];
            }
            else
            {
                userItemLimit = new();
                _usersLimits[userId] = userItemLimit;
            }

            return userItemLimit;
        }

        public bool Delete(Guid userId, Guid itemId)
        {
            List<ToDoItem>? userItems = _usersItems.GetValueOrDefault(userId);

            if (userItems == null)
            {
                return false;
            }

            ToDoItem? item = GetItemByGuid(userItems, itemId);

            if (item == null)
            {
                return false;
            }

            return userItems.Remove(item);
        }
        public void SetMaxNumber(Guid userId, short maxNumber)
        {
            ItemLimit userItemLimit = GetUserItemLimit(userId);
            userItemLimit.number = maxNumber;
        }
        public void SetMaxLength(Guid userId, short maxLength)
        {
            ItemLimit userItemLimit = GetUserItemLimit(userId);
            userItemLimit.length = maxLength;
        }

        private ToDoItem? GetItemByGuid(List<ToDoItem> userItems, Guid id)
        {
            foreach (ToDoItem item in userItems)
            {
                if (item.Id.Equals(id))
                {
                    return item;
                }
            }

            return null;
        }

        public bool HasItemDuplicate(List<ToDoItem> userItems, string name)
        {
            foreach (var item in userItems)
            {
                if (item.Name == name)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
