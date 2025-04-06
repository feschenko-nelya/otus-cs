using System;

namespace HW5
{
	public static class StackExtensions
	{
		public static void Merge(this AbstractStack s1, AbstractStack s2)
		{
			for (int i = s2.Size - 1; i >= 0; --i)
			{
				s1.Add(s2.Pop());
			}
		}

    }
}
