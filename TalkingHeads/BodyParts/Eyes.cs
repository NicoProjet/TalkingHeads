using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace TalkingHeads.BodyParts
{
    public static class Eyes
    {

        private class Form
        {
            public Point TopLeft;
            public bool TopLeft_initialized = false;
            public Point BottomRight;
            public bool BottomRight_initialized = false;
            public Color Centroid;
            public bool Centroid_initialized = false;
            public UInt64 pixelNumber = 0;
            public Rectangle Rect;

            public Form(int x, int y, Color color)
            {
                Point p = new Point(x, y);
                Add(p, color);
            }

            public Form(Point p, Color color)
            {
                Add(p, color);
            }

            public void ComputeTopLeft(Point p)
            {
                if (!TopLeft_initialized)
                {
                    TopLeft = new Point(p.X, p.Y);
                    TopLeft_initialized = true;
                }
                else
                {
                    if (TopLeft.X > p.X)
                    {
                        TopLeft.X = p.X;
                    }
                    if (TopLeft.Y > p.Y)
                    {
                        TopLeft.Y = p.Y;
                    }
                }
            }

            public void ComputeBottomRight(Point p)
            {
                if (!BottomRight_initialized)
                {
                    BottomRight = new Point(p.X, p.Y);
                    BottomRight_initialized = true;
                }
                else
                {
                    if (BottomRight.X < p.X)
                    {
                        BottomRight.X = p.X;
                    }
                    if (BottomRight.Y < p.Y)
                    {
                        BottomRight.Y = p.Y;
                    }
                }
            }

            public void ComputeCentroid(Color c)
            {
                if (!Centroid_initialized)
                {
                    Centroid = c;
                    Centroid_initialized = true;
                }
                else
                {
                    int A = Centroid.A;
                    int diff = (int)Centroid.A - (int)c.A;
                    if (pixelNumber < Int32.MaxValue)
                    {
                        if ((ulong)Math.Abs(diff) / (pixelNumber + 1) >= 1) A -= diff / ((int)pixelNumber + 1);
                    }

                    int R = Centroid.R;
                    diff = (int)Centroid.R - (int)c.R;
                    if (pixelNumber < Int32.MaxValue)
                    {
                        if ((ulong)Math.Abs(diff) / (pixelNumber + 1) >= 1) R -= diff / ((int)pixelNumber + 1);
                    }

                    int G = Centroid.G;
                    diff = (int)Centroid.G - (int)c.G;
                    if (pixelNumber < Int32.MaxValue)
                    {
                        if ((ulong)Math.Abs(diff) / (pixelNumber + 1) >= 1) G -= diff / ((int)pixelNumber + 1);
                    }

                    int B = Centroid.B;
                    diff = (int)Centroid.B - (int)c.B;
                    if (pixelNumber < Int32.MaxValue)
                    {
                        if ((ulong)Math.Abs(diff) / (pixelNumber + 1) >= 1) B -= diff / ((int)pixelNumber + 1);
                    }

                    Centroid = Color.FromArgb(A, R, G, B);
                }
            }

            public void ComputeRectangle()
            {
                Rect = Rectangle.FromLTRB(TopLeft.X, TopLeft.Y, BottomRight.X, BottomRight.Y);
            }

            public void Add(Point p, Color c)
            {
                ComputeTopLeft(p);
                ComputeBottomRight(p);
                ComputeCentroid(c);
                pixelNumber++;
            }

            public void Add(int x, int y, Color c)
            {
                Add(new Point(x, y), c);
            }

            public bool SameColor(Color c)
            {
                return Centroid.A == c.A && Centroid.R == c.R && Centroid.G == c.G && Centroid.B == c.B;
            }

            public Rectangle ToRectangle()
            {
                return Rectangle.FromLTRB(TopLeft.X, TopLeft.Y, BottomRight.X, BottomRight.Y);
            }

            public int Width()
            {
                if (!BottomRight_initialized || !TopLeft_initialized) return -1;
                else return BottomRight.X - TopLeft.X;
            }
            public int Height()
            {
                if (!BottomRight_initialized || !TopLeft_initialized) return -1;
                else return BottomRight.Y - TopLeft.Y;
            }

            public bool IntersectsWith(Form other)
            {
                return other.BottomRight.X >= TopLeft.X
                    && other.TopLeft.X <= BottomRight.X
                    && other.BottomRight.Y >= TopLeft.Y
                    && other.TopLeft.Y <= BottomRight.Y;
            }
        }



        private static uint ToDoubleWord(Color c)
        {
            uint value = 0;
            value += c.A;
            value <<= 8;
            value += c.R;
            value <<= 8;
            value += c.G;
            value <<= 8;
            value += c.B;
            value <<= 8;
            return value;
        }

        private static void PrintForms(List<Form> forms)
        {
            foreach (Form f in forms)
            {
                Console.WriteLine(f.Centroid + " - " + f.pixelNumber);
            }
            Console.WriteLine(forms.Count());
        }

        private static bool CompareARGB(Color c1, Color c2)
        {
            int A_diff = Math.Abs((int)c1.A - (int)c2.A);
            int R_diff = Math.Abs((int)c1.R - (int)c2.R);
            int G_diff = Math.Abs((int)c1.G - (int)c2.G);
            int B_diff = Math.Abs((int)c1.B - (int)c2.B);
            return A_diff < Configuration.Max_A_diff
                && R_diff < Configuration.Max_R_diff
                && G_diff < Configuration.Max_G_diff
                && B_diff < Configuration.Max_B_diff;
        }

        private static int ComputeColorDifference(Color c1, Color c2)
        {
            int A_diff = Math.Abs((int)c1.A - (int)c2.A);
            int R_diff = Math.Abs((int)c1.R - (int)c2.R);
            int G_diff = Math.Abs((int)c1.G - (int)c2.G);
            int B_diff = Math.Abs((int)c1.B - (int)c2.B);
            return A_diff + R_diff + G_diff + B_diff;
        }

        private static bool CompareColors(Color c1, Color c2)
        {
            int A_diff = Math.Abs((int)c1.A - (int)c2.A);
            int R_diff = Math.Abs((int)c1.R - (int)c2.R);
            int G_diff = Math.Abs((int)c1.G - (int)c2.G);
            int B_diff = Math.Abs((int)c1.B - (int)c2.B);
            return A_diff < Configuration.Max_A_diff
                && R_diff < Configuration.Max_R_diff
                && G_diff < Configuration.Max_G_diff
                && B_diff < Configuration.Max_B_diff
                && A_diff + R_diff + G_diff + B_diff <= Configuration.Max_Color_diff;
        }

        private static int DistanceColor(Color c1, Color c2)
        {
            int A_diff = (int)c1.A - (int)c2.A;
            int R_diff = (int)c1.R - (int)c2.R;
            int G_diff = (int)c1.G - (int)c2.G;
            int B_diff = (int)c1.B - (int)c2.B;
            return (int)Math.Sqrt(Math.Pow(A_diff, 2)
                                 + Math.Pow(R_diff, 2)
                                 + Math.Pow(G_diff, 2)
                                 + Math.Pow(B_diff, 2));
        }

        public static List<Rectangle> FindForms(Bitmap bmp, bool print = false, bool cleanPrecisionLoss = false)
        {
            // Classify pixels by color
            if (print) Console.WriteLine("Find Segments");
            List<Form> forms = new List<Form>();
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color c = bmp.GetPixel(i, j);
                    Form form = forms.FirstOrDefault(x => CompareColors(x.Centroid, c));

                    //Form form = forms.Where(x => CompareColors(x.Centroid, c)).OrderBy(x => DistanceColor(x.Centroid, c)).FirstOrDefault();
                    if (form == null)
                    {
                        forms.Add(new Form(i, j, c));
                    } else
                    {
                        form.Add(i, j, c);
                    }
                }
                forms = forms.Where(x => x.pixelNumber > (ulong)Configuration.Min_PixelPerLine).ToList();
            }
            if (forms.Count() == 0) return forms.Select(x => x.Rect).ToList();

            // Compute segments for each figure found
            foreach (Form form in forms)
            {
                form.ComputeRectangle();
            }

            if (print) PrintForms(forms);

            // Remove background
            if (print) Console.WriteLine("Background removal");
            /* Simple and efficient but only works for bmp or no precision loss (before saving)
            Form backGround = forms.FirstOrDefault(x => x.TopLeft.X == 0 && x.TopLeft.Y == 0 && x.BottomRight.X == bmp.Width - 1 && x.BottomRight.Y == bmp.Height - 1);
            if (backGround == null)
            {
                backGround = forms.OrderByDescending(x => x.pixelNumber).First();
            }
            forms.Remove(backGround);
            */
            Color BackGroundColor;
            if (Configuration.DynamicBackGroundColor)
            {
                BackGroundColor = forms.OrderByDescending(x => x.pixelNumber).First().Centroid;
            }
            else
            {
                BackGroundColor = Color.White;
            }
            int index = 0;
            while (index < forms.Count())
            {
                var t = DistanceColor(forms[index].Centroid, BackGroundColor);
                if (DistanceColor(forms[index].Centroid, BackGroundColor) < Configuration.Max_Color_Dist)
                {
                    forms.RemoveAt(index);
                }
                else index++;
            }

            if (print) PrintForms(forms);

            // Clean precision loss due to image format to reduce error (slight chance of loosing segments too small)
            if (print) Console.WriteLine("Precision loss Cleaning");
            if (cleanPrecisionLoss)
            {
                int minWidht = bmp.Width / Configuration.MinFormSizeDivide * Configuration.PrecisionLossCleaningCoeff / 100;
                int minHeight = bmp.Height / Configuration.MinFormSizeDivide * Configuration.PrecisionLossCleaningCoeff / 100;
                index = 0;
                while(index < forms.Count())
                {
                    if (forms[index].Width() < minWidht || forms[index].Height() < minHeight) forms.RemoveAt(index);
                    else index++;
                }
            }
            if (print) PrintForms(forms);

            // Remove intersecting segments which can happen if the precision loss cause a segment around a figure 
            // Should not happen after a good cleaning of precision loss but was introduced before and was very cost efficient
            if (print) Console.WriteLine("Intersections removal");
            index = 0;
            while (index < forms.Count() - 1)
            {
                Form intersectedWith = forms.Skip(index + 1).FirstOrDefault(x => x.IntersectsWith(forms[index]));
                if (intersectedWith != null)
                {
                    if (forms[index].pixelNumber < intersectedWith.pixelNumber)
                    {
                        forms.RemoveAt(index);
                    }
                    else
                    {
                        forms.Remove(intersectedWith);
                    }
                }
                else
                {
                    index++;
                }
            }
            if (print) PrintForms(forms);
            return forms.Select(x => x.Rect).ToList();
        }

        public static List<Rectangle> FindForms(Bitmap bmp, ImageFormat format)
        {
            if (format == ImageFormat.Jpeg)
            {
                return FindForms(bmp, false, true);
            }
            else if (format == ImageFormat.Bmp)
            {
                return FindForms(bmp, false, false);
            }
            else if (format == ImageFormat.Png)
            {
                return FindForms(bmp, false, true);
            }
            else
            {
                return FindForms(bmp, false, false);
            }
        }
    }
}
