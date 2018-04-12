using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ClickEnglish {
    /// <summary>
    /// Interaction logic for DictionaryManager.xaml
    /// </summary>
    public partial class DictionaryManager : Window {
        private DatabaseManager _manager;
        public ObservableCollection<Question> Data;
        public ObservableCollection<Category> Category;

        public DictionaryManager(DatabaseManager manager) {
            InitializeComponent();
            _manager = manager;
            var result = manager.TakeDictionary(GlobalSettings.ID, out var dictionaryData);
            if(result)
                LoadDictionary(dictionaryData);
        }

        private bool LoadDictionary(DataSet raw)
        {
            if (raw == null)
                throw new Exception("Method: LoadDictionary. Raw DataSet is null.");
            if (raw.Tables[0].Rows.Count == 0)
                return false;
            Data = new ObservableCollection<Question>();
            Category = new ObservableCollection<Category>();
            for(int i = 0; i < raw.Tables[0].Rows.Count; i++)
            {
                var tempId = Convert.ToInt32(raw.Tables[0].Rows[i][0]);
                var tempEng = raw.Tables[0].Rows[i][1].ToString();
                var tempPl = raw.Tables[0].Rows[i][2].ToString();
                var tempPerc = Convert.ToDouble(raw.Tables[0].Rows[i][3]);
                var tempImg = raw.Tables[0].Rows[i][4].ToString();

                var tempCatId = Convert.ToInt32(raw.Tables[0].Rows[i][6]);
                var tempCatName = raw.Tables[0].Rows[i][5].ToString();
                var tempCat = new Category(tempCatId, tempCatName);

                Category.Add(tempCat);

                var tempQuestion = new Question(tempId, tempEng, tempPl, tempCat, tempPerc, tempImg);
                Data.Add(tempQuestion);
            }
            dgDictionary.DataContext = Data;
            dgDictionary.ItemsSource = Data;
            cbCategories.ItemsSource = Category;
            return true;
        }

        private void AddNewRecord(object sender, AddingNewItemEventArgs e) {

        }

        private void EditRecord_Begin(object sender, DataGridBeginningEditEventArgs e) {
            
        }

        private void EditRecord_End(object sender, DataGridCellEditEndingEventArgs e) {
            
        }

        private void Category_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Searcher_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void AddWord_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditWord_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveWord_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ExitWithoutSaving_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ExitSavingChanges_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
