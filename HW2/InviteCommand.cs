using System;

namespace HW3
{
	public class InviteCommand : AbstractCommand
	{
        private bool _isActivated = false;
		public InviteCommand()
		{
		}
        public override string GetCode()
        {
            return "/invite";
        }

        public override void Execute(string arg)
		{
            Console.Clear();

            if (!_isActivated)
            {
                Console.WriteLine("Здравствуйте.");
                _isActivated = true;
            }

            if (string.IsNullOrEmpty(ProgramInfo.userName))
            {
                Console.WriteLine("Введите одну из команд:");
            }
            else
            {
                Console.WriteLine($"{ProgramInfo.userName}, введите одну из команд:");
            }

            foreach (var command in ProgramInfo.commands)
            {
                if (command.IsEnabled())
                {
                    Console.WriteLine(command.GetCode());
                }
            }
            Console.WriteLine();
        }

        public override string GetInfo()
		{
			return "Приветствие";
		}
    }
}
