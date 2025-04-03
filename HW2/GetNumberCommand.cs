using System;
using HW3;

namespace HW4
{
    public class GetNumberCommand : AbstractCommand
    {
        private int _max = 0;
        private int _min = 0;
        private int _number = -1;
        public int Number 
        { 
            get { return _number; }
            private set { _number = value;} 
        }

        public GetNumberCommand(int min, int max)
		{
            _max = max;
            _min = min;
        }

        public override void Execute(string arg)
        {
            _number = -1;

            while (_number == -1)
            {
                try
                {
                    string? str = Console.ReadLine();
                    _number = StringExtends.ParseAndValidateInt(str, _min, _max);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public override string GetCode()
        {
            return "/getnumber";
        }

        public override string GetInfo()
        {
            return "Получение номера.";
        }
    }
}
