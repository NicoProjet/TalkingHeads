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
            // values are integers like during form creation in GeomWorld
            _SensoryScalingBounds.MinARGB = 0;
            _SensoryScalingBounds.MaxARGB = 255;
            _SensoryScalingBounds.MinFormWidth = bmp.Width / Configuration.MinFormSizeDivide;
            _SensoryScalingBounds.MaxFormWidth = bmp.Width / Configuration.MaxFormSizeDivide;
            _SensoryScalingBounds.MinFormHeight = bmp.Height / Configuration.MinFormSizeDivide;
            _SensoryScalingBounds.MaxFormHeight = bmp.Height / Configuration.MaxFormSizeDivide;
            _SensoryScalingBounds.MinXpos = 0;
            _SensoryScalingBounds.MaxXpos = bmp.Width;
            _SensoryScalingBounds.MinYpos = 0;
            _SensoryScalingBounds.MaxYpos = bmp.Height;
        }

        private static void ComputeScaledValues(List<Form> forms)
        {
            foreach(Form form in forms)
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

        private static List<DiscriminationTree> GetDiscriminationTrees(TalkingHead th, Bitmap bmp, ImageFormat format) // the trees used to make a description
        {
            SensoryScalingInit(bmp);
            List<DiscriminationTree> trees = th.GetTrees();
            List<Form> forms = Eyes.FindForms(bmp, format);

            ComputeScaledValues(forms);
            ComputeSaliencies(forms);
            ComputeContextValues(forms);

            throw new NotImplementedException("Get discrimination trees used for the description using saliency.");
        }
    }
}
