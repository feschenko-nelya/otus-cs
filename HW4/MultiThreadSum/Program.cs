
namespace MultiThreadSum
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IntCalculatorManager calcer = new();

            calcer.AddCalculator(new SimpleIntCalculator());
            calcer.AddCalculator(new ThreadIntCalculator());
            calcer.AddCalculator(new ParallelIntCalculator());

            calcer.AddLogger(new ConsoleLogger());
            calcer.AddLogger(new FileLogger());

            calcer.Calculate(1, 100_000);
            calcer.Calculate(1, 1_000_000);
            calcer.Calculate(1, 10_000_000);
        }
    }
}
