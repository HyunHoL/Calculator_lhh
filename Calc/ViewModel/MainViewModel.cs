using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Calc.ViewModel
{
    public class MainViewModel
    {

        private string displayText = "";

        public string DisplayText
        {
            get { return displayText; }

            set
            {
                if (displayText != value)
                {
                    displayText = value;
                }
            }
        }

        public void UpdateDisplayText (string parameter)
        {
            displayText += parameter;
        }

        //private ICommand _clickCommand;

        //// TextBlock에 출력할 문자열을 저장할 변수

        //public ICommand ClickCommand
        //{
        //    get
        //    {
        //        if (_clickCommand == null)
        //        {
        //            _clickCommand = new RelayCommand(Click);
        //        }
        //        return _clickCommand;
        //    }
        //}

        //// 문자열을 저장하는 변수 (마지막 등호 연산할 때 씀)
        //public string inputString = string.Empty;

        //// 첫번째로 들어올 숫자
        //public double inputNumber1 = 0;

        //// 연산 기호를 누른 후 들어올 숫자
        //public double inputNumber2 = 0;


        //private void Click(object parameter)
        //{
        //    displayText += parameter;
        //}
    }











    //MainViewModel()
    //{
    //    AddCommand = new RelayCommand(Click);
    //}




    /**
    * @brief 숫자 버튼을 클릭하였을 때 TextBlock에 숫자가 출력되게 만드는 함수
    * @param 숫자 버튼을 눌렀을 때 숫자
    * @return displayText에 누른 숫자를 반환
    * 2023-08-09|이현호|설명
    */
    //public class AddCommand
    //{


    //}
}


