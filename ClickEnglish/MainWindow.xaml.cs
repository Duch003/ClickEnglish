using System.Collections.Generic;
using System.Data;
using System.Windows;

namespace ClickEnglish
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DatabaseManager _manager;

        public MainWindow()
        {
            InitializeComponent();
            _manager = new DatabaseManager("localhost", "Duch003", "Killer003", "5432", "MyDictionaryApp_IntegrationTests");
            do {
                OpenLogInScreen();
            } while(GlobalSettings.ID == 0);
        }

        #region Methods
        private void OpenLogInScreen()
        {
            this.IsEnabled = false;
            var temp = new LogInWindow();
            temp.ShowDialog();
            this.IsEnabled = true;
        }

        private void OpenSettings()
        {
            this.IsEnabled = false;
            var temp = new Settings(_manager);
            temp.ShowDialog();
            this.IsEnabled = true;
        }

        private void OpenAbout()
        {
            this.IsEnabled = false;
            var temp = new About();
            temp.ShowDialog();
            this.IsEnabled = true;
        }

        private void OpenDictionaryManager()
        {
            this.IsEnabled = false;
            var temp = new DictionaryManager(_manager);
            temp.ShowDialog();
            this.IsEnabled = true;
        }

        private void OpenCategoryManager()
        {
            this.IsEnabled = false;
            var temp = new CategoryManager(_manager);
            temp.ShowDialog();
            this.IsEnabled = true;
        }

        private void ParticularCategory()
        {
            this.IsEnabled = false;

        }
        #endregion

        private bool ConvertToList(DataSet raw, out List<Question> processed)
        {
            if(raw is null) {
                processed = null;
                return false;
            }

            if(raw.Tables[0].Rows.Count == 0) {
                processed = null;
                return false;
            }

            processed = new List<Question>();
            for(int i = 0; i < raw.Tables[0].Rows.Count; i++) {
                var eng = raw.Tables[0].Rows[i][0].ToString();
                var pl = raw.Tables[0].Rows[i][1].ToString();
                var percentage = raw.Tables[0].Rows[i][2].ToString();
                var img = raw.Tables[0].Rows[i][3].ToString();

                processed.Add(new Question)
            }
                
        }

        #region Events
        private void Settings_Click(object sender, RoutedEventArgs e) => OpenSettings();

        private void RandomVocabSet_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;

            var gameWindow = new MainGameBoard();
            gameWindow.Show();
            this.IsEnabled = true;
        }

        private void ParticularCategory_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            var gameWindow = new MainGameBoard();
            gameWindow.Show();
            this.IsEnabled = true;
        }

        private void TimeChallange_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            var gameWindow = new MainGameBoard();
            gameWindow.Show();
            this.IsEnabled = true;
        }

        private void WholeDictionary_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            var gameWindow = new MainGameBoard();
            gameWindow.Show();
            this.IsEnabled = true;
        }

        private void EditDictionary_Click(object sender, RoutedEventArgs e) => OpenDictionaryManager();

        private void EditCategories_Click(object sender, RoutedEventArgs e) => OpenCategoryManager();

        private void About_Click(object sender, RoutedEventArgs e) => OpenAbout();
        #endregion
    }
}

