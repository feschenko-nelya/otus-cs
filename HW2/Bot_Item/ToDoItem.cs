namespace HW2.User
{
    public enum ToDoItemState 
    { 
        None, 
        Active, 
        Completed 
    };
    public class ToDoItem
    {
        public Guid Id { get; init; }
        public ToDoUser User { get; set; }
        public string Name { get; init; }
        public DateTime CreatedAt { get; init; }
        public ToDoItemState State { get; private set; }
        public DateTime? StateChangedAt { get; private set; }

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
        public string toString()
        {
            return $"{Name} - {CreatedAt.ToString("dd.MM.yyyy HH:mm:ss")} - {Id}";
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
