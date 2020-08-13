﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkingHeads.DataStructures;
using Xam = Xamarin.Forms;

namespace TalkingHeads.BodyParts
{
    public static class Brain
    {
        public struct SensoryScalingBounds
        {
            public double MinA { get; set; }
            public double MaxA { get; set; }
            public double MinRGB { get; set; }
            public double MaxRGB { get; set; }
            public double MinFormWidth { get; set; }
            public double MaxFormWidth { get; set; }
            public double MinFormHeight { get; set; }
            public double MaxFormHeight { get; set; }
            public double MinXpos { get; set; }
            public double MaxXpos { get; set; }
            public double MinYpos { get; set; }
            public double MaxYpos { get; set; }
        }
        public struct Saliencies
        {
            public double Alpha { get; set; }
            public double Red { get; set; }
            public double Green { get; set; }
            public double Blue { get; set; }
            public double Xpos { get; set; }
            public double Ypos { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
        }

        public struct ContextScalingBounds
        {
            public double MinAlpha { get; set; }
            public double MaxAlpha { get; set; }
            public double MinRed { get; set; }
            public double MaxRed { get; set; }
            public double MinGreen { get; set; }
            public double MaxGreen { get; set; }
            public double MinBlue { get; set; }
            public double MaxBlue { get; set; }
            public double MinFormWidth { get; set; }
            public double MaxFormWidth { get; set; }
            public double MinFormHeight { get; set; }
            public double MaxFormHeight { get; set; }
            public double MinXpos { get; set; }
            public double MaxXpos { get; set; }
            public double MinYpos { get; set; }
            public double MaxYpos { get; set; }
        }

        public static SensoryScalingBounds _SensoryScalingBounds;
        public static Saliencies SaliencyValues;
        public static ContextScalingBounds _ContextScalingBounds;

        private static void SensoryScalingInit(Bitmap bmp)
        {
            SensoryScalingInit(bmp.Width, bmp.Height);
        }

        private static void SensoryScalingInit(int width, int height)
        {
            _SensoryScalingBounds.MinA = Configuration.GrayScale ? Configuration.GrayScaleMinAlpha : 255;
            _SensoryScalingBounds.MaxA = 255;
            _SensoryScalingBounds.MinRGB = 0;
            _SensoryScalingBounds.MaxRGB = Configuration.GrayScale ? 0 : 255;
            _SensoryScalingBounds.MinFormWidth = 0;
            _SensoryScalingBounds.MaxFormWidth = width / Configuration.MaxFormSizeDivide;
            _SensoryScalingBounds.MinFormHeight = 0;
            _SensoryScalingBounds.MaxFormHeight = height / Configuration.MaxFormSizeDivide;
            _SensoryScalingBounds.MinXpos = 0;
            _SensoryScalingBounds.MaxXpos = width;
            _SensoryScalingBounds.MinYpos = 0;
            _SensoryScalingBounds.MaxYpos = height;
        }

        private static void ComputeSensoryScaling(Bitmap bmp, List<Form> forms)
        {
            SensoryScalingInit(bmp);
            foreach (Form form in forms)
            {
                form.ComputeSensoryScaledValues(_SensoryScalingBounds);
            }
        }

        private static void ComputeSensoryScaling(int width, int height, List<Form> forms)
        {
            SensoryScalingInit(width, height);
            foreach (Form form in forms)
            {
                form.ComputeSensoryScaledValues(_SensoryScalingBounds);
            }
        }

        private static void ResetSaliencyValues()
        {
            SaliencyValues.Alpha = 1;
            SaliencyValues.Red = 1;
            SaliencyValues.Green = 1;
            SaliencyValues.Blue = 1;
            SaliencyValues.Xpos = 1;
            SaliencyValues.Ypos = 1;
            SaliencyValues.Width = 1;
            SaliencyValues.Height = 1;
        }

        private static void ComputeSaliencyAlpha(Form a, Form b)
        {
            double distance = Math.Abs(a.SensoryScaledValues.Alpha - b.SensoryScaledValues.Alpha);
            if (distance < SaliencyValues.Alpha)
            {
                SaliencyValues.Alpha = distance;
            }
        }

