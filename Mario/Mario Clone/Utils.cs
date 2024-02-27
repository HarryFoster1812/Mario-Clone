using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Mario_Clone
{
    class Utils
    {
        public static Image ImageFromPath(string path)
        {
            Image img = new Image();
            var src = BitmapSourceFromPath(path);
            img.Source = src;

            return img;
        }
        public static BitmapImage BitmapSourceFromPath(string path)
        {
            string wd = Directory.GetCurrentDirectory();
            var src = new BitmapImage(new Uri(wd + @"\" + path));

            return src;
        }
        public static TransformedBitmap FlippedBitmapSourceFromPath(string path)
        {
            string wd = Directory.GetCurrentDirectory();
            var src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(wd + @"\" + path, UriKind.Absolute);
            src.EndInit();
            var flipped = new TransformedBitmap();
            flipped.BeginInit();
            flipped.Source = src;
            flipped.Transform = new ScaleTransform(-1, 1, 0, 0); ;
            flipped.EndInit();

            return flipped;
        }

    }
}