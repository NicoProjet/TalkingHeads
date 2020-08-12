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
using System.Linq.Expressions;
using Plugin.Media;
using Plugin.Media.Abstractions;
using GUI.Utils;

namespace GUI.ViewModels
{
    public class DiscriminationGameViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        TalkingHead th;
        public List<DiscriminationTree.Guess> ProcessingMemory;
        string photoPath = "";
        Image img = new Image();

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
        public bool IsGuesserOrCorrect { get; set; }
        public bool ShowSuccessOrFailureButtons { get; set; }

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
            get => Guess;
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
            ShowSuccessOrFailureButtonsBind = !ShowSuccessOrFailureButtons;
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

        public bool IsGuesserOrCorrectFormBind
        {
            get => IsGuesserOrCorrect;
            set
            {
                IsGuesserOrCorrect = value;
                var args = new PropertyChangedEventArgs(nameof(IsGuesserOrCorrectFormBind));
                PropertyChanged?.Invoke(this, args);
            }
        }

        public bool ShowSuccessOrFailureButtonsBind
        {
            get => ShowSuccessOrFailureButtons;
            set
            {
                ShowSuccessOrFailureButtons = value;
                var args = new PropertyChangedEventArgs(nameof(ShowSuccessOrFailureButtonsBind));
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
            if (EnterCorrectForm)
            {
                CorrectFormToggle();
            }
            if (IsGuesser)
            {
                TalkingHeadRole = "Speaker";
                IsGuesserBind = false;
                IsGuesserOrCorrectFormBind = false;
                IsSpeakerBind = true;
                EnterCorrectForm = false;
                ShowSuccessOrFailureButtonsBind = true;
            }
            else
            {
                TalkingHeadRole = "Guesser";
                IsGuesserBind = true;
                IsGuesserOrCorrectFormBind = true;
                IsSpeakerBind = false;
                EnterCorrectForm = false;
                ShowSuccessOrFailureButtonsBind = true;
            }
        }

        private string GetGuessSentence(int IDForm)
        {
            if (IDForm == -2)
            {
                CorrectFormToggle();
                return "I do not recognize the words: " + HeardText;
            }
            if (IDForm == -1 && ProcessingMemory.Count() > 0)
            {
                string caracteristics = "";
                foreach (DiscriminationTree.Guess guess in ProcessingMemory)
                {
                    caracteristics += guess.Node.Print();
                }
                return "I found no form: " + caracteristics;
            }
            return "I guessed form " + IDForm;
        }

        private void ButtonsInit()
        {
            UseCamera = new Command(async () =>
            {
                /*
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    return;
                }
                */
                StoreCameraMediaOptions options = new StoreCameraMediaOptions()
                {
                    MaxWidthHeight = 1080, // 1080 x 810
                    PhotoSize = PhotoSize.MaxWidthHeight,
                    CompressionQuality = 100, // max quality
                    Name = $"{DateTime.UtcNow}.jpg",
                };

                var photo = await CrossMedia.Current.TakePhotoAsync(options);
                //Eyes.FindForms(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), options.Name));
                //Eyes.FindForms(Path.Combine("/storage/emulated/0/Android/data/com.companyname.gui/files/Pictures/", options.Name));
                photoPath = Path.Combine("/storage/emulated/0/Android/data/com.companyname.gui/files/Pictures/", options.Name);

                if (photo != null)
                {
                    ImageSrcBind = ImageSource.FromStream(() => { return photo.GetStream(); });
                    img.Source = ImageSource.FromStream(() => { return photo.GetStream(); });
                    ImageStr = photo.GetStream();
                    ImageSize = GetImageSizeFromStream(ImageStr);
                }
            });

            SwapRole = new Command(() =>
            {
                _SwapRole();
            });

            ChooseFormBind = new Command(() =>
            {
                ResetTextValues();
                ProcessingMemory.Clear();
                //SaidTextBind = Brain.DiscriminationGameDescription(th, ImageStr, (int)ImageSize.Width, (int)ImageSize.Height, out int IDChoice, ProcessingMemory, true);
                //SaidTextBind = Brain.DiscriminationGameDescription(th, photoPath, (int)ImageSize.Width, (int)ImageSize.Height, out int IDChoice, ProcessingMemory, true);
                var test = DependencyService.Get<IImageUtils>();
                SaidTextBind = Brain.DiscriminationGameDescription(th, DependencyService.Get<IImageUtils>().Uncompress(ImageStr), (int)ImageSize.Width, (int)ImageSize.Height, out int IDChoice, ProcessingMemory, true);
                ChoiceBind = IDChoice.ToString();
            });

            MakeGuessBind = new Command(() =>
            {
                ResetTextValues();
                ProcessingMemory.Clear();
                GuessBind = GetGuessSentence(Brain.DiscriminationGameGuessID(th, ImageStr, (int)ImageSize.Width, (int)ImageSize.Height, HeardText, ProcessingMemory, true));
            });

            GuesserIsCorrectBind = new Command(() =>
            {
                if (ProcessingMemory.Count() < 1) return;
                th.UpdateScore(ProcessingMemory, true);
            });

            GuesserIsIncorrectBind = new Command(() =>
            {
                if (ProcessingMemory.Count() < 1) return;
                th.UpdateScore(ProcessingMemory, false);
                CorrectFormBind = "";
                CorrectFormToggle();
            });

            MajorityIsCorrectBind = new Command(() =>
            {
                if (ProcessingMemory.Count() < 1) return;
                th.UpdateScore(ProcessingMemory, true);
            });

            MajorityIsIncorrectBind = new Command(() =>
            {
                if (ProcessingMemory.Count() < 1) return;
                th.UpdateScore(ProcessingMemory, false);
            });

            ConfirmCorrectFormBind = new Command(() =>
            {
                int IDForm = 0;
                try
                {
                    IDForm = Int32.Parse(CorrectForm);
                }
                catch
                {
                    CorrectFormBind = "";
                    return;
                }
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

        private void ResetBooleanValues()
        {
            IsGuesserBind = true;
            IsSpeakerBind = false;
            EnterCorrectForm = false;
            IsGuesserOrCorrectFormBind = true;
            ShowSuccessOrFailureButtonsBind = true;
        }

        public DiscriminationGameViewModel()
        {
            th = null;
            TalkingHeadRole = "Guesser";
            ResetBooleanValues();
            ProcessingMemory = new List<DiscriminationTree.Guess>();
            ResetTextValues();
            ButtonsInit();
        }

        public DiscriminationGameViewModel(TalkingHead _th)
        {
            th = _th;
            TalkingHeadRole = "Guesser";
            ResetBooleanValues();
            ProcessingMemory = new List<DiscriminationTree.Guess>();
            ResetTextValues();
            ButtonsInit();
            if (Configuration.TestMode)
            {
                string defaultImage = "x8.bmp";
                ImageSrcBind = defaultImage;
                var assembly = Assembly.GetExecutingAssembly();
                string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(defaultImage));


                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                }
                ImageStr = assembly.GetManifestResourceStream(resourceName);
                ImageSize = GetImageSizeFromStream(ImageStr);
                //ShowSegmentation();
            }
        }
    }
}
