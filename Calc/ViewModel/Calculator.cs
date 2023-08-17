using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc.ViewModel
{
    class Calculator : BaseCalculator
    {
        public double Value { get; private set; }

        public Calculator(double numValue)
        {
            Value = numValue;
        }

        public override double Add(double number1, double number2)
        {
            return number1 + number2;
        }

        public override double Minus(double number1, double number2)
        {
            return number1 - number2;
        }

        public override double Multiply(double number1, double number2)
        {
            return number1 * number2;
        }

        public override double Divide(double number1, double number2)
        {
            return number1 / number2;
        }

        public static Calculator operator + (Calculator num1, Calculator num2)
        {
            return new Calculator(num1.Value + num2.Value);
        }

        public static Calculator operator -(Calculator num1, Calculator num2)
        {
            double newText = num1.Value - num2.Value;
            return new Calculator(newText);
        }


        public static double Calculate(string inputOperator, double inputNumber1, double inputNumber2)
        {

            Calculator number1 = new Calculator(inputNumber1);
            Calculator number2 = new Calculator(inputNumber2);
            double result = 0;

            switch (inputOperator)
            {
                case "+": 
                    result = (number1 + number2).Value;
                    break;
                case "-":
                    result = (number1 - number2).Value;
                    break;
                case "*": return number1.Value * number2.Value;
                case "/": return number2.Value / number1.Value;
            }

            return result;
        }
    }
}
