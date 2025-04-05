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

		public bool Contains(string name)
		{
			foreach (var command in this)
			{
				if (command.GetInfo() == name)
				{
					return true;
				}
			}

			return false;
		}
	}
}