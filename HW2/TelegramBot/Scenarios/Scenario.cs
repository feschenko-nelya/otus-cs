namespace HW2.TelegramBot.Scenario
{
    public enum ScenarioType
    {
        None,
        AddTask
    }
    internal class ScenarioContext
    {
        // Id пользователя в Telegram
        internal long UserId;
        internal ScenarioType CurrentScenario;
        //Текущий шаг сценария
        internal string? CurrentStep = null;
        //Дополнительная инфрмация, необходимая для работы сценария
        internal Dictionary<string, object> Data = new();

        internal ScenarioContext(ScenarioType scenario)
        {
            CurrentScenario = scenario;
        }
    }
}
