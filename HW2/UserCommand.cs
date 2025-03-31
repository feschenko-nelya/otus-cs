using System;

namespace HW3
{
	public class UserCommand : AbstractCommand
	{
		private string _name = string.Empty;
		public UserCommand(string name)
		{
            _name = name;
		}

        public override string GetCode()
		{
            return _name;
        }
        public override void Execute(string arg)
		{
			Console.WriteLine(_name);
		}
        public override string GetInfo()
		{
			return _name;
		}
    }
}
