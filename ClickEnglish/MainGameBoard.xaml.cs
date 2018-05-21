using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ClickEnglish
{
    /// <summary>
    /// Interaction logic for MainGameBoard.xaml
    /// </summary>
    public partial class MainGameBoard : Window
    {
        DispatcherTimer Timer;
        DispatcherTimer Wait = new DispatcherTimer();
        TimeSpan Time;
        Dictionary<int, Question> Questions;
        int Current;
        int Passed;
        string GoodAnwser;

        public MainGameBoard(Dictionary<int, Question> pack, bool IncreaseTime)
        {
            InitializeComponent();
            if (IncreaseTime)
                PrepareDispatcherTimer_TimeCounter();
            else
                PrepareDispatcherTimer_EggTimer();
            Questions = pack;
            Passed = 0;
            Wait.Interval = TimeSpan.FromSeconds(5);
            Wait.Tick += Ask;
            Ask(null, null);
        }

        //Check and show correct anwser
        void Check()
        {
            btnCheck.Content = GoodAnwser;
            if (GoodAnwser.Equals(tbAsk.Text))
            {
                Questions[Current].DecrementRepeats();
                btnCheck.Background = Brushes.Green;
            }
            else
            {
                Questions[Current].IncrementRepeats();
                btnCheck.Background = Brushes.DarkRed;
            }
        }

        //Question randomization
        void Ask(object sender, EventArgs e)
        {
            //Restore custom properties
            btnCheck.Background = Brushes.DarkBlue;
            Wait.Stop();
            btnCheck.Content = "Check";

            Random rnd = new Random();

            //RandomizeQuestion
            do
            {
                Current = rnd.Next(0, Questions.Count);
            } while (!Questions.ContainsKey(Current));
            var temp = Questions[Current];

            //Randomize which to ask
            //If even -> english word
            if (rnd.Next(0, 1000) % 2 == 0)
            {
                tbAsk.Text = temp.English;
                GoodAnwser = temp.Polish;
            }
            //Else -> polish word
            else
            {
                tbAsk.Text = temp.Polish;
                GoodAnwser = temp.English;
            }             

            //Decode image
            if (temp.Picture == null)
                imgHint.Source = new BitmapImage(new Uri(@"pack://application:,,,/background/default_picture.jpg"));
            else
                imgHint.Source = ConvertByteArrayToBitmapImage(temp.Picture) ?? new BitmapImage(new Uri(@"pack://application:,,,/background/default_picture.jpg"));

            //Show repeats of the word
            tbRepeats.Text = "Repeats: " + temp.Repeats;

            //Show remaining words
            tbCounter.Text = "Remaining questions: " + Questions.Count;

            //Show scored questions
            tbPass.Text = "Scored questions: " + Passed;

            //Show category
            tbCategory.Text = temp.Category.Name;
        }

        //https://stackoverflow.com/questions/9564174/convert-byte-array-to-image-in-wpf
        public static BitmapImage ConvertByteArrayToBitmapImage(byte[] bytes)
        {
            try
            {
                var stream = new MemoryStream(bytes);
                stream.Seek(0, SeekOrigin.Begin);
                var image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = stream;
                image.EndInit();
                return image;
            }
            catch
            {
                return null;
            } 
        }

        #region TimeTicker Management
        void PrepareDispatcherTimer_EggTimer()
        {
            Time = new TimeSpan(0, GlobalSettings.TimeChallange, 0);
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(1);
            Timer.Tick += timer_NegativeTick;
            Timer.Start();
        }

        void PrepareDispatcherTimer_TimeCounter()
        {
            Time = new TimeSpan(0, 0, 0);
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(1);
            Timer.Tick += timer_PositiveTick;
            Timer.Start();
        }

        void timer_NegativeTick(object sender, EventArgs e)
        {
            tbTimer.Text = $"{Time.Hours} : {Time.Minutes} : {Time.Seconds}";
            if(Time.TotalSeconds == 0)
            {
                Timer.Stop();
                if (MessageBoxResult.OK == MessageBox.Show("Well done!", "Time is up!", MessageBoxButton.OK, MessageBoxImage.Information))
                    this.Close();
                else
                    this.Close();
            }
            Time.Subtract(new TimeSpan(0, 0, 1));
            
        }

        void timer_PositiveTick(object sender, EventArgs e)
        {
            tbTimer.Text = $"{Time.Hours} : {Time.Minutes} : {Time.Seconds}";
            Time.Add(new TimeSpan(0, 0, 1));
        }
        #endregion

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            Wait.Start();
            Check();
            if(Questions[Current].Repeats == 0)
            {
                Questions.Remove(Current);
                Passed++;
            }
            if(Questions.Count == 0)
            {
                this.Close();
            }
        }
    }
}
