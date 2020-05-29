using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkingHeads.DataStructures;

namespace TalkingHeads.BodyParts
{
    public static class Brain
    {
        public struct SensoryScalingBounds
        {
            public double MinARGB { get; set; }
            public double MaxARGB { get; set; }
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
            _SensoryScalingBounds.MinARGB = 0;
            _SensoryScalingBounds.MaxARGB = 255;
            _SensoryScalingBounds.MinFormWidth = 0;
            _SensoryScalingBounds.MaxFormWidth = bmp.Width / Configuration.MaxFormSizeDivide;
            _SensoryScalingBounds.MinFormHeight = 0;
            _SensoryScalingBounds.MaxFormHeight = bmp.Height / Configuration.MaxFormSizeDivide;
            _SensoryScalingBounds.MinXpos = 0;
            _SensoryScalingBounds.MaxXpos = bmp.Width;
            _SensoryScalingBounds.MinYpos = 0;
            _SensoryScalingBounds.MaxYpos = bmp.Height;
        }

        private static void ComputeScaledValues(Bitmap bmp, List<Form> forms)
        {
            SensoryScalingInit(bmp);
            foreach (Form form in forms)
            {
                form.ComputeScaledValues(_SensoryScalingBounds);
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
            ResetSaliencyValues();
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
            for (int i = 0; i < forms.Count(); i++)
            {
                for (int j = i+1; j < forms.Count(); j++)
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
            foreach(Form form in forms)
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
            ComputeScaledValues(bmp, forms);
            ComputeSaliencies(forms);
            ComputeContextValues(forms);
        }

        public static List<DiscriminationTree> GetDiscriminationTrees(TalkingHead th, Bitmap bmp, ImageFormat format, List<Form> forms = null) // the trees used to make a description
        {
            if (forms == null) forms = Eyes.FindForms(bmp, format);
            ComputeScalingValues(bmp, forms);
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

        public static Form ChooseForm(List<Form> forms)
        {
            int index = Configuration.seed.Next(forms.Count());
            return forms[index];
        }

        public static string DiscriminationGameDescription(TalkingHead th, Bitmap bmp, ImageFormat format, bool printInConsole = false)
        {
            List<Form> forms = Eyes.FindForms(bmp, format);
            List<DiscriminationTree> trees = GetDiscriminationTrees(th, bmp, format, forms);

            Form chosenForm = ChooseForm(forms);
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
                        _guess = tree.GetGuess(chosenForm.Centroid.A);
                        break;
                    case Enumerations.Disciminants.Red:
                        _guess = tree.GetGuess(chosenForm.Centroid.R);
                        break;
                    case Enumerations.Disciminants.Green:
                        _guess = tree.GetGuess(chosenForm.Centroid.G);
                        break;
                    case Enumerations.Disciminants.Blue:
                        _guess = tree.GetGuess(chosenForm.Centroid.B);
                        break;
                    case Enumerations.Disciminants.Xpos:
                        _guess = tree.GetGuess(chosenForm.GetCenter().X);
                        break;
                    case Enumerations.Disciminants.Ypos:
                        _guess = tree.GetGuess(chosenForm.GetCenter().Y);
                        break;
                    case Enumerations.Disciminants.Width:
                        _guess = tree.GetGuess(chosenForm.Width());
                        break;
                    case Enumerations.Disciminants.Height:
                        _guess = tree.GetGuess(chosenForm.Height());
                        break;
                }
                guess += _guess.Word;
                meaning += _guess.Node.Data.StringValue;
            }
            if (printInConsole) Console.WriteLine(meaning);
            return guess;
        }

        public static Form DiscriminationGameGuess(TalkingHead th, Bitmap bmp, ImageFormat format, bool printInConsole = false)
        {
            List<Form> forms = Eyes.FindForms(bmp, format);

            ComputeScaledValues(bmp, forms);

            throw new NotImplementedException();
        }
    }
}
