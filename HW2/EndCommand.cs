using System;
using System.Xml.Linq;

namespace HW3
{
	public class EndCommand : AbstractCommand
    {
		public EndCommand()
		{
		}
        public override string GetCode()
        {
            return "/exit";
        }

        public override void Execute(string arg)
        {
            ProgramInfo.state = ProgramInfo.State.Finished;

            Console.Write("До свидания");
            if (string.IsNullOrEmpty(ProgramInfo.userName))
            {
                Console.WriteLine(".");
            }
            else
            {
                Console.WriteLine($", {ProgramInfo.userName}.");
            }
        }

        public override string GetInfo()
        {
            return "Завершение работы программы";
        }
    }
}