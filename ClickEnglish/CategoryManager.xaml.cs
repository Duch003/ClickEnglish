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

namespace ClickEnglish
{
    /// <summary>
    /// Interaction logic for CategoryManager.xaml
    /// </summary>
    public partial class CategoryManager : Window
    {
        private DatabaseManager _manager;
        public ObservableCollection<Category> Data { get; set; }

        public CategoryManager(DatabaseManager manager)
        {
            InitializeComponent();
            _manager = manager;
            var result = _manager.TakeCategories(GlobalSettings.ID, out var categoriesData);
            if(result) {
                LoadCategories(categoriesData);
            }
        }

        //Load categories used by user
        private bool LoadCategories(DataSet raw) {
            if(raw is null)
                throw new Exception("Method: LoadCategories. Raw DataSet is null.");
            if(raw.Tables[0].Rows.Count == 0)
                return false;

            Data = new ObservableCollection<Category>();

            for(int i = 0; i < raw.Tables[0].Rows.Count; i++) {
                var tempId = Convert.ToInt32(raw.Tables[0].Rows[i][0]);
                var tempName = raw.Tables[0].Rows[i][1].ToString();

                Data.Add(new Category(tempId, tempName));
            }
            dgCategory.ItemsSource = Data;
            return true;
        }

        private void Add_Click(object sender, RoutedEventArgs e) {
            var tempCat = CategoryList.First();
            var tempQues = new Question(0, "eng", "pl", tempCat, 0, "none");
            _manager.AddNewRecord(GlobalSettings.ID, tempQues);
            var result = _manager.TakeDictionary(GlobalSettings.ID, out var dictionaryData);
            if(result) {
                LoadDictionary(dictionaryData);
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e) {


        }

        private void Exit_Click(object sender, RoutedEventArgs e) {


        }

        private void EditCategory_End(object sender, DataGridCellEditEndingEventArgs e) {
            var newRecord = e.Row.Item as Category;
            var change = (e.EditingElement as TextBox).Text;
            newRecord.Name = change;

            _manager.UpdateCategory(newRecord);
            var result = _manager.TakeCategories(GlobalSettings.ID, out var categoryData);
            if(result) {
                LoadCategories(categoryData);
            }
        }
    }
}
