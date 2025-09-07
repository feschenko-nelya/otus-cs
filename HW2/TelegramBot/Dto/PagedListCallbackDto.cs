
namespace HW2.TelegramBot.Dto
{
    public class PagedListCallbackDto : ToDoListCallbackDto
    {
        public int Page { get; set; } = 0;
        public PagedListCallbackDto() { }

        public static new PagedListCallbackDto FromString(string input)
        {
            PagedListCallbackDto result = new();

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

            if (values.Length > 2)
            {
                int page;
                if (int.TryParse(values[2], out page))
                    result.Page = page;
            }

            return result;
        }

        public override string ToString()
        {
            return $"{base.ToString()}|{Page}";
        }
    }
}
