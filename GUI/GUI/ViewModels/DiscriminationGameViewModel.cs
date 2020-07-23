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
        public ImageSource Image { get; set; }
        public string Role { get; set; } 

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

        public ImageSource ImageSrc
        {
            get => Image;
            set
            {
                Image = value;
                var args = new PropertyChangedEventArgs(nameof(ImageSrc));
                PropertyChanged?.Invoke(this, args);
            }
        }
        private void ButtonsInit()
        {
            UseCamera = new Command(async () =>
            {
                var photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions() { });

                if (photo != null)
                    Image = ImageSource.FromStream(() => { return photo.GetStream(); });
            });

            SwapRole = new Command(() =>
            {
                switch (Role)
                {
                    case "Speaker":
                        TalkingHeadRole = "Guesser";
                        break;
                    case "Guesser":
                        TalkingHeadRole = "Speaker";
                        break;
                }
            });
        }

        public DiscriminationGameViewModel()
        {
            th = null;
            Role = "Guesser";
            ButtonsInit();
        }

        public DiscriminationGameViewModel(TalkingHead _th)
        {
            th = _th;
            Role = "Guesser";
            ButtonsInit();
        }
    }
}
