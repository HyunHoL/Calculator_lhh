using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc.ViewModel
{
    class BaseCalculator
    {
        public virtual double Add (double number1, double number2)
        {
            return (number1 + number2);
        }

        public virtual double Minus (double number1, double number2)
        {
            return (number1 - number2);
        }

        public virtual double Multiply (double number1, double number2)
        {
            return (number1 * number2);
        }

        public virtual double Divide (double number1, double number2)
        {
            return (number1 / number2);
        }
    }
}
