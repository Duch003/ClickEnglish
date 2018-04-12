using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickEnglish
{
    public class Question
    {
        public int ID { get; private set; }
        public string WordEng { get; private set; }
        public string WordPl { get; private set; }
        public Category Category { get; private set; }
        public int Repeats { get; private set; }
        public double Percentage { get; private set; }
        public string ImgSrc { get; private set; }

        public Question(int id, string eng, string pl, Category cat, double percent, string img)
        {
            Repeats = 3;
            WordPl = pl;
            WordEng = eng;
            Category = cat;
            Percentage = Math.Round(percent * 100, 2);
            ImgSrc = img;
        }

        //True if can decrease
        //False if 0
        public bool DecrementRepeats()
        {
            if (Repeats == 0) return false;
            else
            {
                Repeats--;
                return true;
            }
        }

        public void IncrementRepeats()
        {
            Repeats++;
        }
    }

    public class Category
    {
        public int Id { get; private set; }
        public string Name { get; private set; }

        public Category(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
