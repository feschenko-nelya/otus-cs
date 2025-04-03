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

            if (taskName.Length > ProgramInfo.tasksMaxLength)
            {
                throw new TaskLengthLimitException(taskName.Length, ProgramInfo.tasksMaxLength);
            }

            if (ProgramInfo.userCommands.Contains(taskName))
            {
                throw new DuplicateTaskException(taskName);
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
    public class TaskLengthLimitException : Exception
    {
        public TaskLengthLimitException(int taskLength, int taskLengthLimit)
               : base($"Длина задачи ‘{taskLength}’ превышает максимально допустимое значение {taskLengthLimit}.")
        {
        }
    }
    public class DuplicateTaskException : Exception
    {
        public DuplicateTaskException(string task)
               : base($"Задача ‘{task}’ уже существует.")
        {
        }
    }
}