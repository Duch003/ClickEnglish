using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickEnglish
{
    public static class GlobalSettings
    {
        private static bool _soundState;          //State of sounds
        private static int _time;                 //Time for time challange [ms]
        private static int _rndVocabularySize;    //Number of words in challange
        private static int _myId;                 //Id of logged user

        public static bool GetSoundState()
        {
            return _soundState;
        }

        public static int GetTime()
        {
            return _time;
        }

        public static int GetRandmVocabularySize()
        {
            return _rndVocabularySize;
        }

        public static int GetId()
        {
            return _myId;
        }

    }
}
