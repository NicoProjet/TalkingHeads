using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using TalkingHeads;
using GUI.Views;

namespace GUI.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        string thName;
        TalkingHead th;
        public string TalkingHeadName 
        {
            get => thName;
            set
            {
                thName = value;
                var args = new PropertyChangedEventArgs(nameof(TalkingHeadName));
                PropertyChanged?.Invoke(this, args);
            } 
        }
        public Command ImportTalkingHead { get; }
        public Command StartDiscriminationGame { get; }
        public ObservableCollection<string> Text { get; set; }
        public MainPageViewModel()
        {
            TalkingHeadName = "Choose a Talking Head";
            ImportTalkingHead = new Command(() =>
            {
                TalkingHeadName = "Albert"; 
            });
            StartDiscriminationGame = new Command(async () =>
            {
                //if (th == null) return;
                if (th == null) th = new TalkingHead();
                var discriminationVM = new DiscriminationGameViewModel(th);
                var discriminationPage = new DiscriminationGame();
                discriminationPage.BindingContext = discriminationVM;
                await Application.Current.MainPage.Navigation.PushAsync(discriminationPage);
            });
        }
    }
}
