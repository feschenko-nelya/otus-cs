
using HW2.User;

namespace HW2.Bot_Item
{
    public interface IToDoRepository
    {
        Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken cancelToken);
        // Возвращает ToDoItem для UserId со статусом Active
        Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken cancelToken);
        Task<IReadOnlyList<ToDoItem>> Find(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken cancelToken);
        Task<ToDoItem?> Get(Guid id, CancellationToken cancelToken);
        Task Add(ToDoItem item, CancellationToken cancelToken);
        Task Update(ToDoItem item, CancellationToken cancelToken);
        Task Delete(Guid id, CancellationToken cancelToken);
        // Проверяет есть ли задача с таким именем у пользователя
        Task<bool> ExistsByName(Guid userId, string name, CancellationToken cancelToken);
        // Возвращает количество активных задач у пользователя
        Task<int> CountActive(Guid userId, CancellationToken cancelToken);
    }
}
