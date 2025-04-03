using System;
using HW3;

namespace HW4
{
	public class TasksMaxNumberCommand : AbstractCommand
	{
		public TasksMaxNumberCommand()
		{
		}

        public override void Execute(string arg)
        {
            Console.WriteLine("Введите максимально допустимое количество задач");
            GetNumberCommand getNumberCommand = new(1, 100);
            getNumberCommand.Execute();
            ProgramInfo.tasksMaxLimit = getNumberCommand.Number;

            if (ProgramInfo.tasksMaxLimit == 0)
            {
                ProgramInfo.state = ProgramInfo.State.Finished;
            }
        }

        public override string GetCode()
        {
            return "/tasksmaxnumber";
        }

        public override string GetInfo()
        {
            return "Максимально допустимое количество задач";
        }
    }
}
