using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Calc.ViewModel
{
    public partial class CalcVM : INotifyPropertyChanged
    {
        #region [상수]
        
            // 문자열을 저장하는 변수 (마지막 등호 연산할 때 씀)
        public string inputString;
        
            // TextBlock에 출력할 문자열을 저장할 변수
        public string displayText;
        
            // 첫번째로 들어올 숫자
        public double inputNumber1;
            
            // 연산 기호를 누른 후 들어올 숫자
        public double inputNumber2;
        
            // 연산 기호를 저장
        public string inputOperator;

        #endregion

        #region [속성]
        public ICommand AddCommand { get; }
        public ICommand OperationCommand { get; }
        public ICommand CalcCommand { get; }
        public ICommand ClearCommand { get; }
        
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region [생성자]
        public CalcVM()
        {
            inputString = string.Empty;
            displayText = "";
            inputNumber1 = 0;
            inputNumber2 = 0;
            inputOperator = "";

            AddCommand = new RelayCommand(UpdateDisplayText);
            OperationCommand = new RelayCommand(GetOperator);
            CalcCommand = new RelayCommand(StartCalculate);
            ClearCommand = new RelayCommand(StartClear);
        }

        #endregion

        #region [public Method]
        
        /**
            * @brief DisplayText가 변할 때 마다 TextBlock의 Text를 업데이트 해주는 함수
            * @return (string)displayText : TextBlock에 출력할 문자열을 저장하는 변수
        * @note Patch-notes
            * 날짜|작성자|설명
            * 2023-08-09|이현호|TextBlock에 Text를 업데이트 해주는 함수
        */
        
        public string DisplayText
        {
            get { return displayText; }

            set
            {
                if (displayText != value)
                {
                    displayText = value;
                    OnPropertyChanged("DisplayText");
                }
            }
        }

        #endregion

        #region [private Method] 
        
        /**
            * @brief 숫자 버튼을 클릭했을 때 숫자를 DisplayText와 InputString에 저장해주는 함수
            * @param 0부터 9까지의 숫자를 받아옴
        * @note Patch-notes
            * 날짜|작성자|설명
            * 2023-08-09|이현호|숫자 parameter를 DisplayText와 InputString에 저장해주는 함수
        */
        
        private void UpdateDisplayText(object parameter)
        {
            if (inputString.Length > 0)
            {
                inputString = "";
            }
            DisplayText += (string)parameter;
            inputString += (string)parameter;
        }

        /**
             * @brief 연산자를 받았을 때 이전에 입력된 숫자를 inputNumber1 변수에 저장하고 연산자를 inputOperator 변수에 저장해주는 함수
            * @param+,-,*,/ 연산자를 받아옴
        * @note Patch-notes
            * 날짜|작성자|설명
            * 2023-08-09|이현호|inputString의 길이가 0 초과인 즉, 숫자가 먼저 입력되었을 때만 실행된다, 연산자를 연속으로 입력하였을 때는 나중에 입력한 연산자가 적용된다.         
        */
        
        private void GetOperator(object parameter)
        {
            if (inputString.Length > 0)
            {
                if (double.TryParse(inputString, out inputNumber1))
                {
                    inputOperator = parameter.ToString();
                    DisplayText = "";
                }
            }
        }

        /**
            * @brief Calculate 함수를 이용해 연산을 진행해주는 함수
            * @param = 등호 기호가 클릭되었을 때만 실행
        * @note Patch-notes
            * 날짜|작성자|설명
            * 2023-08-09|이현호|연산 결과를 다시 inputNumber1에 저장해줌으로써 지속적으로 연산이 가능하다
            * @warning 등호를 누르지 않고 연산자를 추가하는 경우 맨 처음 입력한 숫자는 사라진다.
            * 입력하는 숫자와 연산자를 배열을 사용하여 저장하는 방법 생각해보기  ->  큐 이용하기!
        */
        
        private void StartCalculate(object parameter)
        {
            inputNumber2 = double.Parse(inputString);
            DisplayText = Calculate(inputOperator, inputNumber1, inputNumber2).ToString();
            inputString = Calculate(inputOperator, inputNumber1, inputNumber2).ToString();
            inputNumber1 = Calculate(inputOperator, inputNumber1, inputNumber2);
            inputOperator = "";
            inputNumber2 = 0;

        }

        /**
            * @brief 이전 연산 기록을 모두 지우는 함수
            * @param AC 기호가 클릭 되었을 때만 실행
        * @note Patch-notes
            * 날짜|작성자|설명
            * 2023-08-09|이현호|연산 기록을 모두 지운다
        */
        
        private void StartClear(object parameter)
        {
            inputString = "";
            inputNumber1 = 0;
            inputNumber2 = 0;
            inputOperator = "";
            DisplayText = "";
        }

        /**
            * @brief 연산 기호 별로 연산을 진행해주는 함수
            * @return (double) 연산 결과 값을 반환해줌
        * @note Patch-notes
            * 날짜|작성자|설명
            * 2023-08-09|이현호|연산 결과를 반환해주는 함수
        */
        
        private static double Calculate(string inputOperator, double inputNumber1, double inputNumber2)
        {
            switch(inputOperator)
            {
                case "+": return inputNumber1 + inputNumber2;
                case "-": return inputNumber1 - inputNumber2;
                case "*": return inputNumber1 * inputNumber2;
                case "/": 
                    if (inputNumber2 == 0)
                    {
                        return 99999999999;
                    }
                    
                    return inputNumber1 / inputNumber2;
            }

            return 0;
        }


        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}

