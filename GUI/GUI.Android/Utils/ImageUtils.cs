using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GUI.Utils;
using Java.IO;
using Java.Nio;
using SkiaSharp;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: Dependency(typeof(GUI.Droid.Utils.ImageUtils))]
namespace GUI.Droid.Utils
{
    class ImageUtils : IImageUtils
    {
        Task<Bitmap> GetBitmap(Xamarin.Forms.Image image)
        {
            var handler = new ImageLoaderSourceHandler();
            return handler.LoadImageAsync(image.Source, null);
        }

        public byte[] GetByteArrayFromStream(Stream str)
        {
            str.Seek(0, SeekOrigin.Begin);
            using (MemoryStream ms = new MemoryStream())
            {
                str.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public int[] Uncompress(Stream str)
        {
            byte[] JPG_data = GetByteArrayFromStream(str);
            Bitmap bmp = BitmapFactory.DecodeByteArray(JPG_data, 0, JPG_data.Length);
            int[] pixels = new int[bmp.Width * bmp.Height];
            bmp.GetPixels(pixels, 0, bmp.Width, 0, 0, bmp.Width, bmp.Height);
            return pixels;
        }




        public byte[] CustomUnCompressImage(Stream str)
        {
            str.Seek(0, SeekOrigin.Begin);
            Bitmap bmp = BitmapFactory.DecodeStream(str);
            //int numberOfBytes = bmp.getByteCount();
            int numberOfBytes = bmp.Width * bmp.Height * 4;
            ByteBuffer buffer = ByteBuffer.Allocate(numberOfBytes);
            bmp.CopyPixelsToBuffer(buffer);
            return buffer.ToArray<byte>();
            // unable to cast to byte array
        }
    }
}