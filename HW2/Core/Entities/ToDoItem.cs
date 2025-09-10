using System.Text;
using HW2.Core.Entities;
using Telegram.Bot.Types;

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
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public ToDoItemState State { get; set; }
        public DateTime StateChangedAt { get; set; }
        public DateTime? Deadline { get; set; }
        public Guid? ListId { get; set; }

        public ToDoUser User { get; set; } = new();
        public ToDoList? List { get; set; } = null;

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
