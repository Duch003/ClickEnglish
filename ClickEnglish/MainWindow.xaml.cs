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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClickEnglish
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //TODO Wszystkie ustawienia przypisane do użytkownika
        //TODO Dodać hasło dla użytkowników
        //Current user settings
        private bool _soundState;                        //State of sounds
        private int _time;                 //Time for time challange [ms]
        private int _rndVocabularySize;                        //Number of words in challange
        private int _myId;                                 //Id of logged user
        private DatabaseManager _manager;

        public MainWindow()
        {
            InitializeComponent();
            _manager = new DatabaseManager("localhost", "Duch003", "Killer003", "5432", "MyDictionaryApp_IntegrationTests");
            OpenSettings();
            MessageBox.Show($"{_soundState}, {_time}, {_rndVocabularySize}, {_myId}");
        }

        private void OpenSettings()
        {
            this.IsEnabled = false;
            var temp = new Settings();
            temp.DialogFinished += new EventHandler<WindowEventArgs>(Settings_DialogFinished);
            temp.ShowDialog();
            this.IsEnabled = true;
        }

        private void Settings_DialogFinished(object sender, WindowEventArgs e)
        {
            _soundState = e.SoundState;
            _time = e.Time;
            _rndVocabularySize = e.RndVocabularySize;
        }

        public void SaveSettings()
        {

        }

        #region Events

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            OpenSettings();
        }

        private void RandomVocabSet_Click(object sender, RoutedEventArgs e)
        {
            var gameWindow = new MainGameBoard();
            gameWindow.Show();
        }

        private void ParticularCategory_Click(object sender, RoutedEventArgs e)
        {
            var gameWindow = new MainGameBoard();
            gameWindow.Show();
        }

        private void TimeChallange_Click(object sender, RoutedEventArgs e)
        {
            var gameWindow = new MainGameBoard();
            gameWindow.Show();
        }

        private void WholeDictionary_Click(object sender, RoutedEventArgs e)
        {
            var gameWindow = new MainGameBoard();
            gameWindow.Show();
        }

        private void EditDictionary_Click(object sender, RoutedEventArgs e)
        {
            var dictionaryManager = new DictionaryManager();
            dictionaryManager.Show();
        }

        private void EditCategories_Click(object sender, RoutedEventArgs e)
        {
            var categoryManager = new CategoryManager();
            categoryManager.Show();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new About();
            aboutWindow.Show();
        }

        #endregion
    }
}

