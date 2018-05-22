using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ClickEnglish
{
    /// <summary>
    /// Interaction logic for CategoryManager.xaml
    /// </summary>
    public partial class CategoryManager : Window
    {
        public CategoryManager()
        {
            InitializeComponent();
            using (var ctx = new DictionaryContext())
            {
                dgCategory.ItemsSource = new ObservableCollection<Category>(ctx.Categories.ToList());
            }
        }

        #region Events
        private void Add_Click(object sender, RoutedEventArgs e) => AddCategory();

        private void Remove_Click(object sender, RoutedEventArgs e) => RemoveCategory(dgCategory.SelectedItem as Category);

        private void Exit_Click(object sender, RoutedEventArgs e) => Close();

        private void EditCategory_End(object sender, DataGridCellEditEndingEventArgs e) => EditCategory(dgCategory.SelectedItem as Category, e);

        private void KeyClick(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case System.Windows.Input.Key.Delete:
                    RemoveCategory(dgCategory.SelectedItem as Category);
                    break;
                case System.Windows.Input.Key.OemPlus:
                    AddCategory();
                    break;
            }
        }
        #endregion

        #region Methods
        private void EditCategory(Category category, DataGridCellEditEndingEventArgs e)
        {
            var newText = (e.EditingElement as TextBox).Text;
            using (var ctx = new DictionaryContext())
            {
                var temp = ctx.Categories.Where(z => z.ID == category.ID).First();
                temp.Name = newText;
                ctx.SaveChanges();
                //TODO Po zatwierdzeniu enterem index zaznaczonego itemu skacze o jeden, a nastepny nie istnieje, jezeli jest zaznaczony ostatni.
                dgCategory.ItemsSource = new ObservableCollection<Category>(ctx.Categories.ToList());
            }
        }

        private void AddCategory()
        {
            using (var ctx = new DictionaryContext())
            {
                var tempCategory = new Category();
                tempCategory.Name = "Category name";
                ctx.Categories.Local.Add(tempCategory);
                ctx.SaveChanges();
                dgCategory.ItemsSource = new ObservableCollection<Category>(ctx.Categories.ToList());
            }
        }

        private void RemoveCategory(Category category)
        {
            if (category == null) return;
            using (var ctx = new DictionaryContext())
            {
                var tempCategory = (from z in ctx.Categories
                                    where category.ID == z.ID
                                    select z).FirstOrDefault();
                ctx.Categories.Remove(tempCategory);
                ctx.SaveChanges();
                dgCategory.ItemsSource = new ObservableCollection<Category>(ctx.Categories.ToList());
            }
        }
        #endregion
    }
}
