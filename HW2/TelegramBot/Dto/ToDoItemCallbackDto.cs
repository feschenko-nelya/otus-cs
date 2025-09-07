
using System.Text;

namespace HW2.TelegramBot.Dto
{
    internal class ToDoItemCallbackDto : CallbackDto
    {
        public Guid? ToDoItemId = null;
        public ToDoItemCallbackDto() { }

        public static new ToDoItemCallbackDto FromString(string input)
        {
            ToDoItemCallbackDto result = new();

            var values = input.Split('|');
            result.Action = values.First();

            if (values.Length > 1)
            {
                Guid id;
                if (Guid.TryParse(values[1], out id))
                    result.ToDoItemId = id;
            }

            return result;
        }

        public override string ToString()
        {
            StringBuilder strb = new();
            strb.Append(Action);

            if (ToDoItemId != null)
            {
                strb.Append($"|{ToDoItemId.ToString()}");
            }

            return strb.ToString();
        }
    }
}
