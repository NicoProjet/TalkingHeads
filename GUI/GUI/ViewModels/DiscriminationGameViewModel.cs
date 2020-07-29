using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using TalkingHeads;
using Xamarin.Forms;

namespace GUI.ViewModels
{
    public class DiscriminationGameViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        TalkingHead th;

        public Command UseCamera { get; set; }
        public Command SwapRole { get; set; }
        public Command MakeGuessBind { get; set; }
        public Command ChooseFormBind { get; set; }
        public Command GuesserIsCorrectBind { get; set; }
        public Command GuesserIsIncorrectBind { get; set; }
        public Command MajorityIsCorrectBind { get; set; }
        public Command MajorityIsIncorrectBind { get; set; }
        public ImageSource ImageSrc { get; set; }
        public string Role { get; set; } 
        public string HeardText { get; set; }
        public string SaidText { get; set; }
        public string Guess { get; set; }
        public string Choice { get; set; }
        public bool IsGuesser { get; set; }
        public bool IsSpeaker { get; set; }

        public string TalkingHeadRole
        {
            get => "This Talking Head is a " + Role;
            set
            {
                Role = value;
                var args = new PropertyChangedEventArgs(nameof(TalkingHeadRole));
                PropertyChanged?.Invoke(this, args);
            }
        }

        public string HeardTextBind
        {
            get => HeardText;
            set
            {
                HeardText = value;
                var args = new PropertyChangedEventArgs(nameof(HeardTextBind));
                PropertyChanged?.Invoke(this, args);
            }
        }

        public string SaidTextBind
        {
            get => SaidText;
            set
            {
                SaidText = value;
                var args = new PropertyChangedEventArgs(nameof(SaidTextBind));
                PropertyChanged?.Invoke(this, args);
            }
        }

        public string GuessBind
        {
            get => (Guess != "" ? "I guessed form " : "") + Guess;
            set
            {
                Guess = value;
                var args = new PropertyChangedEventArgs(nameof(GuessBind));
                PropertyChanged?.Invoke(this, args);
            }
        }

        public string ChoiceBind
        {
            get => (Choice != "" ? "I chose the form " : "") + Choice;
            set
            {
                Choice = value;
                var args = new PropertyChangedEventArgs(nameof(ChoiceBind));
                PropertyChanged?.Invoke(this, args);
            }
        }

        public ImageSource ImageSrcBind
        {
            get => ImageSrc;
            set
            {
                ImageSrc = value;
                var args = new PropertyChangedEventArgs(nameof(ImageSrcBind));
                PropertyChanged?.Invoke(this, args);
            }
        }

        public bool IsGuesserBind
        {
            get => IsGuesser;
            set
            {
                IsGuesser = value;
                IsSpeakerBind = !value;
                var args = new PropertyChangedEventArgs(nameof(IsGuesserBind));
                PropertyChanged?.Invoke(this, args);
            }
        }

        public bool IsSpeakerBind
        {
            get => IsSpeaker;
            set
            {
                IsSpeaker = value;
                var args = new PropertyChangedEventArgs(nameof(IsSpeakerBind));
                PropertyChanged?.Invoke(this, args);
            }
        }

        private void ButtonsInit()
        {
            UseCamera = new Command(async () =>
            {
                var photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions() { });

                if (photo != null)
                    ImageSrcBind = ImageSource.FromStream(() => { return photo.GetStream(); });
            });

            SwapRole = new Command(() =>
            {
                switch (Role)
                {
                    case "Speaker":
                        TalkingHeadRole = "Guesser";
                        IsGuesserBind = true;
                        break;
                    case "Guesser":
                        TalkingHeadRole = "Speaker";
                        IsGuesserBind = false;
                        break;
                }
            });

            ChooseFormBind = new Command(() =>
            {
                // To do
            });

            MakeGuessBind = new Command(() =>
            {
                // To do
            });

            GuesserIsCorrectBind = new Command(() =>
            {
                // To do
            });

            GuesserIsIncorrectBind = new Command(() =>
            {
                // To do
            });

            MajorityIsCorrectBind = new Command(() =>
            {
                // To do
            });
            MajorityIsIncorrectBind = new Command(() =>
            {
                // To do
            });
        }

        public DiscriminationGameViewModel()
        {
            th = null;
            TalkingHeadRole = "Guesser";
            ChoiceBind = "";
            GuessBind = "";
            IsGuesserBind = true;
            ButtonsInit();
        }

        public DiscriminationGameViewModel(TalkingHead _th)
        {
            th = _th;
            TalkingHeadRole = "Guesser";
            ChoiceBind = "";
            GuessBind = "";
            IsGuesserBind = true;
            ButtonsInit();

            if (Configuration.TestMode)
            {
                ImageSrcBind = "x5.bmp";
            }
        }
    }
}
