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
        public event EventHandler<WindowEventArgs> DialogFinished;
        public Settings(ref int rndVocabSize, ref bool soundState, ref int time)
        {
            InitializeComponent();
            VocabularySlider.Value  = rndVocabSize;
            //Dopisać aktualny maks dla slidera
            TimeSlider.Value = time;
            tglSoundState.IsChecked = soundState;
        }

        public void OnDialogFinished()
        {
            if (DialogFinished != null)
                DialogFinished(this, new WindowEventArgs(time: (int)TimeSlider.Value, rndVocabularySize: (int)VocabularySlider.Value, soundState: (bool)tglSoundState.IsChecked));
        }

    }
}
