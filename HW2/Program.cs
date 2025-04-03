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
                init();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
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
                catch (ArgumentException exception)
                {
                    Console.WriteLine(exception.Message);
                }
                catch (TaskCountLimitException exception)
                {
                    Console.WriteLine(exception.Message);
                }
                catch (TaskLengthLimitException exception)
                {
                    Console.WriteLine(exception.Message);
                }
                catch (DuplicateTaskException exception)
                {
                    Console.WriteLine(exception.Message);
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"""
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

                Console.ReadLine();
            }
        }

        private static void init()
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
    }
}
