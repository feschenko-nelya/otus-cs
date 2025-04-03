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
                catch (ArgumentException exception)
                {
                    ShowException(exception.Message);
                }
                catch (TaskCountLimitException exception)
                {
                    ShowException(exception.Message);
                }
                catch (TaskLengthLimitException exception)
                {
                    ShowException(exception.Message);
                }
                catch (DuplicateTaskException exception)
                {
                    ShowException(exception.Message);
                }
                catch (Exception exception)
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

        private static void ShowException(string message)
        {
            Console.WriteLine();
            Console.WriteLine(message);
        }
    }
}
