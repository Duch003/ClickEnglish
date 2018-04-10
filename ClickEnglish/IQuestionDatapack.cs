using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickEnglish
{
    internal interface IQuestionDataPack
    {
        string WordEng { get; set; } //Angielskie słowo
        string WordPl { get; set; } //Polskie słowo
        Category Category { get; set; } //Kategoria pytania
        ushort Repeats { get; set; } //Ilość powtórzeń
        string ImgSrc { get; set; } //Ścieżka do zdjęcia 
        float Percentage { get; set; } //Stopień trudności
    }
}
