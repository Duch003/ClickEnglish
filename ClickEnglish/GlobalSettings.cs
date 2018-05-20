using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickEnglish
{
    public static class GlobalSettings
    {
        public static bool Sound { get; set; }
        private static int _TimeChallange;
        public static int TimeChallange 
        {
            get 
            {
                return _TimeChallange;
            }
            set 
            {
                if (TimeChallange_UpperLimt >= value)
                    _TimeChallange = value;
                else
                    _TimeChallange = TimeChallange_UpperLimt;
            }
        }
        private static int _VocabularySize;
        public static int VocabularySize 
        {
            get 
            {
                return _VocabularySize;
            }
            set 
            {
                if (VocabularySize_UpperLimit >= value)
                    _VocabularySize = value;
                else
                    _VocabularySize = VocabularySize_UpperLimit;
            }
        }
        static int TimeChallange_UpperLimt = 15;
        public static int VocabularySize_UpperLimit;
    }
}
