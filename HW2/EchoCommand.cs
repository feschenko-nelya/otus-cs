using System;

namespace HW3
{
	public class EchoCommand : AbstractCommand
    {
		public EchoCommand()
		{
		}
        public override string GetCode()
        {
            return "/echo";
        }

        public override void Execute(string arg)
		{
			if (!IsEnabled())
				return;

			Console.WriteLine(arg);
		}

        public override string GetInfo()
		{
			return "Повторный вывод введенного пользователем текста после названия команды через пробел (доступно после команды /start);";
		}

        public override bool IsEnabled()
		{
			return (ProgramInfo.state == ProgramInfo.State.Started);

        }
    }
}
