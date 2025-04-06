using System;

namespace HW5
{
	public class StackRef
	{
		private class StackItem
		{
			public string value = "";
			public StackItem? previous = null;
		}

		private StackItem? _top = null;
		private int _size = 0;
        public string? Top
        {
            get
            {
                if (_top == null)
                {
                    return null;
                }

                return _top.value;
            }
        }

        public int Size
        {
            get
            {
                return _size;
            }
        }

        public StackRef()
		{
		}

        public StackRef(params string[] strs)
        {
            for (int i = 0; i < strs.Length; ++i)
            {
                strs[i] = strs[i];

                Add(strs[i]);
            }
        }

        public void Add(string str)
        {
            var newObj = new StackItem();
            newObj.value = str;

            if (_top != null)
			{
				newObj.previous = _top;
			}

			_top = newObj;

            ++_size;
        }

        public string Pop()
        {
            if ((_size <= 0) || (_top == null))
            {
                throw new StackIsEmptyException();

            }

            --_size;

            string lastValue = _top.value;
            StackItem? previousItem = _top.previous;

            _top = null;
            _top = previousItem;

            return lastValue;
        }

        static public StackRef Concat(params StackRef[] ss)
        {
            StackRef result = new();

            foreach (StackRef s in ss)
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
            StackItem? curItem = _top;

            while (curItem != null)
            {
                Console.Write(curItem.value + " ");

                curItem = curItem.previous;
            }
            Console.WriteLine();
        }
    }
}
