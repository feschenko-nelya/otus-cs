using System;

namespace HW3
{
	public class InfoCommand : AbstractCommand
	{
		public InfoCommand()
		{
		}

        public override string GetCode()
		{
			return "/info";
		}

        public override void Execute(string arg)
		{
			Console.WriteLine($"Version: {ProgramInfo.version.ToString()}");
            Console.WriteLine($"Creation date: {ProgramInfo.creationDate.ToString("dd.MM.yyyy")}");
        }

        public override string GetInfo()
		{
			return "Информация о версии и дате создания программы.";
		}
    }
}
