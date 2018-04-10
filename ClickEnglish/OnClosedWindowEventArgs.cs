using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickEnglish
{
    public class WindowEventArgs : EventArgs
    {
        public int Time { get; private set; }
        public int RndVocabularySize { get; private set; }
        public bool SoundState { get; private set; }
        private readonly int _time;
        private readonly int _rndVocabularySize;
        private readonly bool _soundState;
        
        public WindowEventArgs(int time, int rndVocabularySize, bool soundState)
        {
            _time = time;
            _rndVocabularySize = rndVocabularySize;
            _soundState = soundState;
        }
    }
}
