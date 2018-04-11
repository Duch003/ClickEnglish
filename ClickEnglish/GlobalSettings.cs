﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickEnglish
{
    public static class GlobalSettings
    {
        //State of sounds
        //False - disabled
        private static bool _soundState;          
        public static bool SoundState
        {
            get
            {
                return _soundState;
            }
            set
            {
                _soundState = value;
            }
        }
        //Time for time challange
        //Min: 1 min., max 15 min.
        private static int _time;                 
        public static int Time
        {
            get
            {
                return _time;
            }
            set
            {
                if (value >= 1 && value <= 15)
                    _time = value;
                else
                    _time = 5;
            }
        }
        //Number of words in challange 
        //Min: 5, max: 100
        private static int _rndVocabularySize;    
        public static int RandomVocabulaySize
        {
            get
            {
                return _rndVocabularySize;
            }
            set
            {
                if (value >= 5 && value <= 100)
                    _rndVocabularySize = value;
                else
                    _rndVocabularySize = 15;
            }
        }
        //Id of logged user
        private static int _myId;                 
        public static int ID
        {
            get
            {
                return _myId;
            }
            set
            {
                _myId = value;
            }
        }
    }
}
