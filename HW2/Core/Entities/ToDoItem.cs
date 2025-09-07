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
            strb.AppendLine(Name);
            strb.AppendLine();
            strb.AppendLine("Created at: " + CreatedAt.ToString("dd.MM.yyyy HH:mm:ss"));
            strb.Append("Deadline: ");
            if (Deadline != null)
            {
                strb.AppendLine(Deadline?.ToString("dd.MM.yyyy"));
            }

            strb.AppendLine($"State: '{GetStateName()}'");
            strb.AppendLine();
            strb.AppendLine($"Id: '{Id}'");
            
            return strb.ToString();
        }
        public string GetHtmlString()
        {
            StringBuilder strb = new();
            strb.AppendLine($"<b>{Name}</b>");
            strb.AppendLine();
            strb.AppendLine("Created at: " + CreatedAt.ToString("dd.MM.yyyy HH:mm:ss"));
            strb.Append("Deadline: ");
            if (Deadline != null)
            {
                strb.AppendLine(Deadline?.ToString("dd.MM.yyyy"));
            }

            strb.AppendLine($"State: '{GetStateName()}'");
            strb.AppendLine();
            strb.AppendLine($"Id: '{Id}'");

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
    }
}
