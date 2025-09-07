using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using HW2.Core.Entities;

namespace Core.Entity
{
    public enum ToDoItemState 
    { 
        None, 
        Active, 
        Completed 
    };
    public class ToDoItem
    {
        [JsonInclude]
        public Guid Id { get; private set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = "";
        [JsonInclude]
        public DateTime CreatedAt { get; private set; }
        public ToDoItemState State { get; set; }
        [JsonInclude]
        public DateTime? StateChangedAt { get; private set; }
        public DateTime? Deadline { get; set; }
        public ToDoList? List { get; set; }

        [JsonConstructor]
        private ToDoItem()
        {

        }
        public ToDoItem(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
            State = ToDoItemState.Active;
            StateChangedAt = DateTime.Now;
            CreatedAt = DateTime.Now;
        }
        public void SetCompleted()
        {
            State = ToDoItemState.Completed;
            StateChangedAt = DateTime.Now;
        }
        public override string ToString()
        {
            StringBuilder strb = new();
            strb.Append(Name);
            strb.Append(" - cr: ");
            strb.Append(CreatedAt.ToString("dd.MM.yyyy HH:mm:ss"));
            strb.Append(" - ");
            strb.Append(Id.ToString());

            if (Deadline != null)
            {
                strb.Append(" - dl: ");
                strb.Append(Deadline?.ToString("dd.MM.yyyy"));
            }

            return strb.ToString();
        }

        public string GetStateName()
        {
            switch (State)
            {
            case ToDoItemState.Active:
                return "активна";
            case ToDoItemState.Completed:
                return "выполнена";
            }

            return "-";
        }
        //public void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    Guid id;
        //    if (Guid.TryParse(info.GetString("Id"), out id))
        //        Id = id;

        //    if (Guid.TryParse(info.GetString("UserId"), out id))
        //        UserId = id;

        //    Name = info.GetString("Name");

        //    if (Guid.TryParse(info.GetString("UserId"), out id))
        //        UserId = id;

        //    DateTime createdAt;
        //    if (DateTime.TryParse(info.GetString("CreatedAt"), out createdAt))
        //        CreatedAt = createdAt;
        //}
    }
}
