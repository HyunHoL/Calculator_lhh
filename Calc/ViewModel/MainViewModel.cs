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
    public class  Calculation
    {
        public string Expression { get; set; }
        public string Result { get; set; }
    }

    public partial class CalcVM : INotifyPropertyChanged
    {

        #region [상수]

        public string inputString, inputBracket;

        public string mathematicalExpression;

        public string history, history2, history3, history4, history5;

        public string result, result2, result3, result4, result5;

        public string[] inputValue;

        public string[] stack;

        public int top2;

        public int top;

        public int front;

        public int rear;

        public string[] historySave;

        public string inputNum;

        #endregion

        #region [속성]
        public ICommand AddCommand { get; }
        public ICommand AddBracket { get; }
        public ICommand OperationCommand { get; }
        public ICommand CalcCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SinCommand { get; }
        public ICommand CosCommand { get; }
        public ICommand TanCommand { get; }
        public ICommand RootCommand { get; }
        public ObservableCollection<String> HistorySave { get; } = new ObservableCollection<string>();
        public ICommand ToggleListViewCommand { get; }

        private bool _isListViewVisible = false;
        private string _selectedHistoryItem;


        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region [생성자]
        public CalcVM()
        {

            inputBracket = inputNum = inputString = string.Empty;
            history2 = history3 = history4 = history5 = history = mathematicalExpression = "";
            result = result2 = result3 = result4 = result5 = "";
            inputValue = new string[100];
            stack = new string[100];
            top = top2 = -1;
            historySave = new string[10];
            front = rear = 0;

            AddCommand = new RelayCommand(UpdateDisplayText);
            AddBracket = new RelayCommand(GetBracket);
            OperationCommand = new RelayCommand(GetOperator);
            CalcCommand = new RelayCommand(CalcPostfix);
            ClearCommand = new RelayCommand(StartClear);
            DeleteCommand = new RelayCommand(StartDelete);
            SinCommand = new RelayCommand(CalcSin);
            CosCommand = new RelayCommand(CalcCos);
            TanCommand = new RelayCommand(CalcTan);
            RootCommand = new RelayCommand(CalcRoot);
            ToggleListViewCommand = new RelayCommand(ToggleListViewVisibility);
        }

        #endregion

        #region [public Method]

        /**
        * @brief 버튼을 클릭했을 때만 ListView가 보이게 해주는 함수
        * @return (bool) false일 때, ListView 보이지 않음 , true일 때, ListView 보임
        * @note Patch-notes
        * 2023-08-17|이현호
        */

        public bool IsListViewVisible
        {
            get => _isListViewVisible;

            set
            {
                if (_isListViewVisible != value)
                {
                    _isListViewVisible = value;
                    OnPropertyChanged("IsListViewVisible");
                }
            }
        }

        /**
        * @brief 버튼을 눌렀을 때, bool 값을 바꿔주는 함수
        * @note Patch-notes
        * 2023-08-17|이현호
        */

        public void ToggleListViewVisibility()
        {
            IsListViewVisible = !IsListViewVisible;
        }

        /**
        * @brief 연산 history를 클릭했을 때, 연산 결과 값을 TextBox에 출력해주는 함수
        * @return (string) 연산 결과
        * @note Patch-notes
        * 2023-08-17|이현호
        */

        public string SelectedHistoryItem
        {
            get { return _selectedHistoryItem; }
            set
            {
                if (_selectedHistoryItem != value)
                {
                    _selectedHistoryItem = value;

                    int idx = 0;
                    string inputResult = "";

                    if (_selectedHistoryItem == null)
                    {
                        return;
                    }

                    for (int i = 0; i < _selectedHistoryItem.Length; i++)
                    {
                        if (_selectedHistoryItem[i] == '=')
                        {
                            idx = i;
                        }
                    }

                    for (int i = 0; i < _selectedHistoryItem.Length; i++)
                    {
                        if (i > idx)
                        {
                            inputResult += _selectedHistoryItem[i];
                        }
                    }

                    DisplayText = inputResult;
                    AllClear();
                    inputString = DisplayText;
                    OnPropertyChanged(nameof(SelectedHistoryItem));
                }
            }
        }

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
        * @brief 입력 받은 모든 값을 초기화 해주는 함수
        * @note Patch-notes
        * 2023-08-14|이현호
        */

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
            inputString += parameter;
        }

        /**
        * @brief 괄호 버튼을 클릭했을 때 괄호를 DisplayText에 저장하고 푸쉬해주는 함수
        * @param 괄호를 받아옴
        * @note Patch-notes
        * 2023-08-17|이현호
        */

        private void GetBracket(object parameter)
        {
            if ((string)parameter == "(")
            {
                DisplayText += parameter;
                inputBracket = (string)parameter;
                Push(inputBracket);
            }

            else
            {
                DisplayText += parameter;
                inputBracket = (string)parameter;
            }
        }

        /**
        * @brief 숫자와 연산자가 들어왔을 때 inputValue 배열에 푸쉬해주는 함수
        * @param 버튼을 눌렀을 때, parameter를 string으로 받아 온다.
        * @note Patch-notes
        * 2023-08-14|이현호
        */

        private void Push (string infixOperator)
        {
            if (top == inputValue.Length - 1)
            {
                return;
            }

            top++;
            inputValue[top] = infixOperator;
        }

        /**
        * @brief 숫자와 연산자가 들어왔을 때 stack 배열에 푸쉬해주는 함수
        * @param 버튼을 눌렀을 때, parameter를 string으로 받아 온다.
        * @note Patch-notes
        * 2023-08-14|이현호
        */

        private void Push2 (string infixOperator)
        {
            if (top2 == stack.Length - 1)
            {
                return;
            }

            top2++;
            stack[top2] = infixOperator;
        }

        /**
        * @brief inputValue 배열의 마지막에 들어온 값을 빼주는 함수
        * @return (string) 배열의 제일 마지막에 들어온 값을 반환
        * @note Patch-notes
        * 2023-08-14|이현호
        */

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

        /**
        * @brief stack 배열의 마지막에 들어온 값을 빼주는 함수
        * @return (string) 배열의 제일 마지막에 들어온 값을 반환
        * @note Patch-notes
        * 2023-08-14|이현호
        */

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

        /**
        * @brief 연산자 버튼이 클릭 되었을 때, 이전에 입력한 숫자와 연산자를 inputValue 배열에 추가해주는 함수
        * @param 연산자 (+, -, *, /)
        * @note Patch-notes
        * 2023-08-14|이현호
        */

        private void GetOperator (object parameter)
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

            else if (inputString == "")
            {
                DisplayText += parameter;
                inputString = "";
                Push((string)parameter);
                return;
            }

            else
            {
                Push(inputString);
                DisplayText += parameter;
                inputString = "";
                Push((string)parameter);
            }
        }

        /**
        * @brief 연산자의 우선순위를 정해주는 함수
        * @param 연산자 (+, -, *, /)
        * @return (int) +, - 가 들어왔을 때, 0을 return, *나 / 가 들어왔을 때, 이전에 들어온 연산자가 +나 -이면 1을 return
        * @note Patch-notes
        * 2023-08-14|이현호
        */

        private int Priority (string infixOperator)
        {
            if (inputValue[top] == "(")
            {
                return 1;
            }

            else if (infixOperator == "*" || infixOperator == "/")
            {
                return 1;
            }

            return 0;
        }

        /**
        * @brief infix 수식을 postfix 수식으로 바꿔주는 함수
        * @param infix 식을 postfix로 바꿀 배열
        * @note Patch-notes
        * 2023-08-14|이현호
        * 2023-08-17|이현호|괄호 우선 순위 추가
        */

        private void Postfix(string[] postfix)
        {

            Push(inputString);

            if (inputBracket == ")")
            {
                Push(inputBracket);
            }

            int topPostfix = -1;
            int idx = 0;

            while (idx != top + 1)
            {
                string infixOperator = inputValue[idx];
                              
                if (double.TryParse(infixOperator, out double parsenumber))
                {
                    topPostfix++;
                    postfix[topPostfix] = infixOperator;
                }

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
                            string postfixOperator = Pop2();

                            if (postfixOperator == "(")
                            {
                                break;
                            }
                            topPostfix++;
                            postfix[topPostfix] = postfixOperator;
                        }
                    }

                    else
                    {
                        if (top2 == -1)
                        {
                            Push2(infixOperator);
                        }

                        else if (stack[top2] == "(")
                        {
                            Push2(infixOperator);
                        }

                        else
                        {
                            while (true)
                            {
                                if (top2 == -1)
                                {
                                    break;
                                }

                                else if (Priority(infixOperator) < Priority(stack[top2]))
                                {
                                    topPostfix++;
                                    postfix[topPostfix] = Pop2();
                                }


                                else if ((infixOperator == "*" || infixOperator == "/") && (stack[top2] == "*" || stack[top2] == "/"))
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

        /**
        * @brief postfix 수식을 연산해주는 함수
        * @param 연산할 postfix 배열
        * @note Patch-notes
        * 2023-08-14|이현호
        */

        private void CalcPostfix (object parameter)
        {
            
            string[] lastPostfix = new string[100];
            Postfix(lastPostfix);

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

                    double result = Calculator.Calculate(postfixOperator, number1, number2);
                    Push(result.ToString());
                    Push2(result.ToString());
                }
                idx++;
            }
            string displayed = DisplayText;
            DisplayText = Pop();
            HistorySave.Add(displayed + "=" + DisplayText);

            AllClear();
            inputBracket = string.Empty;
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
            HistorySave.Clear();
        }

        /**
        * @brief 바로 직전 연산 기록을 지우는 함수
        * @param ⌫ 기호가 클릭 되었을 때만 실행
        * @note Patch-notes
        * 2023-08-17|이현호
        */

        private void StartDelete (object parameter)
        {
            int idx = 0;
            int idxNum = 0;
            string saveText = string.Empty;
            string saveNum = string.Empty;

            for (int i = 0; i < DisplayText.Length; i++)
            {
                if (i == DisplayText.Length - 1)
                {
                    idx = i;
                    break;
                }

                saveText += DisplayText[i];
            }

            if (DisplayText[idx] == '(')
            {
                Pop();
                DisplayText = saveText;
            }

            else if (DisplayText[idx] == ')')
            {
                DisplayText = saveText;
                inputBracket = string.Empty;
            }

            else if (double.TryParse(DisplayText[idx].ToString(), out double outValue))
            {
                for (int i = 0; i < inputString.Length; i++)
                {
                    if (i == inputString.Length - 1)
                    {
                        idxNum = i;
                        break;
                    }
                    saveNum += inputString[i];
                }
                inputString = saveNum;
                DisplayText = saveText;
            }

            else
            {
                Pop();
                DisplayText = saveText;
                inputString = string.Empty;
            }
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

