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
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            int val = 50;
            int size = 10;
            arrows.ItemsSource = Enumerable.Range(-val, 2 * val + 1).Select(x => new Line { X1 = size * x, X2 = size * x, Y1 = -1, Y2 = 1, Stroke = Brushes.Black }).Concat(
                                 Enumerable.Range(-val, 2 * val + 1).Select(x => new Line { Y1 = size * x, Y2 = size * x, X1 = -1, X2 = 1, Stroke = Brushes.Black }));
        }
    }
}
