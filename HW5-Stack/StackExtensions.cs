using System;

namespace HW5
{
	public static class StackExtensions
	{
		public static void Merge(this OtusStack s1, OtusStack s2)
		{
			for (int i = s2.Size - 1; i >= 0; --i)
			{
				s1.Add(s2.Pop());
			}
		}

		public static void Merge(this StackRef s1, StackRef s2)
        {
            for (int i = s2.Size - 1; i >= 0; --i)
            {
                s1.Add(s2.Pop());
            }
        }
    }
}
