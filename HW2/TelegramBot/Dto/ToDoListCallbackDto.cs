
using System.Text;

namespace HW2.TelegramBot.Dto
{
    public class ToDoListCallbackDto : CallbackDto
    {
        public Guid? ToDoListId { get; set; }

        // На вход принимает строку ввида "{action}|{toDoListId}|{prop2}...".
        // Нужно создать ToDoListCallbackDto с Action = action и ToDoListId = toDoListId.
        public static new ToDoListCallbackDto FromString(string input)
        {
            ToDoListCallbackDto result = new();

            var values = input.Split('|');

            if (values.Length > 0)
            {
                result.Action = values[0];
            }

            if (values.Length > 1)
            {
                Guid guid;
                if (Guid.TryParse(values[1], out guid))
                    result.ToDoListId = guid;
            }

            return result;
        }

        public override string ToString()
        {
            return $"{base.ToString()}|{ToDoListId}";
        }
    }
}