        private static void ComputeSaliencyRed(Form a, Form b)
        {
            double distance = Math.Abs(a.SensoryScaledValues.Red - b.SensoryScaledValues.Red);
            if (distance < SaliencyValues.Red)
            {
                SaliencyValues.Red = distance;
            }
        }

        private static void ComputeSaliencyGreen(Form a, Form b)
        {
            double distance = Math.Abs(a.SensoryScaledValues.Green - b.SensoryScaledValues.Green);
            if (distance < SaliencyValues.Green)
            {
                SaliencyValues.Green = distance;
            }
        }

        private static void ComputeSaliencyBlue(Form a, Form b)
        {
            double distance = Math.Abs(a.SensoryScaledValues.Blue - b.SensoryScaledValues.Blue);
            if (distance < SaliencyValues.Blue)
            {
                SaliencyValues.Blue = distance;
            }
        }

        private static void ComputeSaliencyXpos(Form a, Form b)
        {
            double distance = Math.Abs(a.SensoryScaledValues.Xpos - b.SensoryScaledValues.Xpos);
            if (distance < SaliencyValues.Xpos)
            {
                SaliencyValues.Xpos = distance;
            }
        }

        private static void ComputeSaliencyYpos(Form a, Form b)
        {
            double distance = Math.Abs(a.SensoryScaledValues.Ypos - b.SensoryScaledValues.Ypos);
            if (distance < SaliencyValues.Ypos)
            {
                SaliencyValues.Ypos = distance;
            }
        }

        private static void ComputeSaliencyWidth(Form a, Form b)
        {
            double distance = Math.Abs(a.SensoryScaledValues.Width - b.SensoryScaledValues.Width);
            if (distance < SaliencyValues.Width)
            {
                SaliencyValues.Width = distance;
            }
        }

        private static void ComputeSaliencyHeight(Form a, Form b)
        {
            double distance = Math.Abs(a.SensoryScaledValues.Height - b.SensoryScaledValues.Height);
            if (distance < SaliencyValues.Height)
            {
                SaliencyValues.Height = distance;
            }
        }

        private static void ComputeSaliencyValues(Form a, Form b)
        {
            ComputeSaliencyAlpha(a, b);
            ComputeSaliencyRed(a, b);
            ComputeSaliencyGreen(a, b);
            ComputeSaliencyBlue(a, b);
            ComputeSaliencyXpos(a, b);
            ComputeSaliencyYpos(a, b);
            ComputeSaliencyWidth(a, b);
            ComputeSaliencyHeight(a, b);
        }

        private static void ComputeSaliencies(List<Form> forms)
        {
            ResetSaliencyValues();
            for (int i = 0; i < forms.Count(); i++)
            {
                for (int j = i + 1; j < forms.Count(); j++)
                {
                    ComputeSaliencyValues(forms[i], forms[j]);
                }
            }
        }

        private static void ResetContextScalingBounds()
        {
            _ContextScalingBounds.MinAlpha = 1;
            _ContextScalingBounds.MaxAlpha = 0;

            _ContextScalingBounds.MinRed = 1;
            _ContextScalingBounds.MaxRed = 0;

            _ContextScalingBounds.MinGreen = 1;
            _ContextScalingBounds.MaxGreen = 0;

            _ContextScalingBounds.MinBlue = 1;
            _ContextScalingBounds.MaxBlue = 0;

            _ContextScalingBounds.MinXpos = 1;
            _ContextScalingBounds.MaxXpos = 0;

            _ContextScalingBounds.MinYpos = 1;
            _ContextScalingBounds.MaxYpos = 0;

            _ContextScalingBounds.MinFormWidth = 1;
            _ContextScalingBounds.MaxFormWidth = 0;

            _ContextScalingBounds.MinFormHeight = 1;
            _ContextScalingBounds.MaxFormHeight = 0;
        }

