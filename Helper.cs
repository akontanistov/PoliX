using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Drawing.Imaging; 

namespace PoliX
{
    static class Helper
    {
        static Random rnd = new Random();
        public static double RndRange(double _min, double _max)
        {
            double range = _max - _min;
            double sample = rnd.NextDouble();
            double scaled = (sample * range) + _min;
            double f = scaled;
            return f;
        }

        public static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        public static BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        public static void Randomize<T>(this IList<T> target)
        {
            Random RndNumberGenerator = new Random();
            SortedList<int, T> newList = new SortedList<int, T>();
            foreach (T item in target)
            {
                newList.Add(RndNumberGenerator.Next(), item);
            }
            target.Clear();
            for (int i = 0; i < newList.Count; i++)
            {
                target.Add(newList.Values[i]);
            }
        }

        public static bool PointIsPoint(Vector2 _a, Vector2 _b)
        {
            if (_a.x == _b.x && _a.y == _b.y)
            {
                return true;
            }
            else
                return false;
        }

    }
}
