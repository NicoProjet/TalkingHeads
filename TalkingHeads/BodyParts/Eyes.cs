using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using TalkingHeads.DataStructures;
using Xam = Xamarin.Forms;

namespace TalkingHeads.BodyParts
{
    public static class Eyes
    {
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

        // True if colors are close
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

        private static bool CompareColorToFormColor(Color c, Form form)
        {
            int A_diff = Math.Abs((int)c.A - (int)form.StartingColor.A);
            int R_diff = Math.Abs((int)c.R - (int)form.StartingColor.R);
            int G_diff = Math.Abs((int)c.G - (int)form.StartingColor.G);
            int B_diff = Math.Abs((int)c.B - (int)form.StartingColor.B);
            return A_diff < Configuration.MAX_A_Diff_Starting_Color
                && R_diff < Configuration.MAX_R_Diff_Starting_Color
                && G_diff < Configuration.MAX_G_Diff_Starting_Color
                && B_diff < Configuration.MAX_B_Diff_Starting_Color
                && A_diff + R_diff + G_diff + B_diff <= Configuration.Max_Color_diff_Starting_Color
                && CompareColors(c, form.Centroid);
        }

        private static bool CompareColorsBackground(Color c1, Color c2)
        {
            int A_diff = Math.Abs((int)c1.A - (int)c2.A);
            int R_diff = Math.Abs((int)c1.R - (int)c2.R);
            int G_diff = Math.Abs((int)c1.G - (int)c2.G);
            int B_diff = Math.Abs((int)c1.B - (int)c2.B);
            return A_diff < Configuration.MAX_A_Diff_Backgroung
                && R_diff < Configuration.MAX_R_Diff_Backgroung
                && G_diff < Configuration.MAX_G_Diff_Backgroung
                && B_diff < Configuration.MAX_B_Diff_Backgroung
                && A_diff + R_diff + G_diff + B_diff <= Configuration.Max_Color_diff_Backgroung;
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

        private static List<Form> CleanForms(List<Form> forms, int width, int height, bool print = false, bool cleanPrecisionLoss = true)
        {
            if (forms.Count() == 0) return forms;

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
                //if (DistanceColor(forms[index].Centroid, BackGroundColor) < Configuration.Max_Color_Dist_BackGround)
                if (CompareColorsBackground(forms[index].Centroid, BackGroundColor))
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
                int minWidht = width / Configuration.MinFormSizeDivide * Configuration.PrecisionLossCleaningCoeff / 100;
                int minHeight = height / Configuration.MinFormSizeDivide * Configuration.PrecisionLossCleaningCoeff / 100;
                index = 0;
                while (index < forms.Count())
                {
                    if (forms[index].Width() < minWidht || forms[index].Height() < minHeight || forms[index].pixelNumber < (ulong)(minHeight * minWidht / 2)) forms.RemoveAt(index);
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

            // Set IDs
            int counter = 0;
            foreach (Form form in forms)
            {
                form.ID = counter++;
            }

            return forms;
        }

        private static List<Form> FindForms(Bitmap bmp, bool print = false, bool cleanPrecisionLoss = false)
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
                    }
                    else
                    {
                        form.Add(i, j, c);
                    }
                }
                forms = forms.Where(x => x.pixelNumber > (ulong)Configuration.Min_PixelPerLine).ToList();
            }
            return CleanForms(forms, bmp.Width, bmp.Height, print, cleanPrecisionLoss);
        }

        public static List<Form> FindForms(Bitmap bmp, ImageFormat format)
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

        public static List<Form> FindForms(Stream str, int width, int height, bool print = false, bool cleanPrecisionLoss = false)
        {
            Image bmp = Bitmap.FromStream(str);

            if (print) Console.WriteLine("Find Segments");
            List<Form> forms = new List<Form>();
            BinaryReader br = new BinaryReader(str);
            height = (int) Math.Floor(Math.Sqrt(str.Length / ((double)(4 * 4) / 3)));
            width = (int) Math.Floor(Math.Sqrt(str.Length / ((double)(4 * 4) / 3)) * ((double)4 / (double)3));

            //str.Seek(Configuration.NumberOfBytesInBmpHeaderStream, SeekOrigin.Begin);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    //Color c = Color.FromArgb(str.ReadByte(), str.ReadByte(), str.ReadByte(), str.ReadByte());
                    uint A = br.ReadByte();
                    uint R = br.ReadByte();
                    uint G = br.ReadByte();
                    uint B = br.ReadByte();
                    Color c = Color.FromArgb((int)A, (int)R, (int)G, (int)B);
                    Form form = forms.FirstOrDefault(x => CompareColors(x.Centroid, c));

                    //Form form = forms.Where(x => CompareColors(x.Centroid, c)).OrderBy(x => DistanceColor(x.Centroid, c)).FirstOrDefault();
                    if (form == null)
                    {
                        forms.Add(new Form(i, j, c));
                    }
                    else
                    {
                        form.Add(i, j, c);
                    }
                }
                forms = forms.Where(x => x.pixelNumber > (ulong)Configuration.Min_PixelPerLine).ToList();
            }
            return CleanForms(forms, width, height, print, cleanPrecisionLoss);
        }

        public static List<Form> FindForms(int[] pixels, int width, int height, bool print = false, bool cleanPrecisionLoss = false)
        {
            if (print) Console.WriteLine("Find Segments");
            List<Form> forms = new List<Form>();

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    //Color c = Color.FromArgb(pixels[i * height + j]);
                    Color c = Color.FromArgb(pixels[j * width + i]);
                    Form form = forms.FirstOrDefault(x => CompareColorToFormColor(c, x));

                    //Form form = forms.Where(x => CompareColors(x.Centroid, c)).OrderBy(x => DistanceColor(x.Centroid, c)).FirstOrDefault();
                    if (form == null)
                    {
                        forms.Add(new Form(i, j, c));
                    }
                    else
                    {
                        form.Add(i, j, c);
                    }
                }
                forms = forms.Where(x => x.pixelNumber > (ulong)Configuration.Min_PixelPerLine).ToList();
            }
            return CleanForms(forms, width, height, print, cleanPrecisionLoss);
        }

        public static List<Form> FindFormsJPG(Stream str, int width, int height, bool print = false, bool cleanPrecisionLoss = false)
        {
            byte[] arr = GetImageStreamAsBytes(str);
            if (print) Console.WriteLine("Find Segments");
            List<Form> forms = new List<Form>();

            //str.Seek(Configuration.NumberOfBytesInBmpHeaderStream, SeekOrigin.Begin);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j+=4)
                {
                    byte A = arr[i * height + j];
                    byte R = arr[i * height + j+1];
                    byte G = arr[i * height + j+2];
                    byte B = arr[i * height + j+3];
                    Color c = Color.FromArgb(A, R, G, B);
                    Form form = forms.FirstOrDefault(x => CompareColors(x.Centroid, c));

                    if (form == null)
                    {
                        forms.Add(new Form(i, j, c));
                    }
                    else
                    {
                        form.Add(i, j, c);
                    }
                }
                forms = forms.Where(x => x.pixelNumber > (ulong)Configuration.Min_PixelPerLine).ToList();
            }
            return CleanForms(forms, width, height, print, cleanPrecisionLoss);
        }

        private static byte[] GetImageStreamAsBytes(Stream input)
        {
            input.Seek(0, SeekOrigin.Begin);
            var buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static byte[] GetImage(string path)
        {
            Image img = Image.FromFile(path);
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        private static List<Rectangle> FindFormsSegments(Bitmap bmp, bool print = false, bool cleanPrecisionLoss = false)
        {
            return FindForms(bmp, print, cleanPrecisionLoss).Select(x => x.Rect).ToList();
        }

        public static List<Rectangle> FindFormsSegments(Bitmap bmp, ImageFormat format)
        {
            if (format == ImageFormat.Jpeg)
            {
                return FindFormsSegments(bmp, false, true);
            }
            else if (format == ImageFormat.Bmp)
            {
                return FindFormsSegments(bmp, false, false);
            }
            else if (format == ImageFormat.Png)
            {
                return FindFormsSegments(bmp, false, true);
            }
            else
            {
                return FindFormsSegments(bmp, false, false);
            }
        }
    }
}
