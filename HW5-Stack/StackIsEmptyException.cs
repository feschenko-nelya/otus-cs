using System;

namespace HW5
{
    public class StackIsEmptyException : Exception
    {
        public StackIsEmptyException() : base("Стек пустой.")
        {
        }
    }
}
