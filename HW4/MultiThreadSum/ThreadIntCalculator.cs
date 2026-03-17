

namespace MultiThreadSum
{
    public class ThreadIntCalculator : IIntCalculator
    {
        public long Calculate(int n, int m)
        {
            int cores = Environment.ProcessorCount;
            int chunk = (m - n + 1) / cores;

            long resultSum = 0;

            object lockObj = new();
            List<Thread> threads = new List<Thread>(cores);

            for (int i = 0; i <= cores; i++)
            {
                int start = n + i * chunk;
                int end = Math.Min(start + chunk - 1, m);

                var threadd = new Thread(() =>
                {
                    long localSum = 0;
                    for (int j = start; j <= end; j++)
                    {
                        localSum += j;
                    }

                    lock (lockObj)
                    {
                        resultSum += localSum;
                    }
                });

                threads.Add(threadd);
                threads.Last().Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            return resultSum;
        }

        override public string ToString()
        {
            return "thread";
        }
    }
}
