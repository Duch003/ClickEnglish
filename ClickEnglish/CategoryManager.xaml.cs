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
                dgCategory.ItemsSource = ctx.Categories.Local;
                dgCategory.DataContext = ctx.Categories.Local;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            using (var ctx = new DictionaryContext())
            {
                var tempCategory = new Category();
                tempCategory.Name = "Category name";
                ctx.Categories.Add(tempCategory);
                ctx.SaveChanges();
                dgCategory.ItemsSource = ctx.Categories.Local;
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            using (var ctx = new DictionaryContext())
            {
                var tempCategory = (from z in ctx.Categories
                                    where (dgCategory.SelectedItem as Category).ID == z.ID
                                    select z).First();
                ctx.Categories.Remove(tempCategory);
                ctx.SaveChanges();
                dgCategory.ItemsSource = ctx.Categories.Local;
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void EditCategory_End(object sender, DataGridCellEditEndingEventArgs e)
        {
            var editedCategory = dgCategory.SelectedItem as Category;
            using (var ctx = new DictionaryContext())
            {
                var temp = ctx.Categories.Where(z => z.ID == editedCategory.ID).First();
                temp.Name = editedCategory.Name;
                ctx.SaveChanges();
                dgCategory.ItemsSource = ctx.Categories.Local;
            }
        }
    }
}
