using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickEnglish
{
    internal class Question
    {
        private readonly uint _id;
        private readonly string _wordEng;
        private readonly string _wordPl;
        private readonly Category _category;
        private readonly ushort _repeats;
        private readonly float _percentage;
        private readonly string _imgSrc;

        public Question(IQuestionDataPack dataPack)
        {
            _repeats = dataPack.Repeats;
            _wordPl = dataPack.WordPl;
            _wordEng = dataPack.WordEng;
            _category = dataPack.Category;
            _percentage = dataPack.Percentage;
            _imgSrc = dataPack.ImgSrc;
        }

        public uint GetId()
        {
            return _id;
        }

        public string GetWordEng()
        {
            return _wordEng;
        }

        public string GetWordPl()
        {
            return _wordPl;
        }

        public Category GetCategory()
        {
            return _category;
        }

        public ushort GetRepeats()
        {
            return _repeats;
        }

        public float GetPercentage()
        {
            return _percentage;
        }

        public string GetImageSource()
        {
            return _imgSrc;
        }
    }
}