        private static void ContextScalingInit(List<Form> forms)
        {
            ResetContextScalingBounds();
            foreach (Form form in forms)
            {
                if (form.SensoryScaledValues.Alpha < _ContextScalingBounds.MinAlpha) _ContextScalingBounds.MinAlpha = form.SensoryScaledValues.Alpha;
                if (form.SensoryScaledValues.Alpha > _ContextScalingBounds.MaxAlpha) _ContextScalingBounds.MaxAlpha = form.SensoryScaledValues.Alpha;

                if (form.SensoryScaledValues.Red < _ContextScalingBounds.MinRed) _ContextScalingBounds.MinRed = form.SensoryScaledValues.Red;
                if (form.SensoryScaledValues.Red > _ContextScalingBounds.MaxRed) _ContextScalingBounds.MaxRed = form.SensoryScaledValues.Red;

                if (form.SensoryScaledValues.Green < _ContextScalingBounds.MinGreen) _ContextScalingBounds.MinGreen = form.SensoryScaledValues.Green;
                if (form.SensoryScaledValues.Green > _ContextScalingBounds.MaxGreen) _ContextScalingBounds.MaxGreen = form.SensoryScaledValues.Green;

                if (form.SensoryScaledValues.Blue < _ContextScalingBounds.MinBlue) _ContextScalingBounds.MinBlue = form.SensoryScaledValues.Blue;
                if (form.SensoryScaledValues.Blue > _ContextScalingBounds.MaxBlue) _ContextScalingBounds.MaxBlue = form.SensoryScaledValues.Blue;

                if (form.SensoryScaledValues.Xpos < _ContextScalingBounds.MinXpos) _ContextScalingBounds.MinXpos = form.SensoryScaledValues.Xpos;
                if (form.SensoryScaledValues.Xpos > _ContextScalingBounds.MaxXpos) _ContextScalingBounds.MaxXpos = form.SensoryScaledValues.Xpos;

                if (form.SensoryScaledValues.Ypos < _ContextScalingBounds.MinYpos) _ContextScalingBounds.MinYpos = form.SensoryScaledValues.Ypos;
                if (form.SensoryScaledValues.Ypos > _ContextScalingBounds.MaxYpos) _ContextScalingBounds.MaxYpos = form.SensoryScaledValues.Ypos;

                if (form.SensoryScaledValues.Width < _ContextScalingBounds.MinFormWidth) _ContextScalingBounds.MinFormWidth = form.SensoryScaledValues.Width;
                if (form.SensoryScaledValues.Width > _ContextScalingBounds.MaxFormWidth) _ContextScalingBounds.MaxFormWidth = form.SensoryScaledValues.Width;

                if (form.SensoryScaledValues.Height < _ContextScalingBounds.MinFormHeight) _ContextScalingBounds.MinFormHeight = form.SensoryScaledValues.Height;
                if (form.SensoryScaledValues.Height > _ContextScalingBounds.MaxFormHeight) _ContextScalingBounds.MaxFormHeight = form.SensoryScaledValues.Height;
            }
        }

        private static void ComputeContextValues(List<Form> forms)
        {
            ContextScalingInit(forms);
            foreach (Form form in forms)
            {
                form.ComputeContextValues(_ContextScalingBounds);
            }
        }

        private static int CompareAscSaliencyProperty(System.Reflection.PropertyInfo a, System.Reflection.PropertyInfo b)
        {
            double _a = (double)a.GetValue(SaliencyValues);
            double _b = (double)b.GetValue(SaliencyValues);
            if (_a == _b) return 0;
            else if (_a > _b) return 1;
            else return -1;
        }

        private static int CompareDescSaliencyProperty(System.Reflection.PropertyInfo a, System.Reflection.PropertyInfo b)
        {
            return -CompareAscSaliencyProperty(a, b);
        }

        private static void ComputeScalingValues(Bitmap bmp, List<Form> forms)
        {
            ComputeScalingValues(bmp.Width, bmp.Height, forms);
        }

        private static void ComputeScalingValues(int width, int height, List<Form> forms)
        {
            ComputeSensoryScaling(width, height, forms);
            ComputeSaliencies(forms);
            ComputeContextValues(forms);
        }

        private static List<DiscriminationTree> GetDiscriminationTrees(TalkingHead th, List<Form> forms)
        {
            List<System.Reflection.PropertyInfo> values = new List<System.Reflection.PropertyInfo>();
            values.AddRange(SaliencyValues.GetType().GetProperties());
            values.Sort(CompareDescSaliencyProperty);

            List<DiscriminationTree> trees = new List<DiscriminationTree>();
            for (int i = 0; i < Configuration.Number_Of_Words; i++)
            {
                switch (values[i].Name)
                {
                    case "Alpha":
                        trees.Add(th.Alpha);
                        break;
                    case "Red":
                        trees.Add(th.Red);
                        break;
                    case "Green":
                        trees.Add(th.Green);
                        break;
                    case "Blue":
                        trees.Add(th.Blue);
                        break;
                    case "Xpos":
                        trees.Add(th.Xpos);
                        break;
                    case "Ypos":
                        trees.Add(th.Ypos);
                        break;
                    case "Width":
                        trees.Add(th.Width);
                        break;
                    case "Height":
                        trees.Add(th.Height);
                        break;
                }
            }
            return trees;
        }

