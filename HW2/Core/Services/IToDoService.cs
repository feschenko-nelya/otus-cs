using Core.Entity;

namespace Core.Services
{
    public interface IToDoService
    {
        Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken cancelToken);
        //Возвращает ToDoItem для UserId со статусом Active
        Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken cancelToken);
        Task<IReadOnlyList<ToDoItem>> Find(ToDoUser user, string namePrefix, CancellationToken cancelToken);
        Task<ToDoItem> Add(Guid userId, string name, CancellationToken cancelToken);
        Task<bool> MarkCompleted(Guid userId, Guid itemId, CancellationToken cancelToken);
        Task<bool> Delete(Guid userId, Guid itemId, CancellationToken cancelToken);
    }
}
