using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Calc.Model;
using System.Collections.ObjectModel;

namespace Calc.ViewModel
{

    public partial class CalcVM : INotifyPropertyChanged
    {

        #region [상수]

        public string inputString;

        public string mathematicalExpression;

        public string history;

        public string[] inputValue;

        public string[] stack;

        public int top2;

        public int top;

        public int front;

        public int rear;

        public string[] historySave;

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
        public ICommand HistoryCommand { get; }
        public string HistorySaveElement { get; }
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region [생성자]
        public CalcVM()
        {

            inputString = string.Empty;
            history = mathematicalExpression = "";            
            inputValue = new string[1000];
            stack = new string[1000];
            top = top2 = -1;
            historySave = new string[10];
            front = rear = 0;

            AddCommand = new RelayCommand(UpdateDisplayText);
            OperationCommand = new RelayCommand(GetOperator2);
            CalcCommand = new RelayCommand(CalcPostfix);
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



        public string DisplayHis1
        {
            get { return historySave[0]; }

            set
            {
                if (historySave[0] != value) 
                {
                    historySave[0] = value;
                    OnPropertyChanged("DisplayHis1");
                }
            }
        }
        #endregion

        #region [private Method] 


        private void AllClear ()
        {
            inputString = "";
            Array.Clear(inputValue, 0, inputValue.Length);
            Array.Clear(stack, 0, stack.Length);
            top = -1;
            top2 = -1;
        }

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

        private void Push (string infixOperator)
        {
            if (top == inputValue.Length - 1)
            {
                return;
            }

            top++;
            inputValue[top] = infixOperator;
        }

        private void Push2 (string infixOperator)
        {
            if (top2 == stack.Length - 1)
            {
                return;
            }

            top2++;
            stack[top2] = infixOperator;
        }


        private string Pop ()
        {
            if (top == -1)
            {
                return "ERROR";
            }

            string result = inputValue[top];
            top--;
            return result;
        }

        private string Pop2()
        {
            if (top2 == -1)
            {
                return "ERROR";
            }

            string result = stack[top2];
            top2--;

            return result;
        }

        private void Enqueue (string inputNum)
        {
            historySave[front] = inputNum;
            front = (front + 1) % historySave.Length;
            return;
        }

        private string Dequeue ()
        {
            if (front == rear)
            {
                return "ERROR";
            }

            string result = historySave[rear];
            rear = (rear + 1) % historySave.Length;

            return result;
        }

        private void GetOperator2 (object parameter)
        {
            if ((string)parameter == "-")
            {
                Push((string)inputString);
                DisplayText += parameter;
                inputString = "";
                Push("+");
                inputString += "-";
                return;
            }
            Push((string)inputString);            
            DisplayText += parameter;
            inputString = "";
            Push((string)parameter);
        }
        
        private int Priority (string infixOperator)
        {
            if (inputValue[top] == "(")
            {
                return 1;
            }

            else if (infixOperator == "*" || infixOperator == "/")
            {
                if (stack[top2] == "+" || stack[top2] == "-")
                {
                    return 1;
                }
            }


            return 0;
        }

        private void Postfix(string[] postfix)
        {
            if (inputString == "" && DisplayText != "")
            {
                for (int i = 0; i < DisplayText.Length; i++)
                {
                    inputString = DisplayText[i].ToString();
                    Push(inputString);
                }
            }

            else
            {
                Push(inputString);
            }
            int topPostfix = -1;
            int idx = 0;

            while (idx != top + 1)
            {
                string infixOperator = inputValue[idx];
                
                // 1. 처음이 숫자일 때                
                if (double.TryParse(infixOperator, out double parsenumber))
                {
                    topPostfix++;
                    postfix[topPostfix] = infixOperator;
                }

                // 연산자, (, ) 일 때
                else
                {
                    if (infixOperator == "(")
                    {
                        Push2(infixOperator);
                    }

                    else if (infixOperator == ")")
                    {
                        while (true)
                        {
                            string postfixOperator = Pop();

                            if (postfixOperator == "(")
                            {
                                break;
                            }
                            postfix[topPostfix] = postfixOperator;
                            topPostfix++;
                        }
                    }

                    else
                    {
                        if (top2 == -1)
                        {
                            Push2(infixOperator);
                        }

                        else
                        {
                            while (true)
                            {
                                if (Priority(infixOperator) < Priority(stack[top2]))
                                {
                                    topPostfix++;
                                    postfix[topPostfix] = Pop2();
                                }

                                else if ((infixOperator == "*" || infixOperator == "/") && (stack[top2] == "*" || stack[top2] == "/"))
                                {
                                    topPostfix++;
                                    postfix[topPostfix] = Pop2();
                                }

                                else if ((infixOperator == "+" || infixOperator == "-") && (stack[top2] == "*" || stack[top2] == "/"))
                                {
                                    topPostfix++;
                                    postfix[topPostfix] = Pop2();
                                }

                                else
                                {
                                    break;
                                }
                            }
                            Push2(infixOperator);
                        }
                    }
                }
                idx++;
            }
            while (top2 != -1)
            {
                topPostfix++;
                postfix[topPostfix] = Pop2();
            }
        }

        private void CalcPostfix (object parameter)
        {
            
            string[] lastPostfix = new string[1000];
            Postfix(lastPostfix);


            if (lastPostfix.Length < 3)
            {
                DisplayText = "ERROR";
                AllClear();
                return;
            }

            if (!double.TryParse(lastPostfix[0], out double letter))
            {
                DisplayText = "ERROR";
                AllClear();
                return;
            }

            int idx = 0;

            while (lastPostfix[idx] != null)
            {
                string postfixOperator = lastPostfix[idx];

                if (double.TryParse(postfixOperator, out double parsenumber))
                {
                    Push2(postfixOperator);
                }

                else
                {
                    double number1 = double.Parse(Pop2());
                    double number2 = double.Parse(Pop2());

                    if (postfixOperator == "/" && number1 == 0)
                    {
                        DisplayText = "ERROR";
                        AllClear();
                        return;
                    }

                    double result = Calculate(postfixOperator, number1, number2);
                    Push(result.ToString());
                    Push2(result.ToString());
                }
                idx++;
            }
            string displayed = DisplayText;
            DisplayText = Pop();
            Enqueue(displayed + " = " + DisplayText);
            DisplayHis1 += historySave[0];
            AllClear();
            inputString = DisplayText;
        }

        /**
        * @brief 이전 연산 기록을 모두 지우는 함수
        * @param AC 기호가 클릭 되었을 때만 실행
        * @note Patch-notes
        * 2023-08-09|이현호
        */

        private void StartClear(object parameter)
        {
            AllClear();
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
                     
            switch (inputOperator)
            {
                case "+": return inputNumber1 + inputNumber2;
                case "-": return inputNumber2 - inputNumber1;
                case "*": return inputNumber1 * inputNumber2;
                case "/": return inputNumber2 / inputNumber1;
            }

            return 0;
        }

        /**
        * @brief sin버튼을 눌렀을 때 입력했던 숫자를 degree로 받아서 sin값으로연산해주는 함수  
        * @param sin 버튼
        * @note Patch-notes
        * 2023-08-10|이현호
        */

        private void CalcSin(object parameter)
        {
            if (!double.TryParse(inputString, out double letter))
            {
                DisplayText = "ERROR";
                AllClear();
                return;
            }

            double sinValue = Math.Sin(double.Parse(inputString) * Math.PI / 180);
            inputString = sinValue.ToString();
            DisplayText = sinValue.ToString();
        }

        /**
        * @brief cos버튼을 눌렀을 때 입력했던 숫자를 degree로 받아서 cos값으로 연산해주는 함수  
        * @param cos 버튼
        * @note Patch-notes
        * 2023-08-10|이현호
        */

        private void CalcCos(object parameter)
        {
            if (!double.TryParse(inputString, out double letter))
            {
                DisplayText = "ERROR";
                AllClear();
                return;
            }
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
            if (!double.TryParse(inputString, out double letter))
            {
                DisplayText = "ERROR";
                AllClear();
                return;
            }

            double tanValue = Math.Tan(double.Parse(inputString) * Math.PI / 180);
            
            if (double.Parse(inputString) % 90 == 0)
            {
                DisplayText = "ERROR";
                AllClear();
                return;
            }
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
            if (!double.TryParse(inputString, out double letter))
            {
                DisplayText = "ERROR";
                AllClear();
                return;
            }

            if (double.Parse(inputString) < 0)
            {
                DisplayText = "ERROR";
                AllClear();
                return;
            }
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

