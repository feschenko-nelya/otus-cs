using System;

namespace HW4
{
	static public class StringExtends
	{
		static bool ValidateString(string? str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return false;
			}

            return !string.IsNullOrEmpty(str.Trim());
        }

        public static int ParseAndValidateInt(string? str, int min, int max)
        {
            if (!ValidateString(str))
            {
                return -1;
            }

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

    }
}
