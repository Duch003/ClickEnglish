using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
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
        private byte[] _tempPicture;

        public DictionaryManager()
        {
            InitializeComponent();
            using(var ctx = new DictionaryContext())
            {
                var temp = new ObservableCollection<Word>(ctx.Dictionary.ToList());

                var listCategories = (from z in ctx.Categories
                                      select z).ToList();
                var countCategories = (from z in ctx.Categories
                                       select z).Count();
                if (countCategories == 0)
                    ctx.Categories.Add(new Category()
                    {
                        CategoryID = 0,
                        Title = "None"
                    });
                dgcbxCategories.ItemsSource = new ObservableCollection<Category>(listCategories);
                
                dgDictionary.ItemsSource = temp;
                dgDictionary.DataContext = ctx.Dictionary;
            }
        }

        #region Events
        //TODO Dalej nie wyświetla kategorii
        private void Searcher_TextChanged(object sender, TextChangedEventArgs e)            => Refresh(cbxCategories.SelectedValue.ToString(), txtSearcher.Text);

        private void AddWord_Click(object sender, RoutedEventArgs e)                        => AddWord();

        private void RemoveWord_Click(object sender, RoutedEventArgs e)                     => RemoveWord(dgDictionary.SelectedItem as Word);

        private void Exit_Click(object sender, RoutedEventArgs e)                           => SaveChanges(); 

        private void EditDictionary_End(object sender, DataGridCellEditEndingEventArgs e)   => EditWord(dgDictionary.SelectedItem as Word, e);

        private void KeyPreview(object sender, KeyEventArgs e)
        {
            DataGridRow dgr = (DataGridRow)(dgDictionary.ItemContainerGenerator.ContainerFromIndex(dgDictionary.SelectedIndex));
            if (dgr.IsEditing) return;
            switch (e.Key)
            {
                case Key.Delete:
                    if ((dgDictionary.SelectedItem as Word) == null) return;
                    RemoveWord(dgDictionary.SelectedItem as Word);
                    break;
                case Key.OemPlus:
                    AddWord();
                    break;
            }
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            //Prevent editing ID, Difficulty and Category
            foreach (var z in dgDictionary.Columns)
            {
                if (z.Header.ToString() == nameof(Word.ID) || z.Header.ToString() == nameof(Word.Difficulty)/* || z.Header.ToString() == nameof(Word.Category)*/)
                    z.IsReadOnly = true;
            }
            //Create list of Categories
            using (var ctx = new DictionaryContext())
            {
                var categoryList = (from z in ctx.Categories
                                    select z).ToList();

                ObservableCollection<string> CategoryTitles = new ObservableCollection<string>();
                foreach (var z in categoryList)
                {
                    CategoryTitles.Add(z.Title);
                }
                
                
                cbxCategories.ItemsSource = CategoryTitles;
                cbxCategories.SelectedItem = "None";
            }
        }

        //Reload DataGrid with selected criteria
        private void CategoryChanged(object sender, SelectionChangedEventArgs e)            => Refresh(cbxCategories.SelectedValue.ToString(), txtSearcher.Text);

        //Invoke when user doubleclik on Picture column cell
        //Saves picture in temporary variable
        private void EditDictionary_Begin(object sender, DataGridBeginningEditEventArgs e)
        {
            if (e.Column.Header.ToString() == nameof(Word.Picture))
            {
                _tempPicture = PictureManager();
            }
            
        }

        //Invoke when user click on "Remove picture" 
        private void ClearPicture(object sender, RoutedEventArgs e)
        {
            //Save selected word
            var word = dgDictionary.SelectedItem as Word;
            //Null check
            if (word == null)
                return;
            using (var ctx = new DictionaryContext())
            {
                //Find word, clear picture and Save
                var temp = (from z in ctx.Dictionary
                            where word.ID == z.ID
                            select z).First();
                temp.Picture = null;
                (dgDictionary.SelectedItem as Word).Picture = null;

                ctx.SaveChanges();
            }
            //Reload datagird view
            Refresh(cbxCategories.SelectedValue.ToString(), txtSearcher.Text);
        }
        #endregion

        #region Methods - State Changers
        //Invoke when user click "Add new" button
        private void AddWord()
        {
            using (var ctx = new DictionaryContext())
            {
                //Create word with predefined values
                var tempWord = new Word();
                tempWord.Polish = "Polskie wyrażenie";
                tempWord.English = "English expression";

                //TODO Wywala blad podczas gdy kategoria jest ustawiona na null
                if (cbxCategories.SelectedValue.ToString() == "All")
                    tempWord.Category = null;
                else
                    tempWord.Category = (from z in ctx.Categories
                                         where z.Title == cbxCategories.SelectedValue.ToString()
                                         select z).FirstOrDefault();
                
                tempWord.Difficulty = 100;
                byte[] image = null;/*PictureConverter.ConvertBitmapSourceToByteArray(new System.Uri("pack://application:,,,/background/default_picture.jpg"));*/
                tempWord.Picture = image;

                //Add word to database ans Save
                ctx.Dictionary.Add(tempWord);
                ctx.SaveChanges();
                dgDictionary.ItemsSource = new ObservableCollection<Word>(ctx.Dictionary.ToList());
            }
            //Reload datagrid view
            Refresh(cbxCategories.SelectedValue.ToString(), txtSearcher.Text);
        }

        //Invoke when user click "Remoe selected" button
        private void RemoveWord(Word word)
        {
            //Check selection
            if (word == null)
                return;
            //Save ID
            var selected = (dgDictionary.SelectedItem as Word).ID;
            using (var ctx = new DictionaryContext())
            {
                //Find word and remove
                var tempWord = (from z in ctx.Dictionary
                                    where word.ID == z.ID
                                    select z).FirstOrDefault();
                if (tempWord == null) return;
                ctx.Dictionary.Remove(tempWord);
                //Save database
                ctx.SaveChanges();
                dgDictionary.ItemsSource = new ObservableCollection<Word>(ctx.Dictionary.ToList());
            }
            //Selection protection
            if (dgDictionary.Items.Count > 0)
                dgDictionary.SelectedIndex = 0;
            //Reload datagrid view
            Refresh(cbxCategories.SelectedValue.ToString(), txtSearcher.Text);
        }

        //Invoke when user click "Exit" or "X" button
        private void SaveChanges()
        {
            using (var ctx = new DictionaryContext())
            {
                ctx.SaveChanges();
            }
            this.Close();
        }

        //Invoke when user edit any cell
        private void EditWord(Word word, DataGridCellEditEndingEventArgs e)
        {
            //Switch for column recognition
            switch (e.Column.Header.ToString())
            {
                //------------------ENGLISH WORD-------------------
                case nameof(Word.English):
                    //Save new sentence
                    var newTextEnglish = (e.EditingElement as TextBox).Text;
                    //Null/empty check
                    if (string.IsNullOrEmpty(newTextEnglish))
                        //TODO Zapisać poprzednie słowo tutaj
                        newTextEnglish = "English sentence";
                    using (var ctx = new DictionaryContext())
                    {
                        //Find word by ID and Save
                        var temp = (from z in ctx.Dictionary
                                    where word.ID == z.ID
                                    select z).First();
                        temp.English = newTextEnglish;
                        ctx.SaveChanges();
                    }
                    break;
                //------------------POLISH WORD-------------------
                case nameof(Word.Polish):
                    //Save new sentence
                    var newTextPolish = (e.EditingElement as TextBox).Text;
                    //Null.empty check
                    if (string.IsNullOrEmpty(newTextPolish))
                        newTextPolish = "Polish sentence";
                    using (var ctx = new DictionaryContext())
                    {
                        //Find word by ID and Save
                        var temp = (from z in ctx.Dictionary
                                    where word.ID == z.ID
                                    select z).First();
                        temp.Polish = newTextPolish;
                        ctx.SaveChanges();
                    }
                    break;
                //------------------PICTURE-------------------
                //This case depends on EditDictionary_Begin event
                case nameof(Word.Picture):
                    using (var ctx = new DictionaryContext())
                    {
                        //Find word and Save selected picture as byte[]
                        var temp = (from z in ctx.Dictionary
                                    where word.ID == z.ID
                                    select z).First();
                        temp.Picture = _tempPicture;
                        (dgDictionary.SelectedItem as Word).Picture = _tempPicture;
                        ctx.SaveChanges();
                    }
                    //Clear temporary variable
                    _tempPicture = null;
                    break;
            }
            //Reload datagrid view
            Refresh(cbxCategories.SelectedValue.ToString(), txtSearcher.Text);
        }

        //Invoke when user doubleclick on picture cell
        //Convert picture to byte[] and save in temporary variable
        private byte[] PictureManager()
        {
            //Create dialog
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".jpeg";
            dlg.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            dlg.Title = "Select your hint picture.";
            dlg.Multiselect = false;
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

        //Refresh Datagrid depending on selected Category and Filter
        private void Refresh(string Category, string Filter)
        {
            if (Category == "All" && string.IsNullOrEmpty(Filter))
            {
                using (var ctx = new DictionaryContext())
                {
                    var temp = from z in ctx.Dictionary
                               select z;

                    dgDictionary.ItemsSource = null;
                    dgDictionary.ItemsSource = new ObservableCollection<Word>(temp.ToList());
                }
            }
            //Category == "All", Filter == "Some Filter"
            else if (Category == "All" && !string.IsNullOrEmpty(Filter))
            {
                using (var ctx = new DictionaryContext())
                {
                    var temp = from z in ctx.Dictionary
                               where z.English.Contains(Filter) ||
                               z.Polish.Contains(Filter)
                               select z;

                    dgDictionary.ItemsSource = null;
                    dgDictionary.ItemsSource = new ObservableCollection<Word>(temp.ToList());
                }
            }
            //Category == "Selected Category", Filter == "Some Filter"
            else if (Category != "All" && !string.IsNullOrEmpty(Filter))
            {
                using (var ctx = new DictionaryContext())
                {
                    var temp = from z in ctx.Dictionary
                               where z.Category.Title == Category &&
                               (z.English.Contains(Filter) ||
                               z.Polish.Contains(Filter))
                               select z;

                    dgDictionary.ItemsSource = null;
                    dgDictionary.ItemsSource = new ObservableCollection<Word>(temp.ToList());
                }
            }
            //Category == "Selected Category", Filter == ""
            else if (Category != "All" && string.IsNullOrEmpty(Filter))
            {
                using (var ctx = new DictionaryContext())
                {
                    var temp = from z in ctx.Dictionary
                               where z.Category.Title == Category
                               select z;

                    dgDictionary.ItemsSource = null;
                    dgDictionary.ItemsSource = new ObservableCollection<Word>(temp.ToList());
                }
            }
            //Reload view
            var tempSource = dgDictionary.ItemsSource;
            dgDictionary.ItemsSource = null;
            dgDictionary.ItemsSource = tempSource;
        }
        #endregion

        //public class NullImageConverter : IValueConverter
        //{
        //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //    {
        //        if (value == null)
        //            return DependencyProperty.UnsetValue;
        //        return value;
        //    }

        //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        //    {
        //        // According to https://msdn.microsoft.com/en-us/library/system.windows.data.ivalueconverter.convertback(v=vs.110).aspx#Anchor_1
        //        // (kudos Scott Chamberlain), if you do not support a conversion 
        //        // back you should return a Binding.DoNothing or a 
        //        // DependencyProperty.UnsetValue
        //        return Binding.DoNothing;
        //        // Original code:
        //        // throw new NotImplementedException();
        //    }
        //}

    }

    


}
