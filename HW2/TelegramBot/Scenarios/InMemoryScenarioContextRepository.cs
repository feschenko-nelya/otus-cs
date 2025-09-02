
namespace HW2.TelegramBot.Scenario
{
    internal class InMemoryScenarioContextRepository : IScenarioContextRepository
    {
        private Dictionary<long, ScenarioContext> _scenarios = new();
        public Task<ScenarioContext?> GetContext(long userId, CancellationToken ct)
        {
            ScenarioContext? context = null;
            if (_scenarios.TryGetValue(userId, out context))
            {
                return Task.FromResult<ScenarioContext?>(context);
            }

            return Task.FromResult<ScenarioContext?>(null);
        }

        public Task ResetContext(long userId, CancellationToken ct)
        {
            _scenarios.Remove(userId);

            return Task.CompletedTask;
        }

        public Task SetContext(long userId, ScenarioContext context, CancellationToken ct)
        {
            _scenarios[userId] = context;

            return Task.CompletedTask;
        }
    }
}
