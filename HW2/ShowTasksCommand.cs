using System;

namespace HW3
{
	public class ShowTasksCommand : AbstractCommand
	{
		public ShowTasksCommand()
		{
		}

        public override string GetCode()
		{
			return "/showtasks";
		}
		public override void Execute(string arg)
		{
			if (ProgramInfo.userCommands.Count == 0)
			{
				Console.WriteLine("Список задач пуст.");
			}
			else
			{
				for (int i = 0; i < ProgramInfo.userCommands.Count; ++i)
				{
					var command = ProgramInfo.userCommands.ElementAt(i);
					Console.WriteLine($"{i + 1}. {command.GetInfo()}");
				}
			}
		}
        public override string GetInfo()
		{
			return "Список задач пользователя.";
		}
    }
}
