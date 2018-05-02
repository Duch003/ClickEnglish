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
        private DatabaseManager _manager;
        public ObservableCollection<Question> Data { get; set; } 
        private List<Category> CategoryList;                     
        public ObservableCollection<string> CategoryStringList { get; set; }

        string temp;

        public DictionaryManager(DatabaseManager manager)
        {
            InitializeComponent();
            _manager = manager;
            var result = manager.TakeDictionary(GlobalSettings.ID, out var dictionaryData);
            if(result) {
                LoadDictionary(dictionaryData);
            }
            result = manager.TakeCategories(GlobalSettings.ID, out var categoryData);
            if(result)
                LoadCategories(categoryData);
        }

        #region Methods
        //Load categories used by user
        private bool LoadCategories(DataSet raw)
        {
            if(raw is null)
                throw new Exception("Method: LoadCategories. Raw DataSet is null.");
            if(raw.Tables[0].Rows.Count == 0)
                return false;

            CategoryList = new List<Category>();
            CategoryStringList = new ObservableCollection<string>();

            for(int i = 0; i < raw.Tables[0].Rows.Count; i++) {
                var tempId = Convert.ToInt32(raw.Tables[0].Rows[i][0]);
                var tempName = raw.Tables[0].Rows[i][1].ToString();

                CategoryList.Add(new Category(tempId, tempName));
                CategoryStringList.Add(tempName);
            }
            dgcbCategory.ItemsSource = CategoryStringList;
            return true;
        }

        //Parse DataSet to Collection of Questions
        private bool LoadDictionary(DataSet raw)
        {
            if(raw == null)
                throw new Exception("Method: LoadDictionary. Raw DataSet is null.");
            if(raw.Tables[0].Rows.Count == 0)
                return false;

            Data = new ObservableCollection<Question>();

            for(int i = 0; i < raw.Tables[0].Rows.Count; i++) {
                var tempId =    Convert.ToInt32( raw.Tables[0].Rows[i][0]);
                var tempEng =                    raw.Tables[0].Rows[i][1].ToString();
                var tempPl =                     raw.Tables[0].Rows[i][2].ToString();
                var tempPerc =  Convert.ToDouble(raw.Tables[0].Rows[i][3]);
                var tempImg =                    raw.Tables[0].Rows[i][4].ToString();
                var tempCatId = Convert.ToInt32( raw.Tables[0].Rows[i][6]);
                var tempCatName =                raw.Tables[0].Rows[i][5].ToString();

                var tempCat = new Category(tempCatId, tempCatName);
                var tempQuestion = new Question(tempId, tempEng, tempPl, tempCat, tempPerc, tempImg);
                Data.Add(tempQuestion);
            }
            dgDictionary.ItemsSource = Data;
            return true;
        }
        #endregion

        #region Events
        private void EditRecord_End(object sender, DataGridCellEditEndingEventArgs e)
        {
            var newRecord = e.Row.Item as Question;
            if(e.EditingElement is TextBox) {
                var change = (e.EditingElement as TextBox).Text;
                var column = (e.Column.Header);
                switch(column) {
                    case "Polish word":
                        newRecord.WordPl = change;
                        break;
                    case "English word":
                        newRecord.WordEng = change;
                        break;
                    case "Difficulty":
                        newRecord.Percentage = Convert.ToDouble(change.Replace("%","")) / 100;
                        break;
                    case "Picture":
                        newRecord.ImgSrc = temp;
                        temp = "";
                        break;
                }
            } else {
                var change = (e.EditingElement as ComboBox).SelectedValue.ToString();
                var tempCategory = from z in CategoryList
                                   where z.Name == change
                                   select z;
                newRecord.Cat = tempCategory == null ? CategoryList.First() : tempCategory.First();
            }
            _manager.UpdateRecord(newRecord);
            var result = _manager.TakeDictionary(GlobalSettings.ID, out var dictionaryData);
            if(result) {
                LoadDictionary(dictionaryData);
            }
        }

        private void Searcher_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(_manager is null)
                return;
            //If empty, whitespace, null, TempData filled with data and IS NOT already flled with whole data
            if((string.IsNullOrEmpty(txtSearcher.Text) || string.IsNullOrWhiteSpace(txtSearcher.Text))) {
                var otherResult = _manager.TakeDictionary(GlobalSettings.ID, out var dictionaryData);
                if(otherResult) {
                    LoadDictionary(dictionaryData);
                }
                return;
            }
            var result = _manager.TakeDictionary_WordCondition(GlobalSettings.ID, out var temp, txtSearcher.Text);
            if(result) {
                LoadDictionary(temp);
            }
        }

        private void AddWord_Click(object sender, RoutedEventArgs e)
        {
            var tempCat = CategoryList.First();
            var tempQues = new Question(0, "eng", "pl", tempCat, 0, "none");
            _manager.AddNewRecord(GlobalSettings.ID, tempQues);
            var result = _manager.TakeDictionary(GlobalSettings.ID, out var dictionaryData);
            if(result) { 
                LoadDictionary(dictionaryData);
            }
        }

        private void RemoveWord_Click(object sender, RoutedEventArgs e)
        {
            _manager.RemoveRecord((dgDictionary.SelectedItem as Question).ID);
            var result = _manager.TakeDictionary(GlobalSettings.ID, out var dictionaryData);
            if(result) {
                LoadDictionary(dictionaryData);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion

        private void EditBegin_SpecialPicturesHandler(object sender, DataGridBeginningEditEventArgs e) {
            if(e.Column.Header.ToString() != "Picture") {
                return;
            }
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".bmp"; // Default file extension
            dlg.Filter = "JPEG (.jpeg)|*.jpeg;*.jpg" +
                "|TIFF (.tiff)|*.tiff" +
                "|BMP (.bmp)|*.bmp" +
                "|GIF (.gif)|*.gif" +
                "|PNG (.png)|*.png" +
                "|JPG (.jpg)|*.jpg"; // Filter files by extension
            dlg.Multiselect = false;

            if(dlg.ShowDialog() == true) {
                var sourcePath = dlg.FileName;
                var fileName = dlg.SafeFileName;
                var targetPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

                var combinedSource = System.IO.Path.Combine(sourcePath);
                var combinedTarget = System.IO.Path.Combine(targetPath, fileName);

                if(!System.IO.Directory.Exists(targetPath))
                    System.IO.Directory.CreateDirectory(targetPath);

                System.IO.File.Copy(combinedSource, combinedTarget, true);
                temp = combinedTarget;
            }
        }
    }
}
