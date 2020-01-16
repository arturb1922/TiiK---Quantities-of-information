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
        private List<AnalyzedData> currentQuantities;
        public MainWindow() {
            InitializeComponent();
        }

        private void Analyze() {
            var data = new List<AnalyzedData>();
            data = QuantityCounter.CountQuantities(textBox_FileLocation.Text);
            currentQuantities = data;

            data.Sort((x, y) => y.Quantity.CompareTo(x.Quantity));
            dataGrid_AnalyzeResults.ItemsSource = data;

            double Entropy = new double();
            foreach (AnalyzedData d in data) {
                Entropy += d.Probability * Math.Log((1 / d.Probability), 2);
            }
            Entropy = Math.Round(Entropy, 5);
            textBox_Entropy.Text = Entropy.ToString();
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
            Analyze();
        }

        private void button_Encode_Click(object sender, RoutedEventArgs e) {
            Analyze();
            ShannonFano.Encode(textBox_FileLocation.Text, currentQuantities);
        }

        private void button_Decode_Click(object sender, RoutedEventArgs e)
        {
            ShannonFano.Decode(textBox_FileLocation.Text);
        }
    }

    public struct AnalyzedData
    {
        public String Hex { set; get; }
        public char Character { set; get; }
        public double Count { set; get; }
        public double Probability { set; get; }
        public double Quantity { set; get; }
    }
}
