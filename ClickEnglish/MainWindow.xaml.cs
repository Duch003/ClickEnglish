using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Linq;
using Microsoft.Win32;
using System.Data.Entity;
using System.Windows.Data;
using System.Globalization;

namespace ClickEnglish
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<int, string> CategoriesDictionary;

        public MainWindow()
        {
            //Initialization
            InitializeComponent();

            //Gathering information
            using (var context = new DictionaryContext())
            {
                //Get Categories
                var categories = from z in context.Categories
                                 select z;

                //Save as Dictionary
                CategoriesDictionary = new Dictionary<int, string>();
                foreach(var z in categories)
                {
                    CategoriesDictionary.Add(z.ID, z.Name);
                }

                //Count rows in Settings
                var count = (from z in context.UserSettings
                             select z).Count();

                //If 0 -> first app run
                if(count == 0)
                {
                    var setting = new UserSettings()
                    {
                        ID = 0,
                        Sound = true,
                        TimeChallange = 15,
                        VocabularySize = 0
                    };
                    context.UserSettings.Add(setting);
                    context.SaveChanges();
                    GlobalSettings.Sound = setting.Sound;
                    GlobalSettings.TimeChallange = setting.TimeChallange;
                    GlobalSettings.VocabularySize = setting.VocabularySize;
                    GlobalSettings.VocabularySize_UpperLimit = (from z in context.Dictionary
                                                                select z).Count();
                }
                //If 1 or more -> load data, first record
                else
                {
                    var setting = (from z in context.UserSettings
                                   select z).First();

                    GlobalSettings.Sound = setting.Sound;
                    GlobalSettings.TimeChallange = setting.TimeChallange;
                    GlobalSettings.VocabularySize = setting.VocabularySize;
                    GlobalSettings.VocabularySize_UpperLimit = (from z in context.Dictionary
                                                                select z).Count();

                }
            }
            lbCategories.DataContext = CategoriesDictionary.Values;
        }

        #region Methods
        void TimeChallange()
        {
            this.IsEnabled = false;
            Dictionary<int, Question> pack = new Dictionary<int, Question>();
            using (var ctx = new DictionaryContext())
            {
                var questionSet = from z in ctx.Dictionary
                                  select z;

                int i = 0;
                foreach(var z in questionSet)
                {
                    var question = new Question(z, 1);
                    pack.Add(i, question);
                }
            }
            MainGameBoard game = new MainGameBoard(pack, false);
            game.ShowDialog();
            this.IsEnabled = true;
        }

        void WholeDictionary()
        {
            this.IsEnabled = false;
            Dictionary<int, Question> pack = new Dictionary<int, Question>();
            using (var ctx = new DictionaryContext())
            {
                var questionSet = from z in ctx.Dictionary
                                  select z;

                int i = 0;
                foreach (var z in questionSet)
                {
                    var question = new Question(z, 3);
                    pack.Add(i, question);
                }
            }
            MainGameBoard game = new MainGameBoard(pack, true);
            game.ShowDialog();
            this.IsEnabled = true;
        }

        void ParticularCategory()
        {
            this.IsEnabled = false;
            Dictionary<int, Question> pack = new Dictionary<int, Question>();
            using (var ctx = new DictionaryContext())
            {
                var questionSet = (from z in ctx.Categories
                                  select z.Words).First();

                int i = 0;
                foreach (var z in questionSet)
                {
                    var question = new Question(z, 3);
                    pack.Add(i, question);
                }
            }
            MainGameBoard game = new MainGameBoard(pack, true);
            game.ShowDialog();
            this.IsEnabled = true;
        }

        void RandomVocabSet()
        {
            this.IsEnabled = false;
            Dictionary<int, Question> pack = new Dictionary<int, Question>();
            using (var ctx = new DictionaryContext())
            {
                //Take questions
                var questionRaw = from z in ctx.Dictionary
                                  orderby z.Difficulty ascending
                                  select z;
                //Count sum of difficulty
                var difficultySum = (from z in questionRaw
                                     select z.Difficulty).Sum();
                //Count number of questions
                var wordsCount = (from z in questionRaw
                                  select z).Count();
                //Calculate average
                var average = difficultySum / wordsCount;
                //Take words with difficulty below average
                var questionsBelowAverage = from z in questionRaw
                                    where z.Difficulty <= average
                                    select z;
                //Count words with high difficulty
                var count = (from z in questionsBelowAverage
                             select z).Count();

                IQueryable<Word> questionSet;
                //If more than user set
                if (count > GlobalSettings.VocabularySize)
                    questionSet = (from z in questionsBelowAverage
                                   orderby z.Difficulty ascending
                                   select z).Take(GlobalSettings.VocabularySize);
                //Rarely, when all difficulties are equals
                else if (count == 0)
                    questionSet = questionRaw;
                //If less than 50 -> take all words
                else
                    questionSet = from z in questionsBelowAverage
                                  where z.Difficulty < average
                                  select z;
                
                int i = 0;
                foreach (var z in questionSet)
                {
                    var question = new Question(z, 3);
                    pack.Add(i, question);
                }
            }
            MainGameBoard game = new MainGameBoard(pack, true);
            game.ShowDialog();
            this.IsEnabled = true;
        }

        void Settings()
        {
            this.IsEnabled = false;
            var temp = new Settings();
            temp.ShowDialog();
            this.IsEnabled = true;
        }

        void EditDictionary()
        {
            this.IsEnabled = false;
            var temp = new DictionaryManager();
            temp.ShowDialog();
            this.IsEnabled = true;
        }

        void EditCategories()
        {
            this.IsEnabled = false;
            var temp = new CategoryManager();
            try
            {
                temp.ShowDialog();
            }
            catch(Exception e)
            {
                var error = string.Format(e.Message + "\n\n" + e.Source + "\n\n" + e.StackTrace);
                MessageBox.Show(error, "ERROR");
            }
            
            this.IsEnabled = true;
        }

        void About()
        {
            this.IsEnabled = false;
            var temp = new About();
            temp.ShowDialog();
            this.IsEnabled = true;
        }
        #endregion

        #region Events
        private void TimeChallange_Click(object sender, RoutedEventArgs e) => TimeChallange();

        private void WholeDictionary_Click(object sender, RoutedEventArgs e) => WholeDictionary();

        private void EditDictionary_Click(object sender, RoutedEventArgs e) => EditDictionary();

        private void Settings_Click(object sender, RoutedEventArgs e) => Settings();

        private void EditCategories_Click(object sender, RoutedEventArgs e) => EditCategories();

        private void About_Click(object sender, RoutedEventArgs e) => About();

        private void ParticularCategory_Click(object sender, RoutedEventArgs e) => ParticularCategory();

        private void RandomVocabSet_Click(object sender, RoutedEventArgs e) => RandomVocabSet();
        #endregion

        public class NullImageConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value == null)
                    return DependencyProperty.UnsetValue;
                return value;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                // According to https://msdn.microsoft.com/en-us/library/system.windows.data.ivalueconverter.convertback(v=vs.110).aspx#Anchor_1
                // (kudos Scott Chamberlain), if you do not support a conversion 
                // back you should return a Binding.DoNothing or a 
                // DependencyProperty.UnsetValue
                return Binding.DoNothing;
                // Original code:
                // throw new NotImplementedException();
            }
        }
    }
}

