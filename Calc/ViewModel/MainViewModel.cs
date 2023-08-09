using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc.ViewModel
{
    public class MainViewModel
    {

        // 문자열을 저장하는 변수 (마지막 등호 연산할 때 씀)
        public string inputString = string.Empty;

        // TextBlock에 출력할 문자열을 저장할 변수
        public string displayText = "";

        // 첫번째로 들어올 숫자
        public double inputNumber1 = 0;

        // 연산 기호를 누른 후 들어올 숫자
        public double inputNumber2 = 0;



        public RelayCommand _AddCommand;

        MainViewModel()
        {
            AddCommand = new RelayCommand(Click);
        }


        public ICommand AddCommand
        {
            get
            {
                return _AddCommand ?? (_AddCommand = new RelayCommand(Click));
            }
        }

        private void Click(object parameter)
        {
            // 클릭 시 실행될 로직을 작성합니다.
            // 여기서는 간단히 메시지를 출력합니다.
            System.Windows.MessageBox.Show("Button Clicked!");
        }




        /**
        * @brief 숫자 버튼을 클릭하였을 때 TextBlock에 숫자가 출력되게 만드는 함수
        * @param 숫자 버튼을 눌렀을 때 숫자
        * @return displayText에 누른 숫자를 반환
        * 2023-08-09|이현호|설명
        */
        public class AddCommand (object parameter)
        {


        }
    }
}

