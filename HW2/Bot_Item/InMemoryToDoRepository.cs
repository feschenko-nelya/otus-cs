using System.Collections.Generic;
using HW2.User;

namespace HW2.Bot_Item
{
    internal class InMemoryToDoRepository : IToDoRepository
    {
        private readonly List<ToDoItem> _items = new();
        public void Add(ToDoItem item)
        {
            _items.Add(item);
        }

        public int CountActive(Guid userId)
        {
            int count = 0;

            foreach(ToDoItem item in _items)
            {
                if (item.State == ToDoItemState.Active)
                {
                    count += 1;
                }
            }

            return count;
        }

        public void Delete(Guid id)
        {
            foreach (ToDoItem item in _items)
            {
                if (item.Id == id)
                {
                    _items.Remove(item);
                    break;
                }
            }
        }

        public bool ExistsByName(Guid userId, string name)
        {
            ToDoItem? item = _items.Find((item) => (item.UserId == userId) && (item.Name == name));

            return (item != null);
        }

        public ToDoItem? Get(Guid id)
        {
            return _items.Find((item) => (item.Id == id));
        }

        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            return _items.FindAll((item) => (item.UserId == userId) && (item.State == ToDoItemState.Active));
        }

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            return _items.FindAll((item) => (item.UserId == userId));
        }

        public IReadOnlyList<ToDoItem> Find(Guid userId, Func<ToDoItem, bool> predicate)
        {
            List<ToDoItem> resItems = new();

            foreach (ToDoItem item in _items)
            {
                if ((item.UserId == userId) && predicate(item))
                {
                    resItems.Add(item);
                }
            }

            return resItems;
        }

        public void Update(ToDoItem item)
        {
            ToDoItem? ownItem = Get(item.Id);

            if (ownItem != null)
            {
                ownItem = item;
            }
        }
    }
}
