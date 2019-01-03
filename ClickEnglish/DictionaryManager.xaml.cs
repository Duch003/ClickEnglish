using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ClickEnglish
{

    /// <summary>
    /// Interaction logic for DictionaryManager.xaml
    /// </summary>
    public partial class DictionaryManager : Window
    {

        private string _valueBeforeEdit;
        private byte[] _pictureBeforeEdit;
        private byte[] _pictureToSave;

        public DictionaryManager()
        {
            InitializeComponent();
            SetDictionary();
            SetCategoryList();
            
        }

        private void ReloadView()
        {
            var tempSource = dgDictionary.ItemsSource;
            dgDictionary.ItemsSource = null;
            dgDictionary.ItemsSource = tempSource;
        }

        private void SetCategoryList()
        {
            using (var ctx = new DictionaryContext())
            {
                var categories = (from z in ctx.Categories
                                  select z.Title).ToList<string>();
                var countCategories = (from z in ctx.Categories
                                       select z).Count();
                if (countCategories == 0)
                {
                    categories.Add("None");
                    ctx.Categories.Add(new Category(){
                        Title = "None"
                    });
                }
                    
                if (!categories.Contains("All"))
                {
                    categories.Add("All");
                    ctx.Categories.Add(new Category(){
                        Title = "All"
                    });
                }
                    

                dgcbxCategories.ItemsSource = categories;
                cbxCategories.ItemsSource = categories;
                cbxCategories.SelectedValue = "All";
            }
        }

        private void SetDictionary()
        {
            using (var ctx = new DictionaryContext())
            {
                var joinedTables = (from category in ctx.Categories
                                    join
                                    word in ctx.Dictionary
                                    on category.CategoryID equals word.CategoryID
                                    select new TemporaryWord
                                    {
                                        ID = word.ID,
                                        English = word.English,
                                        Polish = word.Polish,
                                        Title = category.Title,
                                        Difficulty = word.Difficulty,
                                        Picture = word.Picture
                                    });

                dgDictionary.ItemsSource = ResultConverter.ToObservableCollection(joinedTables);
            }
        }

        private void SetDictionary(int categoryID)
        {
            using (var ctx = new DictionaryContext())
            {
                var joinedTables = (from category in ctx.Categories
                                    join
                                    word in ctx.Dictionary
                                    on category.CategoryID equals word.CategoryID
                                    where word.CategoryID == categoryID
                                    select new TemporaryWord
                                    {
                                        ID = word.ID,
                                        English = word.English,
                                        Polish = word.Polish,
                                        Title = category.Title,
                                        Difficulty = word.Difficulty,
                                        Picture = word.Picture
                                    });

                dgDictionary.ItemsSource = ResultConverter.ToObservableCollection(joinedTables);
            }
        }

        private void SetDictionary(string filter)
        {
            using (var ctx = new DictionaryContext())
            {
                var joinedTables = (from category in ctx.Categories
                                    join
                                    word in ctx.Dictionary
                                    on category.CategoryID equals word.CategoryID
                                    where (word.English.Contains(filter) || word.Polish.Contains(filter))
                                    select new TemporaryWord
                                    {
                                        ID = word.ID,
                                        English = word.English,
                                        Polish = word.Polish,
                                        Title = category.Title,
                                        Difficulty = word.Difficulty,
                                        Picture = word.Picture
                                    });

                dgDictionary.ItemsSource = ResultConverter.ToObservableCollection(joinedTables);
            }
        }

        private void SetDictionary(int categoryID, string filter)
        {
            using (var ctx = new DictionaryContext())
            {
                var joinedTables = (from category in ctx.Categories
                                    join
                                    word in ctx.Dictionary
                                    on category.CategoryID equals word.CategoryID
                                    where word.CategoryID == categoryID && (word.English.Contains(filter) || word.Polish.Contains(filter))
                                    select new TemporaryWord
                                    {
                                        ID = word.ID,
                                        English = word.English,
                                        Polish = word.Polish,
                                        Title = category.Title,
                                        Difficulty = word.Difficulty,
                                        Picture = word.Picture
                                    });

                dgDictionary.ItemsSource = ResultConverter.ToObservableCollection(joinedTables);
            }
        }

        //TODO Co sie stanie jak usune kategorie kiedy sa do niej przypisane slowa?

        private void Filter(string category, string filter)
        {
            //Category == "All", Filter == ""
            if (category == "All" && string.IsNullOrEmpty(filter))
            {
                SetDictionary();
            }

            //Category == "All", Filter == "Some Filter"
            else if (category == "All" && !string.IsNullOrEmpty(filter))
            {
                SetDictionary(filter);
            }

            //Category == "Selected Category", Filter == "Some Filter"
            else if (category != "All" && !string.IsNullOrEmpty(filter))
            {
                Category findCategory;
                using (var ctx = new DictionaryContext())
                {
                    findCategory = (from z in ctx.Categories
                                        select z).ToList().Find(z => z.Title.Equals(category));
                }
                SetDictionary(findCategory.CategoryID, filter);
            }

            //Category == "Selected Category", Filter == ""
            else if (category != "All" && string.IsNullOrEmpty(filter))
            {
                Category findCategory;
                using (var ctx = new DictionaryContext())
                {
                    findCategory = (from z in ctx.Categories
                                    select z).ToList().Find(z => z.Title.Equals(category));
                }
                SetDictionary(findCategory.CategoryID);
            }
            //Reload view
            ReloadView();
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            //Prevent editing ID, Difficulty
            foreach (var z in dgDictionary.Columns)
            {
                if (z.Header.ToString() == nameof(TemporaryWord.ID) || z.Header.ToString() == nameof(TemporaryWord.Difficulty))
                    z.IsReadOnly = true;
            }
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender as TextBox) == null || cbxCategories == null)
                return;
            Filter(cbxCategories.SelectedValue.ToString(), txtSearcher.Text);
        }

        private void CategoryChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox) == null || txtSearcher == null)
                return;
            Filter(cbxCategories.SelectedValue.ToString(), txtSearcher.Text);
        }

        private void AddWord_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveWord_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ClearPicture(object sender, RoutedEventArgs e)
        {

        }

        //Manage particular change
        private void EditDictionary_End(object sender, DataGridCellEditEndingEventArgs e)
        {
            //In English or Polish
            if (e.EditingElement is TextBox)
            {
                if(string.IsNullOrEmpty((e.EditingElement as TextBox).Text))
                {
                    (e.EditingElement as TextBox).Text = _valueBeforeEdit;
                    dgDictionary.CancelEdit();
                    return;
                }
                else
                {
                    var id = (dgDictionary.SelectedItem as TemporaryWord).ID;
                    using (var ctx = new DictionaryContext())
                    {
                        var temp = ctx.Dictionary.Find(id);
                        switch (e.Column.Header.ToString())
                        {
                            case nameof(TemporaryWord.Polish):
                                temp.Polish = (sender as TextBox).Text;
                                break;
                            case nameof(TemporaryWord.English):
                                temp.English = (sender as TextBox).Text;
                                break;
                        }
                        ctx.SaveChanges();
                    }
                }
            }
            //In category
            else if(e.EditingElement is ComboBox)
            {
                var category = (e.EditingElement as ComboBox).SelectedValue.ToString();
                var id = (dgDictionary.SelectedItem as TemporaryWord).ID;
                using (var ctx = new DictionaryContext())
                {
                    var tempWord = ctx.Dictionary.Find(id);
                    var tempCat = ctx.Categories.Find(category);
                    tempWord.CategoryID = tempCat.CategoryID;
                    tempWord.Category = tempCat;
                    ctx.SaveChanges();
                }
            }
            //In picture
            else
            {
                var id = (dgDictionary.SelectedItem as TemporaryWord).ID;
                using (var ctx = new DictionaryContext())
                {
                    var temp = ctx.Dictionary.Find(id);
                    temp.Picture = _pictureToSave;
                    _pictureToSave = null;
                    _pictureBeforeEdit = null;
                    ctx.SaveChanges();
                }
            }
            Filter(cbxCategories.SelectedValue.ToString(), txtSearcher.Text);
        }

        private byte[] PictureManager()
        {
            //Create dialog
            OpenFileDialog dlg = new OpenFileDialog
            {
                DefaultExt = ".jpeg",
                Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png",
                Title = "Select your hint picture.",
                Multiselect = false
            };
            //Show dialog
            var ans = dlg.ShowDialog();
            switch (ans)
            {
                //If user select picture
                case true:
                    var file = dlg.OpenFile();
                    byte[] converted;
                    using (var memoryStream = new MemoryStream())
                    {
                        file.CopyTo(memoryStream);
                        converted = memoryStream.ToArray();
                    }
                    return converted;
                //If not
                default:
                    return null;
            }
        }

        //Save current values in case user cancel edit or type nothing
        private void EditDictionary_Begin(object sender, DataGridBeginningEditEventArgs e)
        {
            //TextBox case
            if (dgDictionary.CurrentItem is TextBox || dgDictionary.CurrentItem is TextBlock)
            {
                _valueBeforeEdit = (dgDictionary.CurrentItem as TextBox).Text;
            }
            //Combobox case
            else if(dgDictionary.CurrentItem is ComboBox)
            {
                return;
            }
            //Picture case
            else
            {
                _pictureBeforeEdit = dgDictionary.SelectedValue as byte[];
                var temp = PictureManager();
                if(temp == null)
                {
                    dgDictionary.CancelEdit();
                    return;
                }
                _pictureToSave = temp;
            }
        }

        private void KeyPreview(object sender, KeyEventArgs e)
        {

        }
    }


    public static class ResultConverter
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IQueryable<T> en)
        {
            return new ObservableCollection<T>(en);
        }
    }

    public class TemporaryWord : IQuestion
    {
        public int ID { get; set; }
        public string English { get; set; }
        public string Polish { get; set; }
        public Category Category { get; set; }
        public double Difficulty { get; set; }
        public byte[] Picture { get; set; }
        public string Title { get; set; }
    }

}
