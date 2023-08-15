using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc.ViewModel
{
    class BaseCalculator
    {
        public double Add (double number1, double number2)
        {
            return (number1 + number2);
        }

        public double Minus (double number1, double number2)
        {
            return (number1 - number2);
        }

        public double Multiply (double number1, double number2)
        {
            return (number1 * number2);
        }

        public double Divide (double number1, double number2)
        {
            return (number1 / number2);
        }
    }
}
