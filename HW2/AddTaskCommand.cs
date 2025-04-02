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
            if (ProgramInfo.userCommands.Count == ProgramInfo.tasksMaxLimit)
            {
                throw new TaskCountLimitException(ProgramInfo.tasksMaxLimit);
            }

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

    public class TaskCountLimitException : Exception
    {
        public TaskCountLimitException(int taskCountLimit) 
               : base($"Превышено максимальное количество задач равное {taskCountLimit}")
        {

        }
    }
}