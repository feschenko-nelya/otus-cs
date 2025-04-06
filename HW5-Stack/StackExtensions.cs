using System;

namespace HW5
{
	public static class StackExtensions
	{
		public static void Merge(this OtusStack s1, OtusStack s2)
		{
			for (int i = s2.Size - 1; i >= 0; --i)
			{
				string str = s2.Pop();
				s1.Add(str);
			}
		}
	}
}
