using System;

namespace HW3
{
	public class InvokeCommand : AbstractCommand
	{
		public InvokeCommand()
		{
		}
        public override string GetCode()
		{
			return "/invoke";
		}

        public override void Execute(string arg)
		{
            string? userRequest = Console.ReadLine();
            Console.Clear();

            if (!string.IsNullOrEmpty(userRequest))
			{
				int spaceIndex = userRequest.IndexOf(' ');

                if (spaceIndex < 0)
				{
                    AbstractCommand? command = ProgramInfo.mainCommands.Get(userRequest);
					if ((command != null) && command.IsEnabled())
					{
						command.Execute();
                    }
                }
				else
				{
                    string commandCode = userRequest.Substring(0, spaceIndex);

                    AbstractCommand? command = ProgramInfo.mainCommands.Get(commandCode);
					if ((command != null) && command.IsEnabled())
					{
						command.Execute(userRequest.Substring(spaceIndex + 1));
                    }
                }
			}
        }
        public override string GetInfo()
		{
			return "Запрос команды.";
		}
    }
}