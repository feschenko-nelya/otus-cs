
using Core.Entity;
using HW2.Core.DataAccess;
using HW2.Core.Entities;
using HW2.Core.Services;

namespace HW2.Infrastructure.Services
{
    internal class ToDoListService : IToDoListService
    {
        private readonly IToDoListRepository _toDoListRepository;
        public ToDoListService(IToDoListRepository toDoListRepository)
        {
            _toDoListRepository = toDoListRepository;
        }
        public async Task<ToDoList> Add(ToDoUser user, string name, CancellationToken ct)
        {
            if (name.Length > 10)
            {
                throw new TaskLengthLimitException(name.Length, 10);
            }

            IReadOnlyList<ToDoList> toDoLists = await GetUserLists(user.UserId, ct);

            bool isListNameExisted = (from list in toDoLists 
                                      where list.Name == name 
                                      select list)
                                      .Count() > 0;

            if (isListNameExisted)
                throw new Exception("Название списка уже существует.");

            ToDoList newList = new()
            {
                UserId = user.UserId,
                Name = name
            };

            await _toDoListRepository.Add(newList, ct);

            return newList;
        }

        public async Task Delete(Guid id, CancellationToken ct)
        {
            await _toDoListRepository.Delete(id, ct);
        }

        public async Task<ToDoList?> Get(Guid? id, CancellationToken ct)
        {
            return await _toDoListRepository.Get(id, ct);
        }

        public async Task<IReadOnlyList<ToDoList>> GetUserLists(Guid userId, CancellationToken ct)
        {
            return await _toDoListRepository.GetByUserId(userId, ct);
        }
    }
}
