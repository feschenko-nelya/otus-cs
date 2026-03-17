
namespace MultiThreadSum
{
    public class ParallelIntCalculator : IIntCalculator
    {
        public long Calculate(int n, int m)
        {
            return Enumerable.Range(n, m).AsParallel().WithDegreeOfParallelism(Environment.ProcessorCount).Sum(v => (long)v);
        }

        override public string ToString()
        {
            return "parallel";
        }
    }
}
