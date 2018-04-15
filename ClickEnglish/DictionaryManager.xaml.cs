﻿using System;
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

        public ObservableCollection<Question> Data { get; set; } //Actual container for data, filled always with data with actual text criteria

        private List<Category> CategoryList;                     //Actual list of categories
        public ObservableCollection<string> CategoryStringList { get; set; } //Simplified list of categories, cause of Bindings in DG doesnt work normally
        
        private bool _needRestoration = false;

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
                        //TODO Gdzie zapisywać obrazki?
                        newRecord.ImgSrc = change;
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
    }
}
