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
                var countCategories = (from z in ctx.Categories
                                       select z).Count();
                if (countCategories == 0)
                    ctx.Categories.Add(new Category()
                    {
                        CategoryID = 0,
                        Title = "None"
                    });
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
                if (z.Header.ToString() == nameof(Category.CategoryID) || z.Header.ToString() == nameof(Category.Words))
                    z.IsReadOnly = true;
                else if (z.Header.ToString() == nameof(Category.Title))
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
                var temp = ctx.Categories.Where(z => z.CategoryID == category.CategoryID);
                if(temp == null)
                    return;
                if (temp.Count() == 0)
                    return;
                
                temp.First().Title = newText;
                ctx.SaveChanges();
            }
        }

        private void AddCategory()
        {

            //TODO Czy nie połączyć obydwu managerów? Ułatwi to zarządzenie
            //TODO Dopisać kod pilnujący by nie powtarzały się nazwy kategorii
            //TODO Formularze?
            using (var ctx = new DictionaryContext())
            {
                var tempCategory = new Category();
                tempCategory.Title = "Category name";
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
                                    where category.CategoryID == z.CategoryID
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
