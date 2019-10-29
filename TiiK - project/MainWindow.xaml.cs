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

namespace TiiK___project
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow() {
            InitializeComponent();
        }

        private void button_OpenFileDialog_Click(object sender, RoutedEventArgs e) {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".txt";
            dlg.Filter = "TXT Files (*.txt)|*txt";
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true) {
                string filename = dlg.FileName;
                textBox_FileLocation.Text = filename;
            }
        }

        private void button_Analyze_Click(object sender, RoutedEventArgs e) {
            Console.WriteLine();
            var data = new List<AnalyzedData>();

            data = QuantityCounter.CountQuantities(textBox_FileLocation.Text);
            data.Sort((x, y) => y.Quantity.CompareTo(x.Quantity));
            dataGrid_AnalyzeResults.ItemsSource = data;
        }


    }

    public struct AnalyzedData
    {
        public char Character { set; get; }
        public double Count { set; get; }
        public double Probability { set; get; }
        public double Quantity { set; get; }
    }
}
