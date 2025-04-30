using System;

namespace HW5
{
	public abstract class AbstractStack
	{
        protected int _size = 0;
        public int Size => _size;
        public string? Top => GetTop();

        public abstract void Add(string str);
        public abstract string Pop();
        public abstract void Print();
        protected abstract string? GetTop();
    }
}
