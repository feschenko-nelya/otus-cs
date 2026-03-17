
using System.Diagnostics;

namespace MultiThreadSum
{
    public class IntCalculatorManager
    {
        private List<IIntCalculator> _calculators = new();
        private List<ILogger> _loggers = new();

        public void Calculate(int N, int M)
        {
            Log($"Sum of {N} and {M}");

            Stopwatch timer = new();

            foreach (var calculator in _calculators)
            {
                timer.Restart();
                long sum = calculator.Calculate(N, M);

                timer.Stop();

                Log($"{calculator.ToString()} calcer: {sum}, time: {timer.Elapsed} ms");
            }

            Log("");
        }

        public void AddCalculator(IIntCalculator calculator)
        {
            _calculators.Add(calculator);
        }

        public void AddLogger(ILogger logger)
        {
            _loggers.Add(logger);
        }

        private void Log(string message)
        {
            foreach (var logger in _loggers)
            {
                logger.Log(message);
            }
        }
    }
}
