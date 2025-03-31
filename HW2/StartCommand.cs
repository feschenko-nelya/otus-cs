using System;
using System.Xml.Linq;

namespace HW3
{
	public class StartCommand : AbstractCommand
	{
		public StartCommand()
		{
		}
        public override string GetCode()
        {
            return "/start";
        }
        public override void Execute(string arg)
		{
            Console.WriteLine("Введите имя, по которому к Вам можно обратиться:");

            while (string.IsNullOrEmpty(ProgramInfo.userName))
            {
                ProgramInfo.userName = Console.ReadLine();
            }

            Console.Clear();
            Console.WriteLine("Здравствуйте, " + ProgramInfo.userName);

            ProgramInfo.state = ProgramInfo.State.Started;
        }
        public override string GetInfo()
		{
			return "Начало работы программы: ввод имени, по которому обращаться к пользователю;";
		}

        public override bool IsEnabled()
        {
            return (ProgramInfo.state == ProgramInfo.State.None);
        }
    }
}
