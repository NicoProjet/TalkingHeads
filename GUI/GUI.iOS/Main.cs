using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using GUI.iOS;
using GUI.Utils;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(ImageManager))]
namespace GUI.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");
        }
    }

    
    public class ImageManager : IImageManager
    {
        public Size GetDimensionsFrom(byte[] bytes)
        {
            var data = NSData.FromArray(bytes);
            UIImage originalImage = new UIImage(data);
            return new Size(originalImage.Size.Width, originalImage.Size.Height);
        }
    }
}
