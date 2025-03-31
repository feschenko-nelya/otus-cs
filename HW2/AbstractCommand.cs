using System;

namespace HW3
{
	public abstract class AbstractCommand
	{
		public abstract string GetCode();
		public abstract void Execute(string arg);
		public abstract string GetInfo();
		public virtual bool IsEnabled()
		{
			return true;
		}
	}
}
