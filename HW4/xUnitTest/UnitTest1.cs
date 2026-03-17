using MultiThreadSum;

namespace xUnitTest
{
    public class UnitTest1
    {
        [Fact]
        public void TestSimpleCalculator()
        {
            SimpleIntCalculator threadCalcer = new();
            long sum = threadCalcer.Calculate(1, 50);

            Assert.Equal(1275, sum);
        }

        [Fact]
        public void TestThreadCalculator()
        {
            ThreadIntCalculator threadCalcer = new();
            long sum = threadCalcer.Calculate(1, 50);

            Assert.Equal(1275, sum);
        }

        [Fact]
        public void TestParallelCalculator()
        {
            ParallelIntCalculator parallelCalcer = new();
            long sum = parallelCalcer.Calculate(1, 50);

            Assert.Equal(1275, sum);
        }

    }
}