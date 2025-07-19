using HW3;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;

namespace HW2.User
{
    public enum ToDoItemState 
    { 
        None, 
        Active, 
        Completed 
    };
    public class ToDoItem : AbstractCommand
    {
        public Guid Id { get; init; }
        private ToDoUser? User;
        public string Name { get; init; }
        public DateTime CreatedAt { get; init; }
        public ToDoItemState State { get; private set; }
        public DateTime? StateChangedAt { get; private set; }

        public ToDoItem(string name) : base(null)
        {
            Id = Guid.NewGuid();
            Name = name;
            State = ToDoItemState.Active;
            StateChangedAt = DateTime.Now;
        }
        public void SetCompleted()
        {
            State = ToDoItemState.Completed;
            StateChangedAt = DateTime.Now;
        }

        public override void Execute(ITelegramBotClient botClient, Message botMessage)
        {
            botClient.SendMessage(botMessage.Chat, Name);
        }

        public override string GetInfo()
        {
            return $"{Name} - {CreatedAt.ToString("dd.MM.yyyy hh:mm:ss")} - {Id}";
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

        public override string GetCode()
        {
            return Name;
        }
        public override bool IsEnabled(long telegramUserId)
        {
            return true;
        }
    }
}
