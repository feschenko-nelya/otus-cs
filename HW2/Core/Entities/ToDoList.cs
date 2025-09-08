
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Entity;

namespace HW2.Core.Entities
{
    public class ToDoList
    {
        [JsonInclude]
        public Guid Id { get; private set; }
        [JsonInclude]
        public string? Name { get; set; }
        [JsonInclude]
        public Guid? UserId { get; private set; }
        [JsonInclude]
        public DateTime CreatedAt { get; private set; }

        [JsonConstructor]
        private ToDoList()
        {

        }
        public ToDoList(ToDoUser toDoUser, string name)
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
            UserId = toDoUser.UserId;
            Name = name;
        }
    }
}
