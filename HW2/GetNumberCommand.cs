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
                string? snumber = Console.ReadLine();

                try
                {
                    _number = ParseAndValidateInt(snumber, _min, _max);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private int ParseAndValidateInt(string? str, int min, int max)
        {
            int number = 0;
            if (!int.TryParse(str, out number))
            {
                throw new ArgumentException($"Не удалось получить число из \"{str}\".");
            }
            else if (number == 0)
            {
                return 0;
            }
            else if ((number < min) || (number > max))
            {
                throw new ArgumentException($"Число {number} выходит за границы допустимого: [{min}; {max}].");
            }

            return number;
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
