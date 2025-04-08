using System;
using System.Text;

namespace HW5
{
	public class StackRef : AbstractStack
	{
		private class StackItem
		{
			public string value = "";
			public StackItem? previous = null;
		}

		private StackItem? _top = null;

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

        protected override string? GetTop()
        {
            if (_top == null)
            {
                return null;
            }

            return _top.value;
        }

        public override void Add(string str)
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

        public override string Pop()
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

        public override void Print()
        {
            StackItem? curItem = _top;

            StringBuilder valuesStr = new();

            while (curItem != null)
            {
                valuesStr.Insert(0, " ");
                valuesStr.Insert(0, curItem.value);

                curItem = curItem.previous;
            }

            Console.WriteLine(valuesStr);
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
    }
}
