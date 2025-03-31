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
				string[] args = userRequest.Split(' ');

				int argsCount = args.Count();

                if (argsCount > 0)
				{
					AbstractCommand? command = ProgramInfo.mainCommands.Get(args[0]);

					if (command != null)
					{
						if (argsCount == 1)
						{
							command.Execute();
                        }
						else
						{
							command.Execute(args[1]);
                        }

						Console.ReadLine();
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