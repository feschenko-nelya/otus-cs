using HW2.User;

namespace HW2
{
    public interface IToDoService
    {
        IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId);
        //Возвращает ToDoItem для UserId со статусом Active
        IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId);
        IReadOnlyList<ToDoItem> Find(ToDoUser user, string namePrefix);
        ToDoItem Add(Guid userId, string name);
        bool MarkCompleted(Guid userId, Guid itemId);
        bool Delete(Guid userId, Guid itemId);
    }
}
