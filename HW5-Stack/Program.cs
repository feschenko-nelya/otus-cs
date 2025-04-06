using HW5;

namespace HW5
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //BaseTask();

            //ExtTask1();

            //ExtTask2();

            ExtTask3();
        }

        public static void BaseTask()
        {
            
            var s = new OtusStack("a", "b", "c");

            // size = 3, Top = 'c'
            Console.WriteLine($"size = {s.Size}, Top = '{s.Top}'");

            var deleted = s.Pop();

            // Извлек верхний элемент 'c' Size = 2
            Console.WriteLine($"Извлек верхний элемент '{deleted}' Size = {s.Size}");
            s.Add("d");

            // size = 3, Top = 'd'
            Console.WriteLine($"size = {s.Size}, Top = '{s.Top}'");

            s.Pop();
            s.Pop();
            s.Pop();
            // size = 0, Top = null
            Console.WriteLine($"size = {s.Size}, Top = {(s.Top == null ? "null" : s.Top)}");

            try
            {
                s.Pop();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        public static void ExtTask1()
        {
            var s = new OtusStack("a", "b", "c");


            s.Merge(new OtusStack("1", "2", "3"));

            // в стеке s теперь элементы - "a", "b", "c", "3", "2", "1" <- верхний
            s.printStack();
            Console.WriteLine($"верхний: {s.Top}");
        }

        public static void ExtTask2()
        {
            var s = OtusStack.Concat(new OtusStack("a", "b", "c"), 
                                     new OtusStack("1", "2", "3"), 
                                     new OtusStack("А", "Б", "В"));


            // в стеке s теперь элементы -  "c", "b", "a" "3", "2", "1", "В", "Б", "А" <- верхний
            s.printStack();
            Console.WriteLine($"верхний: {s.Top}");
        }

        public static void ExtTask3()
        {
            var s = new StackRef("a", "b", "c");

            // size = 3, Top = 'c'
            Console.WriteLine($"size = {s.Size}, Top = '{s.Top}'");

            var deleted = s.Pop();

            // Извлек верхний элемент 'c' Size = 2
            Console.WriteLine($"Извлек верхний элемент '{deleted}' Size = {s.Size}");
            s.Add("d");

            // size = 3, Top = 'd'
            Console.WriteLine($"size = {s.Size}, Top = '{s.Top}'");

            s.Pop();
            s.Pop();
            s.Pop();
            // size = 0, Top = null
            Console.WriteLine($"size = {s.Size}, Top = {(s.Top == null ? "null" : s.Top)}");

            try
            {
                s.Pop();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}
