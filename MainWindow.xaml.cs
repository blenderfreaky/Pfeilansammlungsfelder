using MaterialDesign2.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
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
using Expression = NCalc2.Expression;

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

            /*pan.MouseWheel += (_, __) => DrawLines();
            pan.MouseLeftButtonDown += (_, __) => DrawLines();
            pan.MouseLeftButtonUp += (_, __) => DrawLines();
            pan.MouseMove += (_, __) => DrawLines();
            pan.PreviewMouseRightButtonDown += (_, __) => DrawLines();*/
            eval.Click += (_, __) => { try { DrawLines(null, null, 1, true, true); } catch (Exception) { } };

            DrawLines(new Point(-1000, -1000), new Point(1000, 1000), 1, true, true);
        }

        public void DrawLines(Point? Min = null, Point? Max = null, double radius = 1, bool drawArrows = true, bool grid = false, bool drawApprox = true)
        {
            var translate = pan.translateTransform;
            var scale = pan.scaleTransform;

            Point min = Min ?? plot.PointFromScreen(new Point(-this.Width * radius, -this.Height * radius));
            Point max = Max ?? plot.PointFromScreen(new Point(this.Width * (1+radius), this.Height * (1 + radius)));
            (min.X, max.X) = (Math.Min(min.X, max.X), Math.Max(min.X, max.X));
            (min.Y, max.Y) = (Math.Min(min.Y, max.Y), Math.Max(min.Y, max.Y));
            Vector size = max - min;

            double length = 100;
            double step = Math.Pow(10, -(int)Math.Log10(scale.ScaleX)) * length;
            double stroke = step / 10;
            double thick = step / 120;

            Point roundedMin0 = new Point(step * (int)(min.X / step), step * (int)(min.Y / step));
            Point roundedMin1 = new Point(step / 10 * (int)(min.X / step * 10), step / 10 * (int)(min.Y / step * 10));

            Expression expression = new Expression(exp.Text);
            double func(double x, double y)
            {
                expression.Parameters["x"] = x;
                expression.Parameters["y"] = y;
                return Convert.ToDouble(expression.Evaluate());
            }

            if (drawArrows)
            {
                arrows.Children.Clear();

                double f = Convert.ToDouble(scaler.Text);
                step /= f;
                for (int i = -1; i <= size.X / step; i++)
                {
                    for (int j = -1; j <= size.Y / step; j++)
                    {
                        double val = func((roundedMin0.X + i * step) / length, (roundedMin0.Y + j * step) / length);
                        if (double.IsNaN(val)) continue;
                        double atan = Math.Atan(val);
                        Vector direction = new Vector(Math.Cos(atan), Math.Sin(atan)) * step * 0.9;
                        Vector directionCrossed = new Vector(-direction.Y, direction.X);
                        arrows.Children.Add(new Line
                        {
                            X1 = roundedMin0.X + i * step,
                            X2 = roundedMin0.X + i * step + direction.X / f,
                            Y1 = roundedMin0.Y + j * step,
                            Y2 = roundedMin0.Y + j * step + direction.Y / f,
                            Stroke = Brushes.Blue,
                            StrokeThickness = thick * 1
                        });
                        double a = 0.8, b = 0.15;
                        arrows.Children.Add(new Line
                        {
                            X1 = roundedMin0.X + i * step + (a * direction.X + b * directionCrossed.X) / f,
                            X2 = roundedMin0.X + i * step + direction.X / f,
                            Y1 = roundedMin0.Y + j * step + (a * direction.Y + b * directionCrossed.Y) / f,
                            Y2 = roundedMin0.Y + j * step + direction.Y / f,
                            Stroke = Brushes.Blue,
                            StrokeThickness = thick * 1
                        });
                        arrows.Children.Add(new Line
                        {
                            X1 = roundedMin0.X + i * step + (a * direction.X - b * directionCrossed.X) / f,
                            X2 = roundedMin0.X + i * step + direction.X / f,
                            Y1 = roundedMin0.Y + j * step + (a * direction.Y - b * directionCrossed.Y) / f,
                            Y2 = roundedMin0.Y + j * step + direction.Y / f,
                            Stroke = Brushes.Blue,
                            StrokeThickness = thick * 1
                        });
                    }
                }
                step *= f;
            }

            if (drawApprox)
            {
                approx.Children.Clear();

                Point start = new Point(Convert.ToDouble(staX.Text), Convert.ToDouble(staY.Text));
                double currentAccuracy = Convert.ToDouble(acc.Text);

                void DrawApprox(double accuracy)
                {
                    for (double x = start.X, y = start.Y; (accuracy < 0 || x <= max.X / length) && (accuracy > 0 || x >= min.X / length) && y <= max.Y / length && y >= min.Y / length;)
                    {
                        double val = func(x, y);
                        if (double.IsNaN(val)) continue;

                        (double oldX, double oldY) = (x, y);
                        double atan = Math.Atan(val);
                        x += Math.Cos(atan) * accuracy;
                        y += Math.Sin(atan) * accuracy;

                        if (x <= max.X / length && y <= max.Y / length && x >= min.X / length && y >= min.Y / length)
                        {
                            approx.Children.Add(new Line
                            {
                                X1 = oldX * length,
                                Y1 = oldY * length,
                                X2 = x * length,
                                Y2 = y * length,
                                Stroke = Brushes.Red,
                                StrokeThickness = thick * 2
                            });
                        }
                    }
                }
                DrawApprox(currentAccuracy);
                DrawApprox(-currentAccuracy);
            }

            if (grid)
            {
                thick /= 1.2;
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

                if (100 > 10)
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
                }
            }
        }
    }
}
