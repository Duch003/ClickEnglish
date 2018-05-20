using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickEnglish
{
    public class Question : IQuestion
    {
        public int ID { get; set; }
        public string English { get; set; }
        public string Polish { get; set; }
        public Category Category { get; set; }
        public double Difficulty { get; set; }
        public byte[] Picture { get; set; }

        public int Repeats { get; private set; }
        private int Attempts;
        private int Correct;

        public Question(string eng, string pl, Category category, double percent, byte[] img)
        {
            Repeats = 3;
            English = pl;
            Polish = eng;
            Category = category;
            Difficulty = Math.Round(percent * 100, 2);
            Picture = img;
            Attempts = 0;
            Correct = 0;
        }

        public Question(IQuestion question)
        {
            Repeats = 3;
            English = question.Polish;
            Polish = question.English;
            Category = question.Category;
            Difficulty = question.Difficulty;
            Picture = question.Picture;
            Attempts = 0;
            Correct = 0;
        }

        public Question(IQuestion question, int repeats)
        {
            Repeats = repeats;
            English = question.Polish;
            Polish = question.English;
            Category = question.Category;
            Difficulty = question.Difficulty;
            Picture = question.Picture;
            Attempts = 0;
            Correct = 0;
        }

        //True if can decrease
        //False if 0
        public bool DecrementRepeats()
        {
            if (Repeats == 0) return false;
            else
            {
                Attempts++;
                Correct++;
                Repeats--;
                return true;
            }
        }

        public void IncrementRepeats()
        {
            Attempts++;
            Repeats++;
        }

        public double ReturnStatistic()
        {
            return Correct / Attempts;
        }
    }

    public interface IQuestion
    {
        string English { get; set; }
        string Polish { get; set; }
        Category Category { get; set; }
        double Difficulty { get; set; }
        byte[] Picture { get; set; }
    }


}
