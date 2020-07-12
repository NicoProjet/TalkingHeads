using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using TalkingHeads;

namespace GUI.ViewModels
{
    class DiscriminationGameViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        TalkingHead th;

        public DiscriminationGameViewModel(TalkingHead _th)
        {
            th = _th;
        }
    }
}
