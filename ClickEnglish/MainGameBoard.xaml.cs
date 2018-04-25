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
using System.Windows.Threading;

namespace ClickEnglish
{
    /// <summary>
    /// Interaction logic for MainGameBoard.xaml
    /// </summary>
    public partial class MainGameBoard : Window
    {
        private List<Question> Questions;
        private TimeSpan Time;
        private DispatcherTimer Timer;
        private int Passed;

        public MainGameBoard(List<Question> question, bool time)
        {
            InitializeComponent();
            Questions = question;
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(1);

            if(time) {
                Time = new TimeSpan(0, GlobalSettings.Time, 0);
                Timer.Tick += Timer_TickDecrement;
            }
                
            else{  
                Time = new TimeSpan(0, 0, 0);
                Timer.Tick += Timer_TickIncrement;
            }
            Passed = Questions.Count;
            Timer.Start();
        }
        private void Timer_TickIncrement(object sender, EventArgs e) {
            Time.Add(TimeSpan.FromSeconds(1));
            tbTimer.Text = Time.Minutes + ":" + Time.Seconds;
        }

        private void Timer_TickDecrement(object sender, EventArgs e) {
            Time.Add(TimeSpan.FromSeconds(-1));
            if(Time.TotalSeconds == 0)
                Timer.Stop();
            tbTimer.Text = Time.Minutes + ":" + Time.Seconds;
            //TODO ZAMKNIECIE GRY
        }

        //Losowanie z dostarczonej puli za każdym razem z możliwością powtórki
        private void Ask()
        {
            //Draw question
            Random rnd;
            Question current;
            do {
                if(Questions.Count == 0)
                    Timer.Stop();
                rnd = new Random();
                int index = Draw();

                current = Questions[index];
                if(current.Repeats <= 0)
                    Questions.Remove(current);
                else
                    break;
            } while(true);

            //Draw which one to ask
            if(rnd.Next(0, 1) == 1) {
                tbAsk.Text = current.WordEng;
            } else {
                tbAsk.Text = current.WordPl;
            }

            //Assign hint picture
            if(current.ImgSrc != "none") {
                imgHint.Source = new BitmapImage(new Uri(current.ImgSrc));
            } else {
                imgHint.Source = new BitmapImage(new Uri("background/default_image.jpg"));
            }

            //Assign repeats
            string postfix;
            if(current.Repeats > 1)
                postfix = " times";
            else
                postfix = " time";
            tbRepeats.Text = current.Repeats + postfix;

            tbCounter.Text = Questions.Count + " questions to ask";

            tbPass.Text = (Questions.Count - Passed) + " passed";

            tbCategory.Text = current.Cat.Name;
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