        public static List<DiscriminationTree> GetDiscriminationTrees(TalkingHead th, Bitmap bmp, ImageFormat format, List<Form> forms = null) // the trees used to make a description
        {
            if (forms == null) forms = Eyes.FindForms(bmp, format);
            return GetDiscriminationTrees(th, forms);
        }

        public static List<DiscriminationTree> GetDiscriminationTrees(TalkingHead th, Stream str, int width, int height, List<Form> forms = null) // the trees used to make a description
        {
            if (forms == null) forms = Eyes.FindForms(str, width, height);
            return GetDiscriminationTrees(th, forms);
        }

        public static Form ChooseForm(List<Form> forms)
        {
            int index = Configuration.seed.Next(forms.Count());
            return forms[index];
        }

        private static string DiscriminationGameDescriptionGetDescription(List<Form> forms, List<DiscriminationTree> trees, out int IDChoice, List<DiscriminationTree.Guess> ProcessingMemory, bool printInConsole = false)
        {
            if (forms.Count() == 0) 
            {
                IDChoice = -1;
                return "";
            }
            Form chosenForm = ChooseForm(forms);
            IDChoice = chosenForm.ID;
            if (printInConsole) Console.WriteLine("The Talking Head chooses the form " + chosenForm.ID);

            string guess = "";
            string meaning = "";
            DiscriminationTree.Guess _guess = new DiscriminationTree.Guess();
            foreach (DiscriminationTree tree in trees)
            {
                if (guess != "")
                {
                    guess += Configuration.Word_Separator;
                    meaning += Configuration.Word_Separator;
                }
                switch (tree.Discriminant)
                {
                    case Enumerations.Disciminants.Alpha:
                        _guess = tree.GetGuess(chosenForm.SensoryScaledValues.Alpha);
                        break;
                    case Enumerations.Disciminants.Red:
                        _guess = tree.GetGuess(chosenForm.SensoryScaledValues.Red);
                        break;
                    case Enumerations.Disciminants.Green:
                        _guess = tree.GetGuess(chosenForm.SensoryScaledValues.Green);
                        break;
                    case Enumerations.Disciminants.Blue:
                        _guess = tree.GetGuess(chosenForm.SensoryScaledValues.Blue);
                        break;
                    case Enumerations.Disciminants.Xpos:
                        //_guess = tree.GetGuess(chosenForm.GetCenter().X);
                        _guess = tree.GetGuess(chosenForm.ContextScaledValues.Xpos);
                        break;
                    case Enumerations.Disciminants.Ypos:
                        //_guess = tree.GetGuess(chosenForm.GetCenter().Y);
                        _guess = tree.GetGuess(chosenForm.ContextScaledValues.Ypos);
                        break;
                    case Enumerations.Disciminants.Width:
                        //_guess = tree.GetGuess(chosenForm.Width());
                        _guess = tree.GetGuess(chosenForm.ContextScaledValues.Width);
                        break;
                    case Enumerations.Disciminants.Height:
                        //_guess = tree.GetGuess(chosenForm.Height());
                        _guess = tree.GetGuess(chosenForm.ContextScaledValues.Height);
                        break;
                }
                guess += _guess.Word;
                meaning += _guess.Node.Data.StringValue;

                ProcessingMemory.Add(_guess);
            }
            if (printInConsole)
            {
                Console.WriteLine("The Talking Head describes the form with " + meaning);
                Console.WriteLine("The Talking Head says '" + guess + "'");
            }
            return guess;
        }

        public static string DiscriminationGameDescription(TalkingHead th, Bitmap bmp, ImageFormat format, bool printInConsole = false)
        {
            List<Form> forms = Eyes.FindForms(bmp, format);
            ComputeScalingValues(bmp, forms);

            List<DiscriminationTree> trees = GetDiscriminationTrees(th, bmp, format, forms);

            return DiscriminationGameDescriptionGetDescription(forms, trees, out int IDChoice, new List<DiscriminationTree.Guess>(), printInConsole);
        }

