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
        
        public string inputString;

        public string mathematicalExpression;

        public int frontOperator, rearOperator;

        public int frontNumber, rearNumber;

        public int frontZero;

        public double[] inputNumber;

        public string[] inputOperator;

        int count;
        #endregion

        #region [속성]
        public ICommand AddCommand { get; }
        public ICommand OperationCommand { get; }
        public ICommand CalcCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand SinCommand { get; }
        public ICommand CosCommand { get; }
        public ICommand TanCommand { get; }
        public ICommand RootCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region [생성자]
        public CalcVM()
        {
            inputString = string.Empty;
            mathematicalExpression = "";
            frontOperator = rearOperator = 0;
            frontNumber = rearNumber = 0;
            count = 0;
            inputNumber = new double[100];
            inputOperator = new string[10];
            AddCommand = new RelayCommand(UpdateDisplayText);
            OperationCommand = new RelayCommand(GetOperator);
            CalcCommand = new RelayCommand(StartCalculate);
            ClearCommand = new RelayCommand(StartClear);
            SinCommand = new RelayCommand(CalcSin);
            CosCommand = new RelayCommand(CalcCos);
            TanCommand = new RelayCommand(CalcTan);
            RootCommand = new RelayCommand(CalcRoot);
        }

        #endregion

        #region [public Method]

        /**
        * @brief DisplayText가 변할 때 마다 TextBlock의 Text를 업데이트 해주는 함수
        * @return (string)mathematicalExpression : TextBlock에 출력할 문자열을 저장하는 변수
        * @note Patch-notes
        * 2023-08-09|이현호
        */

        public string DisplayText
        {
            get { return mathematicalExpression; }

            set
            {
                if (mathematicalExpression != value)
                {
                    mathematicalExpression = value;
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
        * 2023-08-09|이현호
        */

        private void UpdateDisplayText(object parameter)
        {
            DisplayText += parameter;
            inputString += (string)parameter;

        }

        /**
        * @brief 연산자를 받았을 때 이전에 입력된 숫자를 inputNumber1 변수에 저장하고 연산자를 inputOperator 변수에 저장해주는 함수
        * @param+,-,*,/ 연산자를 받아옴
        * @note Patch-notes
        * 2023-08-09|이현호     
        */
        
        private void GetOperator(object parameter)
        {
            if (inputString.Length > 0)
            {
                if ((string)parameter == "*" || (string)parameter == "/")
                {
                    count++;
                }
                DisplayText += parameter;
                EnqueueNumber(double.Parse(inputString));
                inputString = "";
                EnqueueOperator(parameter.ToString());
            }
        }

        /**
        * @brief Calculate 함수를 이용해 연산을 진행해주는 함수
        * @param = 등호 기호가 클릭되었을 때만 실행
        * @note Patch-notes
        * 2023-08-09|이현호
        * 2023-08-10|이현호|+,-와 *,/ 할 때 우선 순위를 지켜서 연산을 하게 만들었다.
        * @warning * 와 / 가 2개 이상 나올 경우 정확한 연산이 되지 않는다.
        */

        private void StartCalculate(object parameter)
        {
            DisplayText += parameter;

            while (frontNumber != rearNumber && frontOperator != rearOperator)
            {

                EnqueueNumber(double.Parse(inputString));
                Swap(inputNumber, frontNumber - 1, frontNumber - 2);

                while (inputOperator[rearOperator] == "+" || inputOperator[rearOperator] == "-")
                {
                    if (count <= 0)
                    {
                        break;
                    }

                    double goBack = 0;
                    string goBack2 = "";
                    goBack = DequeueNumber();
                    EnqueueNumber(goBack);
                    goBack2 = DequeueOperator();
                    EnqueueOperator(goBack2);

                }

                //for (int i = rearOperator; i < inputOperator.Length; i++)
                //{
                //    if (inputOperator[i] == "*")
                //    {
                //        Swap(inputOperator, i, rearOperator);
                //        Swap(inputNumber, i, rearNumber);
                //        Swap(inputNumber, i + 1, rearNumber + 1);
                //        break;
                //    }

                //    else if (inputOperator[i] == "/")
                //    {
                //        Swap(inputOperator, i, rearOperator);
                //        Swap(inputNumber, i, rearNumber);
                //        Swap(inputNumber, i + 1, rearNumber + 1);
                //        break;
                //    }

                //    else
                //    {
                //        continue;
                //    }
                // }
                
                inputString = Calculate(DequeueOperator(), DequeueNumber(), DequeueNumber()).ToString();
                count--;

            }
            DisplayText = "";
            DisplayText += inputString;
        }
        /**
        * @brief 이전 연산 기록을 모두 지우는 함수
        * @param AC 기호가 클릭 되었을 때만 실행
        * @note Patch-notes
        * 2023-08-09|이현호
        */

        private void StartClear(object parameter)
        {
            inputString = "";
            Array.Clear(inputNumber, 0, inputNumber.Length);
            Array.Clear(inputOperator, 0, inputOperator.Length);
            DisplayText = "";
        }

        /**
        * @brief 연산 기호 별로 연산을 진행해주는 함수
        * @return (double) 연산 결과 값을 반환해줌
        * @note Patch-notes
        * 2023-08-09|이현호
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

        /**
        * @brief 연산자를 저장하는 배열에 연산자를 추가하는 함수  
        * @param 연산자가 클릭 되었을 때의 연산자 (+, -, /, *)
        * @note Patch-notes
        * 2023-08-10|이현호
        */

        private void EnqueueOperator (string operate)
        {
            inputOperator[frontOperator] = operate;
            frontOperator = (frontOperator + 1) % inputOperator.Length;
            return;
        }

        /**
        * @brief 연산자를 저장하는 배열에서 연산자를 꺼내오하는 함수  
        * @return (string)배열 제일 앞에 있는 연산자
        * @note Patch-notes
        * 2023-08-10|이현호
        */

        private string DequeueOperator ()
        {
            string operate = inputOperator[rearOperator];
            rearOperator = (rearOperator + 1) % inputOperator.Length;
            return operate;
        }

        /**
        * @brief 숫자를 저장하는 배열에서 숫자를 추가하는 함수  
        * @param 숫자가 클릭 되었을 때의 숫자
        * @note Patch-notes
        * 2023-08-10|이현호
        */

        private void EnqueueNumber(double num)
        {
            inputNumber[frontNumber] = num;
            frontNumber = (frontNumber + 1) % inputNumber.Length;
            return;
        }

        /**
        * @brief 숫자를 저장하는 배열에서 숫자를 꺼내오하는 함수  
        * @return (string)배열 제일 앞에 있는 숫자
        * @note Patch-notes
        * 2023-08-10|이현호
        */

        private double DequeueNumber()
        {
            double num = inputNumber[rearNumber];
            rearNumber = (rearNumber + 1) % inputNumber.Length;
            return num;
        }

        /**
        * @brief 배열에서 원하는 위치에 있는 값들의 자리를 바꾸는 함수  
        * @param (순서를 바꿀 배열, 인덱스 번호, 인덱스 번호)
        * @note Patch-notes
        * 2023-08-10|이현호
        */

        private void Swap<T> (T[] array, int index1, int index2)
        {
            T temp = array[index1];
            array[index1] = array[index2];
            array[index2] = temp;
        }

        /**
        * @brief sin버튼을 눌렀을 때 입력했던 숫자를 degree로 받아서 sin값으로연산해주는 함수  
        * @param sin 버튼
        * @note Patch-notes
        * 2023-08-10|이현호
        */

        private void CalcSin (object parameter)
        {
            try
            {
                double sinValue = Math.Sin(double.Parse(inputString) * Math.PI / 180);
                inputString = sinValue.ToString();
                DisplayText = sinValue.ToString();
            }

            catch (FormatException)
            {
                Console.WriteLine("ERROR");
            }
        }

        /**
        * @brief cos버튼을 눌렀을 때 입력했던 숫자를 degree로 받아서 cos값으로 연산해주는 함수  
        * @param cos 버튼
        * @note Patch-notes
        * 2023-08-10|이현호
        */

        private void CalcCos(object parameter)
        {
            double cosValue = Math.Cos(double.Parse(inputString) * Math.PI / 180);
            inputString = cosValue.ToString();
            DisplayText = cosValue.ToString();
        }

        /**
        * @brief tan버튼을 눌렀을 때 입력했던 숫자를 degree로 받아서 tan값으로 연산해주는 함수  
        * @param tan 버튼
        * @note Patch-notes
        * 2023-08-10|이현호
        */

        private void CalcTan(object parameter)
        {
            double tanValue = Math.Tan(double.Parse(inputString) * Math.PI / 180);
            inputString = tanValue.ToString();
            DisplayText = tanValue.ToString();
        }

        /**
        * @brief root버튼을 눌렀을 때 입력했던 숫자를 root값으로 연산해주는 함수  
        * @param root 버튼
        * @note Patch-notes
        * 2023-08-10|이현호
        */

        private void CalcRoot(object parameter)
        {
            double rootValue = Math.Sqrt(double.Parse(inputString));
            inputString = rootValue.ToString();
            DisplayText = rootValue.ToString();
        }


        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}

