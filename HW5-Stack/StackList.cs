using System;

namespace HW5
{
	public class StackList : AbstractStack
	{
		private List<string> _stack = new();

		public StackList()
		{
		}

		public StackList(params string[] strs)
		{
			for (int i = 0; i < strs.Length; ++i)
			{
				strs[i] = strs[i];

				Add(strs[i]);
			}
		}

		public override void Add(string str) 
		{
			_stack.Add(str);
			++_size;

        }

		public override string Pop()
		{
			if (_size <= 0)
			{
				throw new StackIsEmptyException();

            }

			--_size;

			string result = _stack.Last();
			_stack.RemoveAt(_size);

			return result;
		}

		public override void Print()
		{
			for (int i = 0; i < _size; ++i)
			{
				Console.Write(_stack.ElementAt(i) + " ");
			}
			Console.WriteLine();
		}

        protected override string? GetTop()
		{
			if (_size <= 0)
			{
				return null;
			}

			return _stack.ElementAt(_size - 1);
        }
        static public StackList Concat(params StackList[] ss)
        {
            StackList result = new();

            foreach (StackList s in ss)
            {
                for (int i = s.Size - 1; i >= 0; --i)
                {
                    result.Add(s.Pop());
                }
            }

            return result;
        }
    }
}
