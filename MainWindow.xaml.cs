using MaterialDesign2.Controls;
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

namespace Pfeilansammlungsfelder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MaterialWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            pan.MouseWheel += (_, __) => DrawLines();
            pan.MouseLeftButtonDown += (_, __) => DrawLines();
            pan.MouseLeftButtonUp += (_, __) => DrawLines();
            pan.MouseMove += (_, __) => DrawLines();
            pan.PreviewMouseRightButtonDown += (_, __) => DrawLines();
        }

        public void DrawLines()
        {
            var translate = pan.translateTransform;
            var scale = pan.scaleTransform;

            Point min = plot.PointFromScreen(new Point(0, 0));
            Point max = plot.PointFromScreen(new Point(this.Width*2, this.Height*2));
            Vector size = max - min;

            double length = 100;
            double step = Math.Pow(10, -(int)Math.Log10(scale.ScaleX)) * length;
            double stroke = step / 10;
            double thick = step/120;

            Point roundedMin0 = new Point(step * (int)(min.X / step), step * (int)(min.Y / step));
            Point roundedMin1 = new Point(step / 10 * (int)(min.X / step * 10), step / 10 * (int)(min.Y / step * 10));

            arrows.Children.Clear();

            Func<double, double, double> func = (x,y) => 3 * x*y;

            for (int i = -1; i <= size.X / step; i++)
            {
                for (int j = -1; j <= size.Y / step; j++)
                {
                    double val = func((roundedMin0.X + i * step) / length, (roundedMin0.Y + j * step) / length);
                    double atan = Math.Atan(val);
                    Vector direction = new Vector(Math.Cos(atan), Math.Sin(atan)) * step * 0.8;
                    Vector directionCrossed = new Vector(-direction.Y, direction.X);
                    arrows.Children.Add(new Line
                    {
                        X1 = roundedMin0.X + i * step,
                        X2 = roundedMin0.X + i * step + direction.X,
                        Y1 = roundedMin0.Y + j * step,
                        Y2 = roundedMin0.Y + j * step + direction.Y,
                        Stroke = Brushes.Red,
                        StrokeThickness = thick * 2
                    });
                    double a = 0.8, b = 0.15;
                    arrows.Children.Add(new Line
                    {
                        X1 = roundedMin0.X + i * step + a * direction.X + b * directionCrossed.X,
                        X2 = roundedMin0.X + i * step + direction.X,
                        Y1 = roundedMin0.Y + j * step + a * direction.Y + b * directionCrossed.Y,
                        Y2 = roundedMin0.Y + j * step + direction.Y,
                        Stroke = Brushes.Red,
                        StrokeThickness = thick * 2
                    });
                    arrows.Children.Add(new Line
                    {
                        X1 = roundedMin0.X + i * step + a * direction.X - b * directionCrossed.X,
                        X2 = roundedMin0.X + i * step + direction.X,
                        Y1 = roundedMin0.Y + j * step + a * direction.Y - b * directionCrossed.Y,
                        Y2 = roundedMin0.Y + j * step + direction.Y,
                        Stroke = Brushes.Red,
                        StrokeThickness = thick * 2
                    });
                }
            }
            /*
            markings.Children.Clear();

            for (int i = -1; i < size.X / step + 1; i++) markings.Children.Add(new Line
            {
                X1 = roundedMin0.X + i * step,
                X2 = roundedMin0.X + i * step,
                Y1 = min.Y,
                Y2 = max.Y,
                Stroke = Brushes.Black,
                StrokeThickness = thick,
            });
            for (int j = -1; j < size.Y / step + 1; j++) markings.Children.Add(new Line
            {
                Y1 = roundedMin0.Y + j * step,
                Y2 = roundedMin0.Y + j * step,
                X1 = min.X,
                X2 = max.X,
                Stroke = Brushes.Black,
                StrokeThickness = thick,
            });

            if (size.X / step < 10 && size.Y / step < 10)
            {
                for (int i = -1; i < (size.X / step + 1) * 10; i++) markings.Children.Add(new Line
                {
                    X1 = roundedMin1.X + i * step / 10,
                    X2 = roundedMin1.X + i * step / 10,
                    Y1 = min.Y,
                    Y2 = max.Y,
                    Stroke = Brushes.Black,
                    StrokeThickness = thick / 10,
                });
                for (int j = -1; j < (size.Y / step + 1) * 10; j++) markings.Children.Add(new Line
                {
                    Y1 = roundedMin1.Y + j * step / 10,
                    Y2 = roundedMin1.Y + j * step / 10,
                    X1 = min.X,
                    X2 = max.X,
                    Stroke = Brushes.Black,
                    StrokeThickness = thick / 10,
                });
            }*/
        }
    }
}
