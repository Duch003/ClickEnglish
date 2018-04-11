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
        //private byte TEMP_rndVocabSize;
        //private bool TEMP_soundState;
        //private uint TEMP_time;
        private DatabaseManager _manager;
        public Settings(DatabaseManager manager)
        {
            //TODO Dopisać ograniczniki sliderów
            InitializeComponent();
            _manager = manager;
            VocabularySlider.Value  = GlobalSettings.RandomVocabulaySize;
            TimeSlider.Value = GlobalSettings.Time;
            tglSoundState.IsChecked = GlobalSettings.SoundState;
        }

        private void Settings_Apply(object sender, RoutedEventArgs e)
        {
            if(_manager.IsConnected())
            {
                GlobalSettings.RandomVocabulaySize = (int)VocabularySlider.Value;
                GlobalSettings.Time = (int)TimeSlider.Value;
                GlobalSettings.SoundState = (bool)tglSoundState.IsChecked;
                _manager.SaveSettings();

                this.Close();
            }
            else
                MessageBox.Show("Database is disconnected.", "Cannot save settings.", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Settings_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
