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

                _number = -1;
                if (!int.TryParse(snumber, out _number))
                {
                    _number = -1;
                    throw new ArgumentException($"Не удалось получить число из \"{snumber}\".");
                }
                else if (_number == 0)
                {
                    break;
                }
                else if ((_number < _min) || (_number > _max))
                {
                    _number = -1;
                    throw new ArgumentException($"Число {_number} выходит за границы допустимого: [1; {_max}].");
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
