

namespace HW2.Helpers
{
    public static class EnumerableExtension
    {
        public static IEnumerable<T> GetBatchByNumber<T>(this IEnumerable<T> enums, int batchSize, int batchNumber)
        {
            if (batchSize == 0)
                return enums;

            if (enums.Count() == 0)
                return enums;

            int start = batchNumber * batchSize;
            int end = Math.Min(start + batchSize, enums.Count());

            T[] result = new T[end - start];
            for (int i = start; i < end; i++)
            {
                result[i - start] = enums.ElementAt(i);
            }

            return result;
        }
    }
}
