
namespace HW2.TelegramBot.Scenario
{
    public enum ScenarioType
    {
        None,
        AddTask,
        DeleteTask,
        AddList,
        DeleteList
    }
    public class ScenarioContext
    {
        // Id пользователя в Telegram
        public long UserId { get; set; }
        public ScenarioType CurrentScenario;
        //Текущий шаг сценария
        public string? CurrentStep = null;
        //Дополнительная инфрмация, необходимая для работы сценария
        public Dictionary<string, object> Data = new();
        public DateTime CreatedAt { get; }

        public ScenarioContext(ScenarioType scenario, long userId)
        {
            CurrentScenario = scenario;
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
