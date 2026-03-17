
namespace MultiThreadSum
{
    internal class ConsoleLogger : ILogger
    {
        public Task Log(string message)
        {
            Console.WriteLine(message);

            return Task.CompletedTask;
        }
    }
}
