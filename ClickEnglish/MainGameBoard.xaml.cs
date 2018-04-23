using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClickEnglish
{
    /// <summary>
    /// Interaction logic for MainGameBoard.xaml
    /// </summary>
    public partial class MainGameBoard : Window
    {
        List<Question> Questions;
        List<Question> ToRepeat;
        TimeSpan Timer;

        public MainGameBoard(List<Question> question, bool time)
        {
            InitializeComponent();
            Questions = question;
            if(time)
                Timer = new TimeSpan(0, GlobalSettings.Time, 0);
            else
                Timer = new TimeSpan(0, 0, 0);
           
            if(time) {
                
            } else {

            }
        }

        //Losowanie z dostarczonej puli za każdym razem z możliwością powtórki
        private void Ask()
        {

        }

        private int Draw()
        {
            return new Random().Next(1, Questions.Count);
        }

        private void Check_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
