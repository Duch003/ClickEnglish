using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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

        #region Events
        private void Searcher_TextChanged(object sender, TextChangedEventArgs e)
        {
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

        private void OnEndEdit(object sender, DataGridRowEditEndingEventArgs e)
        {

        }

        private void KeyPreview(object sender, System.Windows.Input.KeyEventArgs e)
        {

        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
        }
        #endregion

        #region Methods

        #endregion

    }
}
