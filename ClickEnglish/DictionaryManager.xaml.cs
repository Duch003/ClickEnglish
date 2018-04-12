using System;
using System.Collections.Generic;
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

namespace ClickEnglish {
    /// <summary>
    /// Interaction logic for DictionaryManager.xaml
    /// </summary>
    public partial class DictionaryManager : Window {
        private DatabaseManager _manager;
        public DictionaryManager(DatabaseManager manager) {
            InitializeComponent();
            _manager = manager;
        }

        private void DownloadAndAssignData() {
            _manager.TakeDictionary(GlobalSettings.ID, out var dictionaryData);
            dgDictionary.DataContext = dictionaryData;
        }

        private void AddNewRecord(object sender, AddingNewItemEventArgs e) {

        }

        private void EditRecord_Begin(object sender, DataGridBeginningEditEventArgs e) {
            
        }

        private void EditRecord_End(object sender, DataGridCellEditEndingEventArgs e) {
            
        }
    }
}
