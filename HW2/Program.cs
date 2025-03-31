﻿using HW3;

namespace HW2
{
    internal class Program
    {
        static void Main()
        {
            ProgramInfo.commands = [new StartCommand(),
                                    new EchoCommand(),
                                    new HelpCommand(),
                                    new InfoCommand(),
                                    new EndCommand(),];

            var inviteCommand = new InviteCommand();
            var invokeCommand = new InvokeCommand();

            while (ProgramInfo.state != ProgramInfo.State.Finished)
            {
                inviteCommand.Execute("");
                invokeCommand.Execute("");
            }
        }
    }
}
