using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Drawing;

namespace ProducerAlpha
{
    public class ImageFast
    {
        [DllImport("gdiplus.dll", CharSet = CharSet.Unicode)]
        public static extern int GdipLoadImageFromFile(string filename, out IntPtr image);

        private ImageFast()
        {
        }

        private static Type imageType = typeof(System.Drawing.Bitmap);

        public static Image FastFromFile(string filename)
        {
            filename = Path.GetFullPath(filename);
            IntPtr loadingImage = IntPtr.Zero;

            // We are not using ICM at all, fudge that, this should be FAAAAAST!
            if (GdipLoadImageFromFile(filename, out loadingImage) != 0)
            {
                throw new Exception("GDI+ threw a status error code.");
            }

            return (Bitmap)imageType.InvokeMember("FromGDIplus", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { loadingImage });
        }
    } 
}
