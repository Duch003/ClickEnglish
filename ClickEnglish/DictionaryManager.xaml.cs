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
        private string _tempWord;
        private byte[] _tempPicture;

        public DictionaryManager()
        {
            InitializeComponent();
            using(var ctx = new DictionaryContext())
            {
                dgDictionary.ItemsSource = new ObservableCollection<Word>(ctx.Dictionary.ToList());
            }
        }

        #region Events
        private void Searcher_TextChanged(object sender, TextChangedEventArgs e)
        {
            using (var ctx = new DictionaryContext())
            {
                if (ctx.Dictionary.Count() != 0)
                {
                    if (string.IsNullOrEmpty((sender as TextBox).Text))
                    {
                        dgDictionary.ItemsSource = new ObservableCollection<Word>(ctx.Dictionary.ToList());
                    }
                    else
                    {
                        var pattern = (sender as TextBox).Text;
                        var newList = (from z in ctx.Dictionary
                                       where
                                       z.Polish.Contains(pattern) ||
                                       z.English.Contains(pattern) ||
                                       z.Category.Name.Contains(pattern)
                                       select z).ToList();
                        dgDictionary.ItemsSource = newList;
                    }
                }
            }
        }

        private void AddWord_Click(object sender, RoutedEventArgs e) => AddWord();

        private void RemoveWord_Click(object sender, RoutedEventArgs e) => RemoveWord(dgDictionary.SelectedItem as Word);

        private void Exit_Click(object sender, RoutedEventArgs e) => SaveChanges(); 

        private void EditDictionary_End(object sender, DataGridCellEditEndingEventArgs e) => EditWord(dgDictionary.SelectedItem as Word, e);

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
            foreach (var z in dgDictionary.Columns)
            {
                if (z.Header.ToString() == nameof(Word.ID) || z.Header.ToString() == nameof(Word.Difficulty))
                    z.IsReadOnly = true;
            }
            using (var ctx = new DictionaryContext())
            {
                var categoryList = (from z in ctx.Categories
                                    select z).ToList();

                dgCbxCategories.ItemsSource = categoryList;
            }
        }
        #endregion

        #region Methods
        private void AddWord()
        {
            using (var ctx = new DictionaryContext())
            {
                var tempWord = new Word();
                tempWord.Polish = "Polskie wyrażenie";
                tempWord.English = "English expression";
                tempWord.Difficulty = 100;
                byte[] image = null;/*PictureConverter.ConvertBitmapSourceToByteArray(new System.Uri("pack://application:,,,/background/default_picture.jpg"));*/
                tempWord.Picture = image;

                ctx.Dictionary.Add(tempWord);
                ctx.SaveChanges();
                dgDictionary.ItemsSource = new ObservableCollection<Word>(ctx.Dictionary.ToList());
            }
            Refresh();
        }

        private void RemoveWord(Word word)
        {
            if (word == null) return;
            var selected = dgDictionary.SelectedIndex;
            using (var ctx = new DictionaryContext())
            {
                var tempCategory = (from z in ctx.Categories
                                    where word.ID == z.ID
                                    select z).FirstOrDefault();
                if (tempCategory == null) return;
                ctx.Categories.Remove(tempCategory);

                ctx.SaveChanges();
                dgDictionary.ItemsSource = new ObservableCollection<Word>(ctx.Dictionary.ToList());
            }
            if (dgDictionary.Items.Count > 0)
                dgDictionary.SelectedIndex = 0;
            Refresh();
        }

        private void SaveChanges()
        {
            using (var ctx = new DictionaryContext())
            {
                ctx.SaveChanges();
            }
            this.Close();
        }

        private void EditWord(Word word, DataGridCellEditEndingEventArgs e)
        {
            //TODO Pomimo wyboru kategorii nie zapisuje się ona

            switch (e.Column.Header.ToString())
            {
                case nameof(Word.Category):
                    var newCategory = (e.EditingElement as ComboBox).SelectedItem as Category;
                    using (var ctx = new DictionaryContext())
                    {
                        var temp = (from z in ctx.Dictionary
                                    where word.ID == z.ID
                                    select z).First();
                        temp.Category = newCategory;
                        MessageBox.Show(temp.Category.Name);
                        ctx.SaveChanges();
                    }
                    break;
                case nameof(Word.English):
                    var newTextEnglish = (e.EditingElement as TextBox).Text;
                    if (string.IsNullOrEmpty(newTextEnglish))
                        return;
                    using (var ctx = new DictionaryContext())
                    {
                        var temp = (from z in ctx.Dictionary
                                    where word.ID == z.ID
                                    select z).First();
                        temp.English = newTextEnglish;
                        ctx.SaveChanges();
                    }
                    break;
                case nameof(Word.Polish):
                    //TODO W evencie wywolywanym przy rozpoczeciu edytcji pola zapisac stara wartosc na zas
                    var newTextPolish = (e.EditingElement as TextBox).Text;
                    if (string.IsNullOrEmpty(newTextPolish))
                      return;
                    using (var ctx = new DictionaryContext())
                    {
                        var temp = (from z in ctx.Dictionary
                                    where word.ID == z.ID
                                    select z).First();
                        temp.Polish = newTextPolish;
                        ctx.SaveChanges();
                    }
                    break;
                case nameof(Word.Picture):
                    using (var ctx = new DictionaryContext())
                    {
                        var temp = (from z in ctx.Dictionary
                                    where word.ID == z.ID
                                    select z).First();
                        temp.Picture = _tempPicture;
                        (dgDictionary.SelectedItem as Word).Picture = _tempPicture;
                        ctx.SaveChanges();
                    }
                    _tempPicture = null;
                    break;
            }
            Refresh();
        }

        private byte[] PictureManager()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".jpeg";
            dlg.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            dlg.Title = "Select your hint picture.";
            dlg.Multiselect = false;

            var ans = dlg.ShowDialog();
            switch (ans)
            {
                case true:
                    var file = dlg.OpenFile();
                    using (var memoryStream = new MemoryStream())
                    {
                        file.CopyTo(memoryStream);
                        return memoryStream.ToArray();
                    }
                default:
                    return null;
            }
        }

        #endregion

        private void EditDictionary_Begin(object sender, DataGridBeginningEditEventArgs e)
        {
            if(e.Column.Header.ToString() == nameof(Word.Picture))
            {
                _tempPicture = PictureManager();
            }
        }

        private void ClearPicture(object sender, RoutedEventArgs e)
        {
            var word = dgDictionary.SelectedItem as Word;
            using (var ctx = new DictionaryContext())
            {
                var temp = (from z in ctx.Dictionary
                            where word.ID == z.ID
                            select z).First();
                temp.Picture = null;
                (dgDictionary.SelectedItem as Word).Picture = null;
                
                ctx.SaveChanges();
            }
            Refresh();
        }

        private void Refresh()
        {
            var temp = dgDictionary.ItemsSource;
            dgDictionary.ItemsSource = null;
            dgDictionary.ItemsSource = temp;
        }
    }

    public class ImageByteConverter : IValueConverter
    {
        //Image to byte[]
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }

        //byte[] to image
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
