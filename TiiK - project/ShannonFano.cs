using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TiiK___project
{
    class ShannonFano
    {
        private void Shannon() {


        }
        public static void Encode(string filepath, List<AnalyzedData> statistics) {
            //wczytac dane z pliku, zakodowac z pomoca statistics (Character, Probability), zwrocic TablicęKodowania i ZakodowanąWiadomość
            string input;
            try {
                input = System.IO.File.ReadAllText(filepath);

            } catch (FileNotFoundException er) {
                Console.WriteLine(er);
                MessageBox.Show("File with specifed path not exists.");
            }

            statistics.Sort((x, y) => y.Probability.CompareTo(x.Probability));

            foreach (var x in statistics) {
                Console.WriteLine(x.Hex + "\t" + x.Probability);
            }

            var test = split(statistics);
            foreach (var x in test.Item1) {
                Console.WriteLine(x.Probability);
            }
            Console.WriteLine();

            foreach (var x in test.Item2) {
                Console.WriteLine(x.Probability);
            }
        }

        private static Tuple<List<AnalyzedData>, List<AnalyzedData>> split(List<AnalyzedData> input) {
            double sum = 0;
            foreach (var p in input) {
                sum += p.Probability;
            }

            int separator = 1;
            double temp = 0;

            foreach (var p in input) {
                temp += p.Probability;
                if (temp >= sum - temp) {
                    break;
                }
                separator++;
            }
            return new Tuple<List<AnalyzedData>, List<AnalyzedData>>(input.GetRange(0, separator), input.GetRange(separator, input.Count-1));
        }
        public static void Decode() {

        }
    }
}
