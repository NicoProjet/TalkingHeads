using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GUI.Utils
{
    public interface IImageUtils
    {
        byte[] GetByteArrayFromStream(Stream str);
        int[] Uncompress(Stream str);
    }
}
