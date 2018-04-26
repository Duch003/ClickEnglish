using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Linq;
using Microsoft.Win32;

namespace ClickEnglish
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DatabaseManager _manager;
        private List<Category> _categories;
        public ObservableCollection<string> Categories;
        public MainWindow()
        {
            InitializeComponent();
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = "Image"; // Default file name
            dlg.DefaultExt = ".bmp"; // Default file extension
            dlg.Filter = "JPEG (.jpeg)|*.jpeg;*.jpg" +
                "|TIFF (.tiff)|*.tiff" +
                "|BMP (.bmp)|*.bmp" +
                "|GIF (.gif)|*.gif" +
                "|PNG (.png)|*.png" +
                "|JPG (.jpg)|*.jpg"; // Filter files by extension SVG, SVGZ
            dlg.ShowDialog();
            _manager = new DatabaseManager("localhost", "Duch003", "Killer003", "5432", "MyDictionaryApp_IntegrationTests");
            do {
                OpenLogInScreen();
            } while(GlobalSettings.ID == 0);
            var result = _manager.TakeCategories(GlobalSettings.ID, out var temp);
            if(result) {
                ConvertCategories(temp);
                //lbCategories.ItemsSource = Categories;
            } else
                throw new Exception("Method: MainWindow. Cannot assign categories to MainWindow.");
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
            var result = _manager.TakeCategories(GlobalSettings.ID, out var newCategories);
            if(result) {
                ConvertCategories(newCategories);
                lbCategories.ItemsSource = Categories;
            }
            this.IsEnabled = true;
        }

        private void ParticularCategory()
        {
            this.IsEnabled = false;
            var query = from z in _categories
                        where z.Name == lbCategories.SelectedValue.ToString()
                        select z;
            if(query.Any()) {
                var result = _manager.GameSet_ParticularCategory(GlobalSettings.ID, query.First().ID, out var data);
                if(result) {
                    var convertedProperly = ConvertToList(data, out var listQuestions);
                    if(convertedProperly) {
                        MainGameBoard game = new MainGameBoard(listQuestions, false);
                    } else {
                        MessageBox.Show("Method: ParticularCategory. An error occured while creating GameBoard. Can't convert result to list.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }  
                } else
                    MessageBox.Show("Method: ParticularCategory. An error occured while creating GameBoard. There are no rows in return.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            } else 
                MessageBox.Show($"Method: ParticularCategory. An error occured while creating GameBoard. " +
                    $"Category {lbCategories.SelectedValue.ToString()} doesnt exists", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            
            this.IsEnabled = true;
        }

        private void RandomVocabSet() {
            this.IsEnabled = false;
            var result = _manager.GameSet_RandomVocabulary(GlobalSettings.ID, GlobalSettings.RandomVocabulaySize, out var data);
            if(result) {
                var convertedProperly = ConvertToList(data, out var listQuestions);
                if(convertedProperly) {
                    MainGameBoard game = new MainGameBoard(listQuestions, false);
                } else
                    MessageBox.Show("Method: RandomVocabSet. An error occured while creating GameBoard. Can't convert result to list.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            } else
                MessageBox.Show("Method: RandomVocabSet. An error occured while creating GameBoard. There are no rows in return.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            this.IsEnabled = true;
        }

        private void TimeChallange() {
            this.IsEnabled = false;
            var result = _manager.GameSet_WholeDictionary(GlobalSettings.ID, out var temp);
            if(result) {
                var convertedProperly = ConvertToList(temp, out var listQuestion);
                if(convertedProperly) {
                    MainGameBoard game = new MainGameBoard(listQuestion, true);
                } else
                    MessageBox.Show("Method: TimeChallange. An error occured while creating GameBoard. Can't convert result to list.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            } else
                MessageBox.Show("Method: TimeChallange. An error occured while creating GameBoard. There are no rows in return.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            this.IsEnabled = true;
        }

        private void WholeDictionary() {
            this.IsEnabled = false;
            var result = _manager.GameSet_WholeDictionary(GlobalSettings.ID, out var temp);
            if(result) {
                var convertedProperly = ConvertToList(temp, out var listQuestion);
                if(convertedProperly) {
                    MainGameBoard game = new MainGameBoard(listQuestion, false);
                } else
                    MessageBox.Show("Method: WholeDictionary. An error occured while creating GameBoard. Can't convert result to list.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            } else
                MessageBox.Show("Method: WholeDictionary. An error occured while creating GameBoard. There are no rows in return.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            this.IsEnabled = true;
        }
        #endregion

        //False raw is null, 0 rows or number of columns different from 7
        //True if successfull
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

            if(!(raw.Tables[0].Columns.Count == 7)) {
                processed = null;
                return false;
            }

            processed = new List<Question>();

            for(int i = 0; i < raw.Tables[0].Rows.Count; i++) {
                var id = Convert.ToInt32(raw.Tables[0].Rows[i][0]);     //Question ID
                var eng = raw.Tables[0].Rows[i][1].ToString();          //Question ENG
                var pl = raw.Tables[0].Rows[i][2].ToString();           //Question PL
                var percentage = Convert.ToDouble(raw.Tables[0].Rows[i][3]); //Percentage
                var img = raw.Tables[0].Rows[i][4].ToString();          //Img path

                var cat_id = Convert.ToInt32(raw.Tables[0].Rows[i][5]); //Category ID
                var cat_name = raw.Tables[0].Rows[i][6].ToString();     //Category Name

                var tempCat = new Category(cat_id, cat_name);
                var tempQues = new Question(id, eng, pl, tempCat, percentage, img);

                processed.Add(tempQues);
            }
            return true;
                
        }

        private bool ConvertCategories(DataSet raw) {
            if(raw is null) {
                return false;
            }

            if(raw.Tables[0].Rows.Count == 0) {
                return false;
            }

            if(!(raw.Tables[0].Columns.Count == 2)) {
                return false;
            }

            for(int i = 0; i < raw.Tables[0].Rows.Count; i++) {
                var cat_id = Convert.ToInt32(raw.Tables[0].Rows[i][0]); //Category ID
                var cat_name = raw.Tables[0].Rows[i][1].ToString();     //Category Name

                var tempCat = new Category(cat_id, cat_name);

                _categories.Add(tempCat);
                Categories.Add(cat_name);
            }
            return true;
        }

        #region Events
        private void Settings_Click(object sender, RoutedEventArgs e) => OpenSettings();

        private void RandomVocabSet_Click(object sender, RoutedEventArgs e) => RandomVocabSet();

        private void ParticularCategory_Click(object sender, RoutedEventArgs e) => ParticularCategory();

        private void TimeChallange_Click(object sender, RoutedEventArgs e) => TimeChallange();

        private void WholeDictionary_Click(object sender, RoutedEventArgs e) => WholeDictionary();

        private void EditDictionary_Click(object sender, RoutedEventArgs e) => OpenDictionaryManager();

        private void EditCategories_Click(object sender, RoutedEventArgs e) => OpenCategoryManager();

        private void About_Click(object sender, RoutedEventArgs e) => OpenAbout();
        #endregion
    }
}

