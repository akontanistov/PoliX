using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.ComponentModel;
using System.Globalization;
using PoliX.Triangulation;

namespace PoliX
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool debug = true;


        //BitmapImage предсавление основного источника, не изменяется, используется только для чтения
        List<BitmapImage> sourceBitmapPictures;
        BitmapImage sourceBitmapPicture;
        Bitmap pointsBitmap;

        List<Vector2> Points = new List<Vector2>();
        List<Triangle> Triangles = new List<Triangle>();

        private int pointsCount = 0;
        private double widthSource;
        private double heightSource;

        void refreshSystem()
        {
            sourceBitmapPictures = new List<BitmapImage>();
            sourceBitmapPicture = null;
            pointsBitmap = null;
            Points.Clear();
            Triangles.Clear();
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void b_loadImg(object sender, RoutedEventArgs e)
        {
            refreshSystem();
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Images (*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|" + "All files (*.*)|*.*"; 

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;

                sourceBitmapPicture = new BitmapImage(new Uri(dlg.FileName));
                if (sourceBitmapPicture != null)
                {
                    sourceImg.Source = sourceBitmapPicture;
                }
            }
        }

        private void b_loadPoints(object sender, RoutedEventArgs e)
        {
            if (debug)
                Console.WriteLine("Создание точек");

            pointsCount = Int32.Parse(tbPoints.Text);
            
            if (sourceBitmapPicture != null)
            {
                widthSource = (double)(sourceBitmapPicture.PixelWidth);
                heightSource = (double)(sourceBitmapPicture.PixelHeight);
            }

            //Создание суперструктуры
            Points.Add(new Vector2(0d, 0d));
            Points.Add(new Vector2(widthSource, 0d));
            Points.Add(new Vector2(widthSource, heightSource));
            Points.Add(new Vector2(0d, heightSource));

            ////Генерация случайных точек
            for (int i = 0; i < pointsCount; i++)
            {
                Points.Add(Vector2.Vector2Rnd(0d, widthSource, 0d, heightSource));
                Console.WriteLine(Points[i].x + " " + Points[i].y);
            }
            Console.WriteLine(" ");

            //Отрисовка карты точек
            if (debug)
                Console.WriteLine("Отрисовка точек");

            Bitmap pBitmapForPoints = Helper.BitmapImage2Bitmap(sourceBitmapPicture);
            var graphics = Graphics.FromImage(pBitmapForPoints);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            for (int i = 0; i < Points.Count; i++)
            {
                graphics.FillEllipse(new System.Drawing.SolidBrush(System.Drawing.Color.Gold), (int)Points[i].x-5, (int)Points[i].y-5, 10, 10);
            }
            sourceImg.Source = Helper.Bitmap2BitmapImage(pBitmapForPoints);

            //pointsBitmap = Help.BitmapImage2Bitmap(sourceBitmapPicture);
            //for (int i = 0; i < Points.Count; i++)
            //{
            //    int x = (int)Points[i].x;
            //    int y = (int)Points[i].y;
            //    if(x >= 0 && y >= 0 & x < WeightSource && y < HeightSource)
            //        pointsBitmap.SetPixel(x, y, System.Drawing.Color.Red);
            //}
            //sourceImg.Source = Help.Bitmap2BitmapImage(pointsBitmap);
        }

        private void b_triangulation(object sender, RoutedEventArgs e)
        {
            if (debug)
                Console.WriteLine("Начало триангуляции:");

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            //Запуск процесса триангуляции
            Triangulation.Triangulation triangulation = new Triangulation.Triangulation(Points);
            sw.Stop();
            Console.WriteLine("Time Taken-->{0}", sw.ElapsedMilliseconds);

            Triangles = triangulation.triangles;

            pointsBitmap = Helper.BitmapImage2Bitmap(sourceBitmapPicture);

            //Расскрашивание треугольников
            int indexCol = Triangles.Count;
            for (int y = 0; y < indexCol; y++)
            {
                Triangles[y].color = pointsBitmap.GetPixel((int)Triangles[y].Centroid.x, (int)Triangles[y].Centroid.y);
            }

            //Отрисовка триангуляции на экран
            var graphics = Graphics.FromImage(pointsBitmap);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            for (int s = 0; s < Triangles.Count; s++)
            {
                graphics.FillPolygon(new System.Drawing.SolidBrush(Triangles[s].color),
                                        new PointF[] {new PointF((float)Triangles[s].points[0].x, (float)Triangles[s].points[0].y),
                                        new PointF((float)Triangles[s].points[1].x, (float)Triangles[s].points[1].y),
                                        new PointF((float)Triangles[s].points[2].x, (float)Triangles[s].points[2].y)
                                        });
            }

            for (int s = 0; s < Triangles.Count; s++)
            {
                for (int i = 0; i < 3; i++)
                    if (Triangles[s].arcs[i].IsBorder)
                    {
                        graphics.DrawLine(new System.Drawing.Pen(System.Drawing.Color.Red),
                        (float)Triangles[s].arcs[i].A.x, (float)Triangles[s].arcs[i].A.y,
                        (float)Triangles[s].arcs[i].B.x, (float)Triangles[s].arcs[i].B.y);
                    }
                for (int i = 0; i < 3; i++)
                    if (Triangles[s].arcs[i].trAB != null && Triangles[s].arcs[i].trBA != null)
                    {
                        graphics.DrawLine(new System.Drawing.Pen(System.Drawing.Color.Green),
                        (float)Triangles[s].arcs[i].trAB.Centroid.x, (float)Triangles[s].arcs[i].trAB.Centroid.y,
                        (float)Triangles[s].arcs[i].trBA.Centroid.x, (float)Triangles[s].arcs[i].trBA.Centroid.y);
                    }
            }




                sourceImg.Source = Helper.Bitmap2BitmapImage(pointsBitmap);
        }

        private void b_saveAsSVG(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog sdlg = new Microsoft.Win32.SaveFileDialog();
            sdlg.FileName = "Pic"; // Default file name
            sdlg.DefaultExt = ".SVG"; // Default file extension
            sdlg.Filter = "Рисунок SVG (.SVG)|*.svg"; // Filter files by extension
            sdlg.RestoreDirectory = true;

            StreamWriter streamWriter;

            Nullable<bool> resultWr = sdlg.ShowDialog();

            if (resultWr == true)
            {
                try
                {
                    streamWriter = new StreamWriter(sdlg.FileName, false, System.Text.Encoding.UTF8);
                }
                catch
                {
                    return;
                }
                streamWriter.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                streamWriter.WriteLine("<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">");
                streamWriter.WriteLine("<svg version=\"1.1\" id=\"Layer_1\" xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" x=\"0px\" y=\"0px\"");
                streamWriter.WriteLine("    viewBox=\"0 0 " + (int)widthSource + " " + (int)heightSource + "\" enable-background=\"new 0 0 " + (int)widthSource + " " + (int)heightSource + "\" xml:space=\"preserve\">");

                string curColorHEX; 
                for (int i = 0; i < Triangles.Count; i++)
                {
                    curColorHEX = System.Drawing.ColorTranslator.ToHtml(Triangles[i].color);
                    streamWriter.WriteLine("<polygon fill=\"" + curColorHEX + "\"" + " stroke=\"" + curColorHEX + "\"" + " stroke-width=\"0\"" +  " points=\"" 
                        +       Triangles[i].points[0].x.ToString("0.0000000", CultureInfo.GetCultureInfo("en-US")) 
                        + "," + Triangles[i].points[0].y.ToString("0.0000000", CultureInfo.GetCultureInfo("en-US")) 
                        + " " + Triangles[i].points[1].x.ToString("0.0000000", CultureInfo.GetCultureInfo("en-US")) 
                        + "," + Triangles[i].points[1].y.ToString("0.0000000", CultureInfo.GetCultureInfo("en-US")) 
                        + " " + Triangles[i].points[2].x.ToString("0.0000000", CultureInfo.GetCultureInfo("en-US")) 
                        + "," + Triangles[i].points[2].y.ToString("0.0000000", CultureInfo.GetCultureInfo("en-US")) 
                        + "\"/>");
                }


                streamWriter.WriteLine("</svg>");
                streamWriter.Close();
            }
        }

        private void b_saveAsPNG(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog sdlg = new Microsoft.Win32.SaveFileDialog();
            sdlg.FileName = "PicPNG"; // Default file name
            sdlg.DefaultExt = ".PNG"; // Default file extension
            sdlg.Filter = "Рисунок PNG (.PNG)|*.png"; // Filter files by extension
            sdlg.RestoreDirectory = true;

            Nullable<bool> resultWr = sdlg.ShowDialog();

            if (resultWr == true)
            {
                pointsBitmap.Save(sdlg.FileName, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////

        private void b_loadImgs(object sender, RoutedEventArgs e)
        {
            refreshSystem();
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Images (*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|" + "All files (*.*)|*.*";
            dlg.Multiselect = true;

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                foreach (String file in dlg.FileNames)
                {
                    try
                    {
                        sourceBitmapPictures.Add(new BitmapImage(new Uri(file)));
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }

                if (sourceBitmapPictures[0] != null)
                {
                    sourceImg.Source = sourceBitmapPictures[0];
                }
            }
        }

        private void b_loadPoints2(object sender, RoutedEventArgs e)
        {
            if (debug)
                Console.WriteLine("Создание точек");

            pointsCount = Int32.Parse(tbPoints2.Text);

            if (sourceBitmapPictures[0] != null)
            {
                widthSource = (float)(sourceBitmapPictures[0].PixelWidth);
                heightSource = (float)(sourceBitmapPictures[0].PixelHeight);
            }

            //Отрисовка карты точек первого рисунка
            if (debug)
                Console.WriteLine("Отрисовка точек");

            Bitmap pBitmapForPoints = Helper.BitmapImage2Bitmap(sourceBitmapPictures[0]);
            var graphics = Graphics.FromImage(pBitmapForPoints);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            for (int i = 0; i < Points.Count; i++)
            {
                graphics.FillEllipse(new System.Drawing.SolidBrush(System.Drawing.Color.SkyBlue), (int)Points[i].x, (int)Points[i].y, 3, 3);
            }
            sourceImg.Source = Helper.Bitmap2BitmapImage(pBitmapForPoints);
        }

        private void b_triangulation2(object sender, RoutedEventArgs e)
        {
            if (debug)
                Console.WriteLine("Начало триангуляции:");

            //Запуск процесса триангуляции
            pointsBitmap = Helper.BitmapImage2Bitmap(sourceBitmapPictures[0]);


            //Расскрашивание треугольников
            int indexCol = Triangles.Count;
            for (int y = 0; y < indexCol; y++)
            {
                Triangles[y].color = pointsBitmap.GetPixel((int)Triangles[y].Centroid.x, (int)Triangles[y].Centroid.y);
            }


            //Отрисовка триангуляции на экран
            var graphics = Graphics.FromImage(pointsBitmap);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            for (int s = 0; s < Triangles.Count; s++)
            {
                graphics.FillPolygon(new System.Drawing.SolidBrush(Triangles[s].color),
                                        new PointF[] {new PointF((float)Triangles[s].points[0].x, (float)Triangles[s].points[0].y),
                                        new PointF((float)Triangles[s].points[1].x, (float)Triangles[s].points[1].y),
                                        new PointF((float)Triangles[s].points[2].x, (float)Triangles[s].points[2].y)
                                        });
            }
            sourceImg.Source = Helper.Bitmap2BitmapImage(pointsBitmap);

            if (debug)
            {
                Console.WriteLine("100,00%");
                Console.WriteLine("Конец триангуляции:");
            }
        }

        private void b_saveAsPNGs(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog sdlg = new Microsoft.Win32.SaveFileDialog();
            sdlg.FileName = "PicPNG"; // Default file name
            sdlg.Filter = "Рисунок PNG (.PNG)|*.png"; // Filter files by extension
            sdlg.RestoreDirectory = true;

            Nullable<bool> resultWr = sdlg.ShowDialog();

            if (resultWr == true)
            {
                if (debug)
                    Console.WriteLine("Начало сохранения файлов");

                for (int i = 0; i < sourceBitmapPictures.Count; i++)
                {
                    if (debug)
                        Console.WriteLine(i + "/" + sourceBitmapPictures.Count);

                    pointsBitmap = Helper.BitmapImage2Bitmap(sourceBitmapPictures[i]);

                    //Расскрашивание треугольников
                    int indexCol = Triangles.Count;
                    for (int y = 0; y < indexCol; y++)
                    {
                        Triangles[y].color = pointsBitmap.GetPixel((int)Triangles[y].Centroid.x, (int)Triangles[y].Centroid.y);

                        ////Смещение точек триангуляции
                        //Triangles[y].points[0] += new Vector2(Help.RndRangeFloat(-5.05f, 5.05f), Help.RndRangeFloat(-5.05f, 5.05f));
                        //Triangles[y].points[1] += new Vector2(Help.RndRangeFloat(-5.05f, 5.05f), Help.RndRangeFloat(-5.05f, 5.05f));
                        //Triangles[y].points[2] += new Vector2(Help.RndRangeFloat(-5.05f, 5.05f), Help.RndRangeFloat(-5.05f, 5.05f));
                    }

                    //Отрисовка триангуляции в буфер
                    Graphics graphics = Graphics.FromImage(pointsBitmap);
                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    for (int s = 0; s < Triangles.Count; s++)
                    {
                        graphics.FillPolygon(new System.Drawing.SolidBrush(Triangles[s].color),
                                                new PointF[] {new PointF((float)Triangles[s].points[0].x, (float)Triangles[s].points[0].y),
                                        new PointF((float)Triangles[s].points[1].x, (float)Triangles[s].points[1].y),
                                        new PointF((float)Triangles[s].points[2].x, (float)Triangles[s].points[2].y)
                                                });
                    }

                    //Сохранение буфера
                    pointsBitmap.Save(sdlg.FileName.Substring(0, sdlg.FileName.Length - 4) + i + ".png", System.Drawing.Imaging.ImageFormat.Png);

                    graphics.Dispose();
                    pointsBitmap.Dispose();
                    System.GC.Collect();
                }
            }
        }
    }
}
