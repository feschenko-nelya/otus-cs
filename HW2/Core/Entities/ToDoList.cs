
using System.Runtime.Serialization;
using Core.Entity;

namespace HW2.Core.Entities
{
    public class ToDoList
    {
        public Guid Id { get; init; }
        public string? Name { get; set; }
        public ToDoUser User { get; init; }
        public DateTime CreatedAt { get; init; }

        public ToDoList(ToDoUser toDoUser)
        {
            Id = Guid.NewGuid();
            User = toDoUser;
            CreatedAt = DateTime.Now;

        }
    }
}
