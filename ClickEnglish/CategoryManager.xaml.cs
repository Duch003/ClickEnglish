using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        private void Exit_Click(object sender, RoutedEventArgs e) => SaveChanges();

        private void EditCategory_End(object sender, DataGridCellEditEndingEventArgs e) => EditCategory(dgCategory.SelectedItem as Category, e);

        private void PreviewKeyDown(object sender, KeyEventArgs e) 
        {
            DataGridRow dgr = (DataGridRow)(dgCategory.ItemContainerGenerator.ContainerFromIndex(dgCategory.SelectedIndex));
            if (dgr.IsEditing) return;
            switch (e.Key)
            {
                case Key.Delete:
                    if ((dgCategory.SelectedItem as Category) == null) return;
                    RemoveCategory(dgCategory.SelectedItem as Category);
                    break;
                case Key.OemPlus:
                    AddCategory();
                    break;
            }

        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            foreach (var z in dgCategory.Columns)
            {
                if (z.Header.ToString() == nameof(Category.ID) || z.Header.ToString() == nameof(Category.Words))
                    z.IsReadOnly = true;
                else if (z.Header.ToString() == nameof(Category.Name))
                    z.Width = 400;
            }
        }
        #endregion

        #region Methods
        private void EditCategory(Category category, DataGridCellEditEndingEventArgs e)
        {
            var newText = (e.EditingElement as TextBox).Text;
            if (string.IsNullOrEmpty(newText))
                return;
            using (var ctx = new DictionaryContext())
            {
                var temp = ctx.Categories.Where(z => z.ID == category.ID);
                if(temp == null)
                    return;
                if (temp.Count() == 0)
                    return;
                
                temp.First().Name = newText;
                ctx.SaveChanges();
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
            var selected = dgCategory.SelectedIndex;
            using (var ctx = new DictionaryContext())
            {
                var tempCategory = (from z in ctx.Categories
                                    where category.ID == z.ID
                                    select z).FirstOrDefault();
                if (tempCategory == null) return;
                ctx.Categories.Remove(tempCategory);
               
                ctx.SaveChanges();
                dgCategory.ItemsSource = new ObservableCollection<Category>(ctx.Categories.ToList());
            }
            if (dgCategory.Items.Count > 0)
                dgCategory.SelectedIndex = 0;
        }

        private void SaveChanges()
        {
            using (var ctx = new DictionaryContext())
            {
                ctx.SaveChanges();
            }
            this.Close();
        }
        #endregion
    }
}
