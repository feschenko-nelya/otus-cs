using System;

namespace HW3
{
	public class CommandContainer : List<AbstractCommand>
	{
        public CommandContainer()
		{
		}

		public AbstractCommand? Get(string code)
		{
			foreach (var command in this)
			{
				if (command.GetCode() == code)
				{
					return command;
				}
			}

			return null;
		}
	}
}