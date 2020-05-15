using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace TalkingHeads.BodyParts
{
    public static class Memory
    {
        public static byte[] LoadImageToByte(string fileName)
        {
            return File.ReadAllBytes(fileName);
            /* Obsolete
            using (Image img = Image.FromFile(fileName))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    img.Save(ms, img.RawFormat);
                    return ms.ToArray();
                }
            }
            */
        }

        public static MemoryStream LoadImageToMemoryStream(string fileName)
        {
            byte[] bytesArray = File.ReadAllBytes(fileName);
            return new MemoryStream(bytesArray);
            /* Obsolete
            using (Image img = Image.FromFile(fileName))
            {
                MemoryStream ms = new MemoryStream();
                img.Save(ms, img.RawFormat);
                return ms;
            }
            */ 
        }

        public static Bitmap LoadImageToBmp(string fileName)
        {
            return new Bitmap(fileName);
        }

        public static Image LoadImage(string fileName)
        {
            return Image.FromFile(fileName);
        }
    }
}
