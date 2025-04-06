using HW5;

namespace HW5
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Base task
            {
                //var s1 = new OtusStack("a", "b", "c");
                //var s2 = new StackRef("a", "b", "c");

                //BaseTask(s1);
                //Console.WriteLine("***");
                //BaseTask(s2);
            }

            // Task 1
            {
                //AbstractStack s = new OtusStack("a", "b", "c");
                //ExtTask1(ref s, new OtusStack("1", "2", "3"));

                //Console.WriteLine("***");

                //s = new StackRef("a", "b", "c");
                //ExtTask1(ref s, new StackRef("1", "2", "3"));
            }


            // Task 2
            {
                //ExtTask2StackList();

                //Console.WriteLine("***");

                //ExtTask2StackRef();

            }

        }

        public static void BaseTask(AbstractStack s)
        {
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

        public static void ExtTask1(ref AbstractStack s1, AbstractStack s2)
        {
            s1.Merge(s2);

            // в стеке s теперь элементы - "a", "b", "c", "3", "2", "1" <- верхний
            s1.Print();
            Console.WriteLine($"верхний: {s1.Top}");
        }

        public static void ExtTask2StackList()
        {
            var s = StackList.Concat(new StackList("a", "b", "c"), 
                                     new StackList("1", "2", "3"), 
                                     new StackList("А", "Б", "В"));


            // в стеке s теперь элементы -  "c", "b", "a" "3", "2", "1", "В", "Б", "А" <- верхний
            s.Print();
            Console.WriteLine($"верхний: {s.Top}");
        }

        public static void ExtTask2StackRef()
        {
            var s = StackRef.Concat(new StackRef("a", "b", "c"),
                                    new StackRef("1", "2", "3"),
                                    new StackRef("А", "Б", "В"));


            // в стеке s теперь элементы -  "c", "b", "a" "3", "2", "1", "В", "Б", "А" <- верхний
            s.Print();
            Console.WriteLine($"верхний: {s.Top}");
        }
    }
}
