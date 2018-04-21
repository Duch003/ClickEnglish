using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;

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

        #region Methods
        /// <summary>
        /// Read data inside of argument and convert it into ObservableCollection
        /// </summary>
        /// <param name="raw">Raw data downloaded from database.</param>
        /// <returns>True if readed successfully, false if argument has 0 rows, Exception id argument is null</returns>
        public bool LoadCategories(DataSet raw)
        {
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
        #endregion

        #region Events
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var tempCat = new Category(0, "Name");
            _manager.AddNewCategory(GlobalSettings.ID, tempCat);
            var result = _manager.TakeCategories(GlobalSettings.ID, out var categoryData);
            if(result) {
                LoadCategories(categoryData);
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            _manager.RemoveCategory((dgCategory.SelectedItem as Category).ID);
            var result = _manager.TakeCategories(GlobalSettings.ID, out var categoryData);
            if(result) {
                LoadCategories(categoryData);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void EditCategory_End(object sender, DataGridCellEditEndingEventArgs e)
        {
            var newRecord = e.Row.Item as Category;
            var change = (e.EditingElement as TextBox).Text;
            newRecord.Name = change;

            _manager.UpdateCategory(newRecord);
            var result = _manager.TakeCategories(GlobalSettings.ID, out var categoryData);
            if(result) {
                LoadCategories(categoryData);
            }
        }
        #endregion
    }
}
