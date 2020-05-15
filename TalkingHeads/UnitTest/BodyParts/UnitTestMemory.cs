using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkingHeads.BodyParts;

namespace TalkingHeads.UnitTest.BodyParts
{
    public class UnitTestMemory
    {
        public static bool RunAll(string fileName)
        {
            bool result = UT_LoadImageToByte(fileName);
            result &= UT_LoadImageToMemoryStream(fileName);
            result &= UT_LoadImageToBmp(fileName);
            result &= UT_LoadImage(fileName);
            Console.WriteLine("UnitTestMemory: " + (result ? "OK" : "Error") + " ..." );
            return result;
        }

        public static bool UT_LoadImageToByte(string fileName)
        {
            bool result = Memory.LoadImageToByte(fileName).Length != 0;
            if (!result) Console.WriteLine("UT_LoadImageToByte: File is empty");
            return result;
        }

        public static bool UT_LoadImageToMemoryStream(string fileName)
        {
            using (MemoryStream ms = Memory.LoadImageToMemoryStream(fileName))
            {
                bool result = ms.Length != 0;
                if (!result) Console.WriteLine("UT_LoadImageToMemoryStream: File is empty");
                return result;
            }
        }

        public static bool UT_LoadImageToBmp(string fileName)
        {
            using (Bitmap bmp = Memory.LoadImageToBmp(fileName))
            {
                bool result = bmp.Width != 0;
                if (!result) Console.WriteLine("UT_LoadImage: File is empty");
                return result;
            }
        }

        public static bool UT_LoadImage(string fileName)
        {
            bool result = !Memory.LoadImage(fileName).Size.IsEmpty;
            if (!result) Console.WriteLine("UT_LoadImage: File is empty");
            return result;
        }
    }
}
