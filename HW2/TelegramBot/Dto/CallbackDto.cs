
namespace HW2.TelegramBot.Dto
{
    public class CallbackDto
    {
        // с помощью него будет определять за какое действие отвечает кнопка
        public string Action { get; set; } = "";

        // На вход принимает строку ввида "{action}|{prop1}|{prop2}...".
        // Нужно создать CallbackDto с Action = action.
        // Нужно учесть что в строке может не быть |, тогда всю строку сохраняем в Action.
        public static CallbackDto FromString(string input)
        {
            CallbackDto result = new();

            result.Action = input.Split('|').First();

            return result;
        }

        public override string ToString()
        {
            return Action;
        }
    }
}
