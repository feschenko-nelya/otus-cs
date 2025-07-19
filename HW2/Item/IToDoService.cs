using HW2.User;

namespace HW2
{
    public interface IToDoService
    {
        IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId);
        //Возвращает ToDoItem для UserId со статусом Active
        IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId);
        ToDoItem Add(Guid userId, string name);
        void MarkCompleted(Guid userId, Guid itemId);
        bool Delete(Guid userId, Guid itemId);
    }
}
