using Core.Entity;
using Core.DataAccess;

namespace Infrastructure.DataAccess
{
    internal class InMemoryToDoRepository : IToDoRepository
    {
        private readonly List<ToDoItem> _items = new();
        public async Task Add(ToDoItem item, CancellationToken cancelToken)
        {
            await Task.Run(() => _items.Add(item));
        }

        public Task<int> CountActive(Guid userId, CancellationToken cancelToken)
        {
            return Task.FromResult<int>(
                    (from item in _items
                     where item.State == ToDoItemState.Active
                     select item)
                      .Count());
        }

        public async Task Delete(Guid id, CancellationToken cancelToken)
        {
            await Task.Run(() =>
            {
                foreach (ToDoItem item in _items)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        break;
                    }

                    if (item.Id == id)
                    {
                        _items.Remove(item);
                        break;
                    }
                }
            });
        }

        public async Task<bool> ExistsByName(Guid userId, string name, CancellationToken cancelToken)
        {
            return await Task<bool>.Run(() =>
            {
                foreach (ToDoItem item in _items)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return false;
                    }

                    if ((item.UserId == userId) && (item.Name == name))
                    {
                        return true;
                    }
                }

                return false;
            });
        }

        public async Task<ToDoItem?> Get(Guid? id, CancellationToken cancelToken)
        {
            if (id == null)
                return null;

            return await Task<ToDoItem?>.Run(() =>
            {
                foreach (ToDoItem item in _items)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return null;
                    }

                    if ((item.Id == id))
                    {
                        return item;
                    }
                }

                return null;
            });
        }

        public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken cancelToken)
        {
            return await Task<IReadOnlyList<ToDoItem>>.Run(() =>
            {
                List<ToDoItem> resItems = new();

                foreach (ToDoItem item in _items)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return [];
                    }

                    if ((item.UserId == userId) && (item.State == ToDoItemState.Active))
                    {
                        resItems.Add(item);
                    }
                }

                return resItems;
            });
        }

        public async Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken cancelToken)
        {
            return await Task<IReadOnlyList<ToDoItem>>.Run(() =>
            {
                List<ToDoItem> resItems = new();

                foreach (ToDoItem item in _items)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return [];
                    }

                    if (item.UserId == userId)
                    {
                        resItems.Add(item);
                    }
                }

                return resItems;
            });
        }

        public async Task<IReadOnlyList<ToDoItem>> Find(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken cancelToken)
        {
            return await Task<IReadOnlyList<ToDoItem>>.Run(() =>
            {
                List<ToDoItem> resItems = new();

                foreach (ToDoItem item in _items)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return [];
                    }

                    if ((item.UserId == userId) && predicate(item))
                    {
                        resItems.Add(item);
                    }
                }

                return resItems;
            });
        }

        public async Task Update(ToDoItem item, CancellationToken cancelToken)
        {
            ToDoItem? ownItem = await Get(item.Id, cancelToken);

            if (ownItem != null)
            {
                ownItem = item;
            }
        }
    }
}
