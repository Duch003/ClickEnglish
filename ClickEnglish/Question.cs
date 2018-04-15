using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickEnglish
{
    public class Question
    {
        public int ID { get; set; }
        public string WordEng { get; set; }
        public string WordPl { get; set; }
        public Category Cat { get; set; }
        public int Repeats { get; set; }
        public double Percentage { get; set; }
        public string ImgSrc { get; set; }

        public Question(int id, string eng, string pl, Category cat, double percent, string img)
        {
            ID = id;
            Repeats = 3;
            WordPl = pl;
            WordEng = eng;
            Cat = cat;
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


}
