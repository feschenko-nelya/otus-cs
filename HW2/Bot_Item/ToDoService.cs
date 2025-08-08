
using HW2.Bot_Item;
using HW2.User;

namespace HW2.Item
{
    public class ToDoService : IToDoService
    {
        private readonly IToDoRepository _toDoRepository;

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

        public ToDoService(IToDoRepository toDoRepository)
        {
            _toDoRepository = toDoRepository;
        }
        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            return _toDoRepository.GetActiveByUserId(userId);
        }

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            return _toDoRepository.GetAllByUserId(userId);
        }

        public bool MarkCompleted(Guid userId, Guid itemId)
        {
            ToDoItem? item = _toDoRepository.Get(itemId);

            if (item == null)
            {
                return false;
            }

            item.SetCompleted();

            return true;
        }

        public ToDoItem Add(Guid userId, string name)
        {
            ItemLimit userItemLimit = GetUserItemLimit(userId);

            
            if (_toDoRepository.GetAllByUserId(userId).Count == userItemLimit.Number)
            {
                throw new TaskCountLimitException(userItemLimit.Number);
            }

            if (name.Length > userItemLimit.Length)
            {
                throw new TaskLengthLimitException(name.Length, userItemLimit.Length);
            }

            if (_toDoRepository.ExistsByName(userId, name))
            {
                throw new DuplicateTaskException(name);
            }

            var newItem = new ToDoItem(name);
            newItem.UserId = userId;

            _toDoRepository.Add(newItem);

            return newItem;
        }

        public bool Delete(Guid userId, Guid itemId)
        {
            _toDoRepository.Delete(itemId);
            
            return true;
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
    }
}
