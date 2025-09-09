

namespace HW2.Helpers
{
    public static class EnumerableExtension
    {
        public static IEnumerable<T> GetBatchByNumber<T>(this IEnumerable<T> enums, int batchSize, int batchNumber)
        {
            if ((batchSize == 0) || (enums.Count() == 0))
                return enums;

            return enums.Skip(batchNumber * batchSize).Take(batchSize);
        }
    }
}
