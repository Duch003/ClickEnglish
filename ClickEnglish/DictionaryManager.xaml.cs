using Microsoft.Win32;
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
    /// Interaction logic for DictionaryManager.xaml
    /// </summary>
    public partial class DictionaryManager : Window
    {
        public DictionaryManager()
        {
            InitializeComponent();
            using(var ctx = new DictionaryContext())
            {
                dgDictionary.ItemsSource = ctx.Dictionary;
            }
        }

        private void Searcher_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(string.IsNullOrEmpty((sender as TextBox).Text))
                using (var ctx = new DictionaryContext())
                {
                    dgDictionary.ItemsSource = ctx.Dictionary;
                }
            else
            {
                using (var ctx = new DictionaryContext())
                {
                    dgDictionary.ItemsSource = from z in ctx.Dictionary
                                               where
                                               z.Category.Name.Contains((sender as TextBox).Text) ||
                                               z.English.Contains((sender as TextBox).Text) ||
                                               z.Polish.Contains((sender as TextBox).Text)
                                               select z;
                }
            }
        }

        private void AddWord_Click(object sender, RoutedEventArgs e)
        {
            using (var ctx = new DictionaryContext())
            {
                var tempWord = new Word();
                tempWord.Difficulty = 100;
                tempWord.English = "English sentence";
                tempWord.Polish = "Polish sentence";
                tempWord.Category = (from z in ctx.Categories select z).First();
                tempWord.Picture = null;
                ctx.Dictionary.Add(tempWord);
                ctx.SaveChanges();
                dgDictionary.ItemsSource = ctx.Dictionary;
            }
        }

        private void RemoveWord_Click(object sender, RoutedEventArgs e)
        {
            using (var ctx = new DictionaryContext())
            {
                var tempWord = (from z in ctx.Dictionary
                               where (dgDictionary.SelectedItem as Word).ID == z.ID
                               select z).FirstOrDefault();
                ctx.Dictionary.Remove(tempWord);
                ctx.SaveChanges();
                dgDictionary.ItemsSource = ctx.Dictionary;
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnEndEdit(object sender, DataGridRowEditEndingEventArgs e)
        {
            var editedWord = dgDictionary.SelectedItem as Word;
            using (var ctx = new DictionaryContext())
            {
                ctx.Dictionary.Where(z => z.ID == editedWord.ID).First().Category = editedWord.Category;
                ctx.Dictionary.Where(z => z.ID == editedWord.ID).First().Difficulty = editedWord.Difficulty;
                ctx.Dictionary.Where(z => z.ID == editedWord.ID).First().English = editedWord.English;
                ctx.Dictionary.Where(z => z.ID == editedWord.ID).First().Polish = editedWord.Polish;
                ctx.Dictionary.Where(z => z.ID == editedWord.ID).First().Picture = editedWord.Picture;
                ctx.SaveChanges();
                dgDictionary.ItemsSource = ctx.Dictionary;
            }
        }
    }
}
