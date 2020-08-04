using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using TalkingHeads;
using TalkingHeads.BodyParts;
using GUI.Views;
using System.Linq;
using System.Reflection;
using System.IO;

namespace GUI.ViewModels
{
    public class TalkingHeadPickerItem
    {
        public int Key { get; set; }
        public string Name { get; set; }
        public TalkingHead _TalkingHead { get; set; }
    }
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        string thName;
        private TalkingHead th;
        private TalkingHead SelectedTh;
        private bool CanStartGame;
        private bool CanLoadTalkingHead;
        public List<TalkingHeadPickerItem> TalkingHeadsCollection { get; set; }


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

        private TalkingHeadPickerItem SelectedTalkingHead { get; set; }
        public TalkingHeadPickerItem SelectedTalkingHeadBinding
        {
            get => SelectedTalkingHead;
            set
            {
                if (SelectedTalkingHead != value)
                {
                    SelectedTalkingHead = value;
                    SelectedTh = value._TalkingHead;
                    CanLoadTalkingHeadBinding = true;
                    CanStartGameBinding = true;
                }
            }
        }

        private void InitTalkingHeadCollection()
        {
            TalkingHeadsCollection = new List<TalkingHeadPickerItem>()
            {
                new TalkingHeadPickerItem { Key = 1, Name = "Albert", _TalkingHead = new TalkingHead { Name = "Albert" } },
                new TalkingHeadPickerItem { Key = 2, Name = "Robert", _TalkingHead = new TalkingHead { Name = "Robert" } },
                new TalkingHeadPickerItem { Key = 3, Name = "Borderlands", _TalkingHead = new TalkingHead { Name = "Borderlands" } },
                new TalkingHeadPickerItem { Key = 4, Name = "Edgar", _TalkingHead = new TalkingHead { Name = "Edgar" } },
                new TalkingHeadPickerItem { Key = 5, Name = "Siren", _TalkingHead = new TalkingHead { Name = "Siren" } },
                new TalkingHeadPickerItem { Key = 6, Name = "Edna", _TalkingHead = new TalkingHead { Name = "Edna" } },
                new TalkingHeadPickerItem { Key = 7, Name = "Tess", _TalkingHead = new TalkingHead { Name = "Tess" } },
                new TalkingHeadPickerItem { Key = 8, Name = "Zero", _TalkingHead = new TalkingHead { Name = "Zero" } },
            };
            TalkingHeadsCollection = TalkingHeadsCollection.OrderBy(x => x.Name).ToList();
        }

        public bool CanStartGameBinding
        {
            get => CanStartGame;
            set
            {
                CanStartGame = value;
                var args = new PropertyChangedEventArgs(nameof(CanStartGameBinding));
                PropertyChanged?.Invoke(this, args);
            }
        }

        public bool CanLoadTalkingHeadBinding
        {
            get => CanLoadTalkingHead;
            set
            {
                CanLoadTalkingHead = value;
                var args = new PropertyChangedEventArgs(nameof(CanLoadTalkingHeadBinding));
                PropertyChanged?.Invoke(this, args);
            }
        }

        public Command ImportTalkingHead { get; }
        public Command StartDiscriminationGame { get; }
        public Command SaveTalkingHeadBinding { get; }
        public MainPageViewModel()
        {
            InitTalkingHeadCollection();
            TalkingHeadName = "Choose a Talking Head";
            CanStartGameBinding = false;
            CanLoadTalkingHeadBinding = false;
            ImportTalkingHead = new Command(() =>
            {
                th = SelectedTh;
                TalkingHeadName = th.Name;
                CanStartGameBinding = true;
            });
            StartDiscriminationGame = new Command(async () =>
            {
                if (th == null) return;
                var discriminationVM = new DiscriminationGameViewModel(th);
                var discriminationPage = new DiscriminationGame();
                discriminationPage.BindingContext = discriminationVM;
                await Application.Current.MainPage.Navigation.PushAsync(discriminationPage);
            });
            SaveTalkingHeadBinding = new Command(() =>
            {

            });
        }
    }
}
