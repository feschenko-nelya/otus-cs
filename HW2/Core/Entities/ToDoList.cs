
using Core.Entity;

namespace HW2.Core.Entities
{
    public class ToDoList
    {
        Guid Id { get; init; }
        string? Name { get; set; }
        ToDoUser User { get; init; }
        DateTime CreatedAt { get; init; }

        public ToDoList(ToDoUser toDoUser)
        {
            Id = Guid.NewGuid();
            User = toDoUser;
            CreatedAt = DateTime.Now;

        }
    }
}
