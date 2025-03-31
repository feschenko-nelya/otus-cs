using System;
using System.Text;

namespace HW3
{
	public class HelpCommand : AbstractCommand
	{
		public HelpCommand()
		{

		}
        public override string GetCode()
		{
			return "/help";
		}
        public override void Execute(string arg)
		{
            Console.WriteLine(GetInfo());

            foreach (var command in ProgramInfo.commands)
            {
				StringBuilder str = new();
				str.Append(command.GetCode());
				str.Append(" - ");
                str.Append(command.GetInfo());

                Console.WriteLine(str.ToString());
            }
        }
        public override string GetInfo()
		{
			return "Краткая информация о программе";
		}
    }
}