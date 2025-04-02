using System;
using HW4;

namespace HW3
{
	public class RemoveTaskCommand : AbstractCommand
	{
		private ShowTasksCommand _showTasksCommand = new();
		public RemoveTaskCommand()
		{
		}

        public override string GetCode()
		{
			return "/removetask";
		}

        public override void Execute(string arg)
		{
			if (ProgramInfo.userCommands.Count == 0)
			{
				Console.WriteLine("Список задач пуст.");
            }
            else
			{
				_showTasksCommand.Execute();
                Console.WriteLine("-----------");
                Console.WriteLine("0. Отменить");

                Console.Write("Введите номер задачи, которую нужно удалить: ");

				GetNumberCommand getNumberCommand = new(0, ProgramInfo.userCommands.Count);
				getNumberCommand.Execute();

                int number = getNumberCommand.Number;

                if (number == 0)
                {
                    return;
                }

                string taskName = ProgramInfo.userCommands.ElementAt(number - 1).GetInfo();
                ProgramInfo.userCommands.RemoveAt(number - 1);
                Console.WriteLine($"Задача \"{taskName}\" удалена.");
			}
		}

        public override string GetInfo()
		{
			return "Удаление задачи пользователя.";
		}
    }
}
