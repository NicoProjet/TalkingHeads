using GUI.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using TalkingHeads;
using TalkingHeads.DataStructures;
using TalkingHeads.BodyParts;
using Xamarin.Forms;
using System.Reflection;
using System.Linq;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Security.Cryptography;

namespace GUI.ViewModels
{
    public class DiscriminationGameViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        TalkingHead th;
        public List<DiscriminationTree.Guess> ProcessingMemory;

        public Command UseCamera { get; set; }
        public Command SwapRole { get; set; }
        public Command MakeGuessBind { get; set; }
        public Command ChooseFormBind { get; set; }
        public Command GuesserIsCorrectBind { get; set; }
        public Command GuesserIsIncorrectBind { get; set; }
        public Command MajorityIsCorrectBind { get; set; }
        public Command MajorityIsIncorrectBind { get; set; }
        public Command ConfirmCorrectFormBind { get; set; }
        public Command CancelCorrectFormBind { get; set; }
        public ImageSource ImageSrc { get; set; }
        public Stream ImageStr { get; set; }
        public Size ImageSize { get; set; }
        public string Role { get; set; } 
        public string HeardText { get; set; }
        public string SaidText { get; set; }
        public string Guess { get; set; }
        public string Choice { get; set; }
        public string CorrectForm { get; set; }
        public bool IsGuesser { get; set; }
        public bool IsSpeaker { get; set; }
        public bool EnterCorrectForm { get; set; }

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

        public string CorrectFormBind
        {
            get => CorrectForm;
            set
            {
                CorrectForm = value;
                var args = new PropertyChangedEventArgs(nameof(CorrectFormBind));
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

        private void CorrectFormToggle()
        {
            EnterCorrectFormBind = !EnterCorrectForm;
            IsGuesserBind = !IsGuesser;
        }
        public bool EnterCorrectFormBind
        {
            get => EnterCorrectForm;
            set
            {
                EnterCorrectForm = value;
                var args = new PropertyChangedEventArgs(nameof(EnterCorrectFormBind));
                PropertyChanged?.Invoke(this, args);
            }
        }

        private Size GetImageSizeFromStream(Stream str)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                str.CopyTo(ms);
                return DependencyService.Get<IImageManager>().GetDimensionsFrom(ms.ToArray());
            }
        }

        private void ShowSegmentation()
        {
            Size size = GetImageSizeFromStream(ImageStr);

            List<TalkingHeads.DataStructures.Form> forms = Eyes.FindForms(ImageStr, (int)size.Width, (int)size.Height);

            // Use skiaSharp
        }

        private void _SwapRole()
        {
            if (IsGuesser)
            {
                TalkingHeadRole = "Speaker";
                IsGuesserBind = false;
                IsSpeakerBind = true;
                EnterCorrectForm = false;
            }
            else
            {
                TalkingHeadRole = "Guesser";
                IsGuesserBind = true;
                IsSpeakerBind = false;
                EnterCorrectForm = false;
            }
        }

        private void ButtonsInit()
        {
            UseCamera = new Command(async () =>
            {
                var photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions() { });

                if (photo != null)
                {
                    ImageSrcBind = ImageSource.FromStream(() => { return photo.GetStream(); });
                    ImageStr = photo.GetStream();
                    ImageSize = GetImageSizeFromStream(ImageStr);
                }
            });

            SwapRole = new Command(() =>
            {
                switch (Role)
                {
                    case "Speaker":
                        _SwapRole();
                        break;
                    case "Guesser":
                        _SwapRole();
                        break;
                }
            });

            ChooseFormBind = new Command(() =>
            {
                ResetTextValues();
                ProcessingMemory.Clear();
                SaidTextBind = Brain.DiscriminationGameDescription(th, ImageStr, (int)ImageSize.Width, (int)ImageSize.Height, out int IDChoice, ProcessingMemory, true);
                ChoiceBind = IDChoice.ToString();
            });

            MakeGuessBind = new Command(() =>
            {
                ResetTextValues();
                ProcessingMemory.Clear();
                GuessBind = Brain.DiscriminationGameGuessID(th, ImageStr, (int)ImageSize.Width, (int)ImageSize.Height, HeardText, ProcessingMemory, true).ToString();
            });

            GuesserIsCorrectBind = new Command(() =>
            {
                th.UpdateScore(ProcessingMemory, true);
            });

            GuesserIsIncorrectBind = new Command(() =>
            {
                th.UpdateScore(ProcessingMemory, false);
                CorrectFormBind = "";
                CorrectFormToggle();
            });

            MajorityIsCorrectBind = new Command(() =>
            {
                th.UpdateScore(ProcessingMemory, true);
            });

            MajorityIsIncorrectBind = new Command(() =>
            {
                th.UpdateScore(ProcessingMemory, false);
            });

            ConfirmCorrectFormBind = new Command(() =>
            {
                int IDForm = Int32.Parse(CorrectForm);
                string description = "";
                foreach(DiscriminationTree.Guess ProcessingMemoryPart in ProcessingMemory)
                {
                    description += ProcessingMemoryPart.Word + Configuration.Word_Separator;
                }
                description = description.Substring(0, description.Length - 1);
                Brain.EnterCorrectForm(th, ImageStr, (int)ImageSize.Width, (int)ImageSize.Height, description, IDForm);
                CorrectFormToggle();
            });

            CancelCorrectFormBind = new Command(() =>
            {
                CorrectFormToggle();
            });
        }

        private void ResetTextValues()
        {
            ChoiceBind = "";
            GuessBind = "";
            CorrectFormBind = "";
        }

        public DiscriminationGameViewModel()
        {
            th = null;
            TalkingHeadRole = "Guesser";
            IsGuesserBind = true;
            IsSpeakerBind = false;
            EnterCorrectForm = false;
            ProcessingMemory = new List<DiscriminationTree.Guess>();
            ResetTextValues();
            ButtonsInit();
        }

        public DiscriminationGameViewModel(TalkingHead _th)
        {
            th = _th;
            TalkingHeadRole = "Guesser";
            IsGuesserBind = true;
            IsSpeakerBind = false;
            EnterCorrectForm = false;
            ProcessingMemory = new List<DiscriminationTree.Guess>();
            ResetTextValues();
            ButtonsInit();
            if (Configuration.TestMode)
            {
                ImageSrcBind = "x5.bmp";
                //byte[] data = File.ReadAllBytes("x5.bmp");
                var assembly = Assembly.GetExecutingAssembly();
                string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("x5.bmp"));

                ImageStr = assembly.GetManifestResourceStream(resourceName);
                using (StreamReader reader = new StreamReader(ImageStr))
                {
                    string result = reader.ReadToEnd();
                }
                ImageSize = GetImageSizeFromStream(ImageStr);
                //ShowSegmentation();
            }
        }
    }
}
