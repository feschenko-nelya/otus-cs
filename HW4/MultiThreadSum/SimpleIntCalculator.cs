
namespace MultiThreadSum
{
    public class SimpleIntCalculator : IIntCalculator
    {
        public long Calculate(int n, int m)
        {
            long result = 0;

            for (int i = n; i <= m; i++)
            {
                result += i;
            }

            return result;
        }

        override public string ToString()
        {
            return "simple";
        }
    }
}
