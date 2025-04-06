using System;
using System.Reflection.Metadata.Ecma335;

namespace HW5
{
	public class OtusStack
	{
		private List<string> _stack = new();
		private int _size = 0;
		public int Size { get => _size; }
		public string? Top
		{
			get
			{
				if (_size <= 0)
				{
					return null;
				}

				return _stack.ElementAt(_size - 1);
            }
		}

		public OtusStack()
		{
		}

		public OtusStack(params string[] strs)
		{
			for (int i = 0; i < strs.Length; ++i)
			{
				strs[i] = strs[i];

				Add(strs[i]);
			}
		}

		public void Add(string str)
		{
			_stack.Add(str);
			++_size;

        }

		public string Pop()
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

		static public OtusStack Concat(params OtusStack[] ss)
		{
			OtusStack result = new();

            foreach (OtusStack s in ss)
			{
				for (int i = s.Size - 1; i >= 0; --i)
				{
					result.Add(s.Pop());
				}
			}

			return result;
		}

		public void printStack()
		{
			for (int i = 0; i < _size; ++i)
			{
				Console.Write(_stack.ElementAt(i) + " ");
			}
			Console.WriteLine();
		}
	}

	public class StackIsEmptyException : Exception
	{
		public StackIsEmptyException() : base("Стек пустой.")
		{

		}
    }
}