        public static string DiscriminationGameDescription(TalkingHead th, Stream str, int width, int height, out int IDChoice, List<DiscriminationTree.Guess> ProcessingMemory, bool printInConsole = false)
        {
            List<Form> forms = Eyes.FindForms(str, width, height);
            ComputeScalingValues(width, height, forms);

            List<DiscriminationTree> trees = GetDiscriminationTrees(th, forms);

            return DiscriminationGameDescriptionGetDescription(forms, trees, out IDChoice, ProcessingMemory, printInConsole);
        }

        public static string DiscriminationGameDescription(TalkingHead th, int[] pixels, int width, int height, out int IDChoice, List<DiscriminationTree.Guess> ProcessingMemory, bool printInConsole = false)
        {
            List<Form> forms = Eyes.FindForms(pixels, width, height, false, true);
            ComputeScalingValues(width, height, forms);

            List<DiscriminationTree> trees = GetDiscriminationTrees(th, forms);

            return DiscriminationGameDescriptionGetDescription(forms, trees, out IDChoice, ProcessingMemory, printInConsole);
        }

        public static string DiscriminationGameDescription(TalkingHead th, string path, int width, int height, out int IDChoice, List<DiscriminationTree.Guess> ProcessingMemory, bool printInConsole = false)
        {
            throw new Exception("Obsolete");
            /*
            List<Form> forms = Eyes.FindForms(path, width, height);
            ComputeScalingValues(width, height, forms);

            List<DiscriminationTree> trees = GetDiscriminationTrees(th, forms);

            return DiscriminationGameDescriptionGetDescription(forms, trees, out IDChoice, ProcessingMemory, printInConsole);
            */
        }

        private static Form DiscriminationGameGuessDetails(TalkingHead th, IEnumerable<Form> forms, string guess, List<DiscriminationTree.Guess> ProcessingMemory, bool print = false)
        {
            List<LexiconAssocation> description = new List<LexiconAssocation>();

            // Translate the words
            foreach (string word in guess.Split(Configuration.Word_Separator))
            {
                //LexiconAssocation currentGuess = th.MakeGuess(word);
                DiscriminationTree.Node currentGuess = th.MakeGuessNode(word);
                if (currentGuess == null) throw (new ArgumentException("Unknown word"));
                else description.Add(currentGuess.Data);
                ProcessingMemory.Add(new DiscriminationTree.Guess { Node = currentGuess, Word = word });
            }

            // Check if the same tree appears twice in the description
            var tmp = description.GroupBy(x => x.TreeDiscriminant);
            bool duplicate = tmp.Any(x => x.Count() > 1);
            if (duplicate)
            {
                // should we accept it?
            }
            description = tmp.Select(x => x.First()).ToList();

            // Make guess
            foreach (LexiconAssocation descriptionPart in description)
            {
                switch (Enumerations.GetDiscriminant(descriptionPart.TreeDiscriminant))
                {
                    case Enumerations.Disciminants.Alpha:
                        forms = forms.Where(x => x.SensoryScaledValues.Alpha >= descriptionPart.MinValue && x.SensoryScaledValues.Alpha <= descriptionPart.MaxValue);
                        break;
                    case Enumerations.Disciminants.Red:
                        forms = forms.Where(x => x.SensoryScaledValues.Red >= descriptionPart.MinValue && x.SensoryScaledValues.Red <= descriptionPart.MaxValue);
                        break;
                    case Enumerations.Disciminants.Green:
                        forms = forms.Where(x => x.SensoryScaledValues.Green >= descriptionPart.MinValue && x.SensoryScaledValues.Green <= descriptionPart.MaxValue);
                        break;
                    case Enumerations.Disciminants.Blue:
                        forms = forms.Where(x => x.SensoryScaledValues.Blue >= descriptionPart.MinValue && x.SensoryScaledValues.Blue <= descriptionPart.MaxValue);
                        break;
                    case Enumerations.Disciminants.Xpos:
                        forms = forms.Where(x => x.ContextScaledValues.Xpos >= descriptionPart.MinValue && x.ContextScaledValues.Xpos <= descriptionPart.MaxValue);
                        break;
                    case Enumerations.Disciminants.Ypos:
                        forms = forms.Where(x => x.ContextScaledValues.Ypos >= descriptionPart.MinValue && x.ContextScaledValues.Ypos <= descriptionPart.MaxValue);
                        break;
                    case Enumerations.Disciminants.Width:
                        forms = forms.Where(x => x.ContextScaledValues.Width >= descriptionPart.MinValue && x.ContextScaledValues.Width <= descriptionPart.MaxValue);
                        break;
                    case Enumerations.Disciminants.Height:
                        forms = forms.Where(x => x.ContextScaledValues.Height >= descriptionPart.MinValue && x.ContextScaledValues.Height <= descriptionPart.MaxValue);
                        break;
                }
            }
            Form response = forms.FirstOrDefault();
            if (print)
            {
                if (response != null) Console.WriteLine(th.Name + "makes the guess " + response.ID);
                else Console.WriteLine(th.Name + " could not find a corresponding form.");
            }
            return response;
        }

