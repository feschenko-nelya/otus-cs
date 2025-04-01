using HW3;

namespace TestPullRequest
{

}

namespace HW2
{
    internal class Program
    {
        static void Main()
        {
            ProgramInfo.mainCommands = [new StartCommand(),
                                        new EchoCommand(),
                                        new AddTaskCommand(),
                                        new ShowTasksCommand(),
                                        new RemoveTaskCommand(),
                                        new HelpCommand(),
                                        new InfoCommand(),
                                        new EndCommand(),];

            var inviteCommand = new InviteCommand();
            var invokeCommand = new InvokeCommand();

            while (ProgramInfo.state != ProgramInfo.State.Finished)
            {
                inviteCommand.Execute();
                invokeCommand.Execute();
            }
        }
    }
}
