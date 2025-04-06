using System;
using HW3;

namespace HW4
{
	public class UserTasksLimitNumberCommand : AbstractCommand
	{
		public UserTasksLimitNumberCommand()
		{
		}

        public override string GetCode()
		{
			return "/settaskslimit";
		}

        public override void Execute(string arg)
		{
			Console.WriteLine("Введите максимально допустимое количество задач от 1 до 100:");

            string? snumber = string.Empty;
            int inumber = -1;

            int commandsCount = 100;

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
        }

        public override string GetInfo()
		{
			return "Ограничение на максимальное количество задач";
		}
    }
}
