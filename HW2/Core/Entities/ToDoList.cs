
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Entity;

namespace HW2.Core.Entities
{
    public class ToDoList : ISerializable
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

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Guid id;
            if (Guid.TryParse(info.GetString("Id"), out id))
                Id = id;

            Name = info.GetString("Name");

            if (Guid.TryParse(info.GetString("UserId"), out id))
                UserId = id;

            DateTime createdAt;
            if (DateTime.TryParse(info.GetString("CreatedAt"), out createdAt))
                CreatedAt = createdAt;
        }
    }
}
