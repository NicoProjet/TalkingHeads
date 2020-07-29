using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace GUI.Utils
{
    public interface IImageManager
    {
        Size GetDimensionsFrom(byte[] bytes);
    }
}
