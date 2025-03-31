using System;

namespace HW3
{
    public class AddTaskCommand : AbstractCommand
    {
	    public AddTaskCommand()
	    {
	    }

        public override string GetCode()
        {
            return "/addtask";
        }
        public override void Execute(string arg)
        {
            Console.Write("Пожалуйста, введите описание задачи: ");
            string? taskName = null;

            while (string.IsNullOrEmpty(taskName))
            {
                taskName = Console.ReadLine();
            }

            ProgramInfo.userCommands.Add(new UserCommand(taskName));

            Console.WriteLine($"Задача \"{taskName}\" добавлена.");
        }
        public override string GetInfo()
        {
            return "Добавление новой задачи.";
        }
    }
}