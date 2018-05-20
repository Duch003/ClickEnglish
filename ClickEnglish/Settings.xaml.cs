using System;
using System.Collections.Generic;
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
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
            tglSoundState.IsChecked= GlobalSettings.Sound;
            if (GlobalSettings.TimeChallange <= 15)
                TimeSlider.Value = GlobalSettings.TimeChallange;
            else if (GlobalSettings.TimeChallange < 1)
                TimeSlider.Value = 1;
            else
                TimeSlider.Value = 15;
            TimeSlider.Maximum = 15;
            TimeSlider.Minimum = 1;
            VocabularySlider.Value = GlobalSettings.VocabularySize;
            VocabularySlider.Maximum = GlobalSettings.VocabularySize_UpperLimit;
            VocabularySlider.Minimum = 1;
        }

        private void Settings_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Settings_Apply(object sender, RoutedEventArgs e)
        {
            using (var ctx = new DictionaryContext())
            {
                ctx.UserSettings.First().Sound = (bool)tglSoundState.IsChecked;
                ctx.UserSettings.First().TimeChallange= (int)TimeSlider.Value;
                ctx.UserSettings.First().VocabularySize = (int)VocabularySlider.Value;
                ctx.SaveChanges();
                
            }
            this.Close();
        }
    }
}