        [Obsolete("Use the Stream version because this one has not been update to follow all the score management rules as it was not compatible with the phone application. Or make sure to update it correctly.")]
        public static Form DiscriminationGameGuess(TalkingHead th, Bitmap bmp, ImageFormat format, string guess, bool printInConsole = false)
        {
            IEnumerable<Form> forms = Eyes.FindForms(bmp, format) as IEnumerable<Form>;
            ComputeScalingValues(bmp, forms.ToList());

            List<DiscriminationTree.Guess> ProcessingMemory = new List<DiscriminationTree.Guess>();
            return DiscriminationGameGuessDetails(th, forms, guess, ProcessingMemory, printInConsole);
        }

        public static Form DiscriminationGameGuess(TalkingHead th, Stream str, int width, int height, string guess, List<DiscriminationTree.Guess> ProcessingMemory, bool printInConsole = false)
        {
            if (guess == "") return null;
            IEnumerable<Form> forms = Eyes.FindForms(str, width, height) as IEnumerable<Form>;
            ComputeScalingValues(width, height, forms.ToList());

            return DiscriminationGameGuessDetails(th, forms, guess, ProcessingMemory, printInConsole);
        }

        public static Form DiscriminationGameGuess(TalkingHead th, int[] pixels, int width, int height, string guess, List<DiscriminationTree.Guess> ProcessingMemory, bool printInConsole = false)
        {
            if (guess == "") return null;
            IEnumerable<Form> forms = Eyes.FindForms(pixels, width, height, false, true) as IEnumerable<Form>;
            ComputeScalingValues(width, height, forms.ToList());

            return DiscriminationGameGuessDetails(th, forms, guess, ProcessingMemory, printInConsole);
        }

        public static int DiscriminationGameGuessID(TalkingHead th, Bitmap bmp, ImageFormat format, string guess, bool printInConsole = false)
        {
            Form form = DiscriminationGameGuess(th, bmp, format, guess, printInConsole);
            if (form == null) return -1;
            return form.ID;
        }

        public static int DiscriminationGameGuessID(TalkingHead th, Stream str, int width, int height, string guess, List<DiscriminationTree.Guess> ProcessingMemory, bool printInConsole = false)
        {
            try
            {
                Form form = DiscriminationGameGuess(th, str, width, height, guess, ProcessingMemory, printInConsole);
                if (form == null) return -1;
                return form.ID;
            }
            catch (ArgumentException e) // word unknown
            {
                return -2;
            }
        }

        public static int DiscriminationGameGuessID(TalkingHead th, int[] pixels, int width, int height, string guess, List<DiscriminationTree.Guess> ProcessingMemory, bool printInConsole = false)
        {
            try
            {
                Form form = DiscriminationGameGuess(th, pixels, width, height, guess, ProcessingMemory, printInConsole);
                if (form == null) return -1;
                return form.ID;
            }
            catch (ArgumentException e) // word unknown
            {
                return -2;
            }
        }

