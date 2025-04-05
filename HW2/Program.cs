using HW3;
using HW4;

namespace HW2
{
    internal class Program
    {
        static void Main()
        {
            try
            {
                Init();
            }
            catch (Exception exception)
            {
                ShowException(exception.Message);
                return;
            }

            var inviteCommand = new InviteCommand();
            var invokeCommand = new InvokeCommand();

            while (ProgramInfo.state != ProgramInfo.State.Finished)
            {
                try
                {
                    inviteCommand.Execute();
                    invokeCommand.Execute();
                }
                catch (Exception exception)
                {
                    ProcessException(exception);
                }

                Console.ReadLine();
            }
        }

        private static void Init()
        {
            ProgramInfo.mainCommands = [new StartCommand(),
                                        new EchoCommand(),
                                        new AddTaskCommand(),
                                        new ShowTasksCommand(),
                                        new RemoveTaskCommand(),
                                        new HelpCommand(),
                                        new InfoCommand(),
                                        new EndCommand(),];

            TasksMaxNumberCommand tasksMaxNumberCommand = new TasksMaxNumberCommand();
            tasksMaxNumberCommand.Execute();

            TaskMaxLengthCommand taskMaxLengthCommand = new TaskMaxLengthCommand();
            taskMaxLengthCommand.Execute();
        }

        private static void ProcessException(Exception exception)
        {
            if ((exception.GetType() == typeof(ArgumentException))
                || (exception.GetType() == typeof(TaskCountLimitException))
                || (exception.GetType() == typeof(TaskLengthLimitException))
                || (exception.GetType() == typeof(DuplicateTaskException))
               )
            {
                ShowException(exception.Message);
            }
            else
            {
                ShowException($"""
                        Произошла непредвиденная ошибка.
                        Детальная информация:

                        Type: {exception.GetType}
                        Message: {exception.Message}
                        Stack trace: 
                        {exception.StackTrace}
                        Inner exception: {exception.InnerException}
                        """);

                ProgramInfo.state = ProgramInfo.State.Finished;
            }
        }

        private static void ShowException(string message)
        {
            Console.WriteLine();
            Console.WriteLine(message);
        }
    }
}
