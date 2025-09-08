using System.Collections.Generic;
using Core.DataAccess;
using Core.Entity;
using Core.Services;
using HW2.Core.DataAccess;
using HW2.Core.Entities;

namespace Infrastructure.Services
{
    public class ToDoService : IToDoService
    {
        public readonly IToDoRepository toDoRepository;

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
            this.toDoRepository = toDoRepository;
        }
        public async Task<ToDoItem?> Get(Guid? toDoItemId, CancellationToken ct)
        {
            return await toDoRepository.Get(toDoItemId, ct);
        }
        public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken cancelToken)
        {
            return await toDoRepository.GetActiveByUserId(userId, cancelToken);
        }

        public async Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken cancelToken)
        {
            return await toDoRepository.GetAllByUserId(userId, cancelToken);
        }
        public async Task<IReadOnlyList<ToDoItem>> Find(ToDoUser user, string namePrefix, CancellationToken cancelToken)
        {
            Func<ToDoItem, bool> predicate = (item) => item.Name.StartsWith(namePrefix);

            return await toDoRepository.Find(user.UserId, predicate, cancelToken);
        }

        public async Task<bool> MarkCompleted(Guid userId, Guid itemId, CancellationToken cancelToken)
        {
            ToDoItem? item = await toDoRepository.Get(itemId, cancelToken);

            if (item == null)
            {
                return false;
            }

            item.SetCompleted();
            toDoRepository?.Update(item, cancelToken);

            return true;
        }

        public async Task<ToDoItem> Add(Guid userId, string name, DateTime? deadline, ToDoList? list, CancellationToken cancelToken)
        {
            ItemLimit userItemLimit = GetUserItemLimit(userId);

            IReadOnlyList<ToDoItem> items = await toDoRepository.GetAllByUserId(userId, cancelToken);
            if (items.Count == userItemLimit.Number)
            {
                throw new TaskCountLimitException(userItemLimit.Number);
            }

            if (name.Length > userItemLimit.Length)
            {
                throw new TaskLengthLimitException(name.Length, userItemLimit.Length);
            }

            if (toDoRepository.ExistsByName(userId, name, cancelToken).Result)
            {
                throw new DuplicateTaskException(name);
            }

            ToDoItem newItem = new()
            {
                Name = name,
                UserId = userId,
                Deadline = (deadline == default(DateTime) || deadline == null) ? null : deadline,
                ListId = list?.Id
            };

            await toDoRepository.Add(newItem, cancelToken);

            return newItem;
        }

        public async Task<bool> Delete(Guid userId, Guid itemId, CancellationToken cancelToken)
        {
            await toDoRepository.Delete(itemId, cancelToken);
            
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
                userItemLimit = _usersLimits[userId];
            }
            else
            {
                userItemLimit = new();
                _usersLimits[userId] = userItemLimit;
            }

            return userItemLimit;
        }
        public async Task<IReadOnlyList<ToDoItem>> GetByUserIdAndList(Guid userId, Guid? listId, CancellationToken ct)
        {
            if (listId == null)
                return [];

            var toDoItems = await GetAllByUserId(userId, ct);

            if (toDoItems.Count == 0)
                return [];

            var result = from item in toDoItems
                         where item.ListId == listId
                         select item;

            return result.ToList();
        }
    }
}