        private static void EnterCorrectForm_ErodeWordInOtherNodes(TalkingHead th, string word, DiscriminationTree.Node node)
        {
            if (node.Data.Words.ContainsKey(word))
            {
                // add the score we will remove to the word in other nodes
                node.Data.Words[word] += Configuration.Word_Score_Update_When_Other_Correct_Guesser;
                if (node.Data.Words[word] > Configuration.Word_Score_Max) node.Data.Words[word] = Configuration.Word_Score_Max;
            }
            else
            {
                node.Left.Data.Words[word] += Configuration.Word_Score_Update_When_Other_Correct_Guesser;
                if (node.Left.Data.Words[word] > Configuration.Word_Score_Max) node.Left.Data.Words[word] = Configuration.Word_Score_Max;
                node.Right.Data.Words[word] += Configuration.Word_Score_Update_When_Other_Correct_Guesser;
                if (node.Right.Data.Words[word] > Configuration.Word_Score_Max) node.Right.Data.Words[word] = Configuration.Word_Score_Max;
            }

            // remove the score in all apparitions of the word
            th.RemoveScoreForWord(word);
        }

        private static void EnterCorrectFormDetails(TalkingHead th, string description, int IDForm, List<Form> forms)
        {
            string[] words = description.Split(Configuration.Word_Separator);
            int index = 0;

            Form correctForm = forms.FirstOrDefault(x => x.ID == IDForm);
            if (correctForm == null) return;

            List<DiscriminationTree> trees = GetDiscriminationTrees(th, forms);
            foreach (DiscriminationTree tree in trees)
            {
                if (index > (words.Length - 1)) continue;
                string word = words[index];
                index++;

                DiscriminationTree.Node node;
                switch (tree.Discriminant)
                {
                    case Enumerations.Disciminants.Alpha:
                        node = th.Alpha.GetNode(correctForm.SensoryScaledValues.Alpha);
                        node.CorrectForm(word);
                        EnterCorrectForm_ErodeWordInOtherNodes(th, word, node);
                        break;
                    case Enumerations.Disciminants.Red:
                        node = th.Red.GetNode(correctForm.SensoryScaledValues.Red);
                        node.CorrectForm(word);
                        EnterCorrectForm_ErodeWordInOtherNodes(th, word, node);
                        break;
                    case Enumerations.Disciminants.Green:
                        node = th.Green.GetNode(correctForm.SensoryScaledValues.Green);
                        node.CorrectForm(word);
                        EnterCorrectForm_ErodeWordInOtherNodes(th, word, node);
                        break;
                    case Enumerations.Disciminants.Blue:
                        node = th.Blue.GetNode(correctForm.SensoryScaledValues.Blue);
                        node.CorrectForm(word);
                        EnterCorrectForm_ErodeWordInOtherNodes(th, word, node);
                        break;
                    case Enumerations.Disciminants.Xpos:
                        node = th.Xpos.GetNode(correctForm.ContextScaledValues.Xpos);
                        node.CorrectForm(word);
                        EnterCorrectForm_ErodeWordInOtherNodes(th, word, node);
                        break;
                    case Enumerations.Disciminants.Ypos:
                        node = th.Ypos.GetNode(correctForm.ContextScaledValues.Ypos);
                        node.CorrectForm(word);
                        EnterCorrectForm_ErodeWordInOtherNodes(th, word, node);
                        break;
                    case Enumerations.Disciminants.Width:
                        node = th.Width.GetNode(correctForm.ContextScaledValues.Width);
                        node.CorrectForm(word);
                        EnterCorrectForm_ErodeWordInOtherNodes(th, word, node);
                        break;
                    case Enumerations.Disciminants.Height:
                        node = th.Height.GetNode(correctForm.ContextScaledValues.Height);
                        node.CorrectForm(word);
                        EnterCorrectForm_ErodeWordInOtherNodes(th, word, node);
                        break;
                }
            }
        }
        public static void EnterCorrectForm(TalkingHead th, int[] pixels, int width, int height, string description, int IDForm)
        {
            List<Form> forms = Eyes.FindForms(pixels, width, height, false, true);
            ComputeScalingValues(width, height, forms);

            EnterCorrectFormDetails(th, description, IDForm, forms);
        }

        public static void EnterCorrectForm(TalkingHead th, Stream str, int width, int height, string description, int IDForm)
        {
            List<Form> forms = Eyes.FindForms(str, width, height, false, true);
            ComputeScalingValues(width, height, forms);

            EnterCorrectFormDetails(th, description, IDForm, forms);
        }
    }
}
