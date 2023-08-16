using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc.ViewModel
{
    class Calculator : BaseCalculator
    {
        public static Calculator operator + (Calculator calc, double value)
        {
            Calculator result = new Calculator();
            result.Add(value, 0);
            return result;
        }

        public static Calculator operator - (Calculator calc, double value)
        {
            Calculator result = new Calculator();
            result.Minus(value, 0);
            return result;
        }

        /**
        * @brief 연산 기호 별로 연산을 진행해주는 함수
        * @return (double) 연산 결과 값을 반환해줌
        * @note Patch-notes
        * 2023-08-09|이현호
        */

        public static double Calculate(string inputOperator, double inputNumber1, double inputNumber2)
        {

            switch (inputOperator)
            {
                case "+": return inputNumber1 + inputNumber2;
                case "-": return inputNumber2 - inputNumber1;
                case "*": return inputNumber1 * inputNumber2;
                case "/": return inputNumber2 / inputNumber1;
            }

            return 0;
        }
    }
}
