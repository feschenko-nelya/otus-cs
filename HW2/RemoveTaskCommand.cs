using System;

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
				string? snumber = string.Empty;
				int inumber = -1;

				int commandsCount = ProgramInfo.userCommands.Count;

                while (string.IsNullOrEmpty(snumber) || inumber == -1)
				{
					snumber = Console.ReadLine();

                    if (!int.TryParse(snumber, out inumber))
					{
						Console.WriteLine($"Введите число от {1} до {commandsCount}.");
                        inumber = -1;
                    }
					else if (inumber == 0)
					{
						return;
					}
					else if ((inumber < 0) || (inumber > commandsCount))
					{
                        Console.WriteLine($"Задачи с номером {inumber} нет. Введите существующий номер из списка.");
                        inumber = -1;
                    }
                }

				string taskName = ProgramInfo.userCommands.ElementAt(inumber - 1).GetInfo();
                ProgramInfo.userCommands.RemoveAt(inumber - 1);
                Console.WriteLine($"Задача \"{taskName}\" удалена.");
			}
		}

        public override string GetInfo()
		{
			return "Удаление задачи пользователя.";
		}
    }
}
