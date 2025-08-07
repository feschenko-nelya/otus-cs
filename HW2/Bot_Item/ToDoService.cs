
using HW2.User;

namespace HW2.Item
{
    public class ToDoService : IToDoService
    {
        private readonly Dictionary<Guid, List<ToDoItem>> _usersItems = new();

        private struct ItemLimit
        {
            public int Number { get; set; }
            public int Length { get; set; }

            public ItemLimit()
            {
                Number = 10;
                Length = 250;
            }
        }
        private Dictionary<Guid, ItemLimit> _usersLimits = new();
        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            List<ToDoItem>? userItems = _usersItems.GetValueOrDefault(userId);

            if (userItems == null)
            {
                return [];
            }

            List<ToDoItem> activeItems = new();

            foreach (ToDoItem item in userItems)
            {
                if (item.State == ToDoItemState.Active)
                {
                    activeItems.Add(item);
                }
            }

            return activeItems;
        }

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            List<ToDoItem>? items = _usersItems.GetValueOrDefault(userId);

            if (items == null)
            {
                return [];
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

            if (userItems.Count == userItemLimit.Number)
            {
                throw new TaskCountLimitException(userItemLimit.Number);
            }

            if (name.Length > userItemLimit.Length)
            {
                throw new TaskLengthLimitException(name.Length, userItemLimit.Length);
            }

            if (HasItemDuplicate(userItems, name))
            {
                throw new DuplicateTaskException(name);
            }

            var newItem = new ToDoItem(name);
            userItems.Add(newItem);

            return newItem;
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
            ItemLimit userItemLimit;
            if (_usersLimits.ContainsKey(userId))
            {
                userItemLimit = _usersLimits[userId];
            }
            else
            {
                userItemLimit = new();
            }

            userItemLimit.Number = maxNumber;
            _usersLimits[userId] = userItemLimit;
        }
        public void SetMaxLength(Guid userId, short maxLength)
        {
            ItemLimit userItemLimit;

            if (_usersLimits.ContainsKey(userId))
            {
                userItemLimit = _usersLimits[userId];
            }
            else
            {
                userItemLimit = new();
            }

            userItemLimit.Length = maxLength;
            _usersLimits[userId] = userItemLimit;
        }
        private ItemLimit GetUserItemLimit(Guid userId)
        {
            ItemLimit userItemLimit;
            if (_usersLimits.ContainsKey(userId))
            {
                userItemLimit = (ItemLimit)_usersLimits[userId];
            }
            else
            {
                userItemLimit = new();
                _usersLimits[userId] = userItemLimit;
            }

            return userItemLimit;
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
