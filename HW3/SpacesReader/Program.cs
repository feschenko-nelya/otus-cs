using System.Diagnostics;

namespace SpacesReader
{
    internal class Program
    {
        private int _spacesNumber = 0;

        static void Main(string[] args)
        {
            Program prg = new();

            int spacesNumber = 0;

            Stopwatch sw = new Stopwatch();

            sw.Start();
            spacesNumber = prg.ReadThreeFilesByTask(args[0]);
            sw.Stop();
            
            Console.WriteLine($"Task read 3 files. Spaces count: {spacesNumber}. Time: {sw.Elapsed.TotalMicroseconds} ms.");

            sw.Restart();
            spacesNumber = prg.ReadDirectoryByTask(args[0]);
            sw.Stop();

            Console.WriteLine($"Task read directory. Spaces count: {spacesNumber}. Time: {sw.Elapsed.TotalMicroseconds} ms.");

            sw.Restart();
            spacesNumber = prg.ReadSpacesParallel(args[0]);
            sw.Stop();

            Console.WriteLine($"Parallel read directory. Spaces count: {spacesNumber}. Time: {sw.Elapsed.TotalMicroseconds} ms.");
        }

        public int ReadThreeFilesByTask(string directoryName)
        {
            _spacesNumber = 0;

            if (!Directory.Exists(directoryName))
            {
                Console.WriteLine($"Directory not exist: {directoryName}");

                return 0;
            }

            var files = Directory.GetFiles(directoryName);
            if (files.Length == 0)
            {
                Console.WriteLine($"Directory is empty: {directoryName}");

                return 0;
            }

            Task[] tasks = new Task[3];

            for (int i = 0; i < tasks.Length; ++i)
            {
                tasks[i] = new Task(() => CountSpaces(files.ElementAt(i)));
                tasks[i].Start();
            }

            Task.WaitAll(tasks);

            return _spacesNumber;
        }

        public int ReadDirectoryByTask(string directoryName)
        {
            _spacesNumber = 0;

            if (!Directory.Exists(directoryName))
            {
                Console.WriteLine($"Directory not exist: {directoryName}");

                return 0;
            }

            var files = Directory.GetFiles(directoryName);
            if (files.Length == 0)
            {
                Console.WriteLine($"Directory is empty: {directoryName}");

                return 0;
            }

            Task[] tasks = new Task[files.Count()];

            int i = 0;
            foreach (var file in files) 
            {
                tasks[i] = new Task(() => CountSpaces(file));
                tasks[i].Start();

                ++i;
            }

            Task.WaitAll(tasks);

            return _spacesNumber;
        }

        public int ReadSpacesParallel(string directoryName)
        {
            _spacesNumber = 0;

            if (!Directory.Exists(directoryName))
            { 
                Console.WriteLine($"Directory not exist: {directoryName}");

                return 0;
            }

            var files = Directory.GetFiles(directoryName);
            if (files.Length == 0)
            {
                Console.WriteLine($"Directory is empty: {directoryName}");

                return 0;
            }

            ParallelLoopResult res = Parallel.ForEach<string>(files, (f) => CountSpaces(f));

            return res.IsCompleted ? _spacesNumber : 0;
        }

        public void CountSpaces(string file)
        {
            if (!File.Exists(file))
            {
                Console.WriteLine($"File not exist: {file}");

                return;
            }

            using (StreamReader sr = new StreamReader(file))
            {
                string? line = string.Empty;

                while ((line = sr.ReadLine()) != null)
                {
                    for (int i = 0; i < line.Length; ++i)
                    {
                        if (line[i] == ' ')
                        {
                            _spacesNumber++;
                        }
                    }
                }
            }
        }
    }
}
