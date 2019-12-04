using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TiiK___project
{
    class ShannonFano
    {

        private static void Shannon(List<AnalyzedData> statistics, Dictionary<char, String> refer) {
            if (statistics.Count == 0) {
                return;
            }
            if (statistics.Count == 1) {
                refer[statistics[0].Character] += "0";
                return;
            }
            if (statistics.Count == 2) {
                refer[statistics[0].Character] += "0";
                refer[statistics[1].Character] += "1";
                return;
            } else {
                var splitted = split(statistics);

                if (splitted.Item1.Count > 1) {
                    foreach (var x in splitted.Item1) {
                        refer[x.Character] += "0";
                    }
                }
                if (splitted.Item2.Count > 1) {
                    foreach (var x in splitted.Item2) {
                        refer[x.Character] += "1";
                    }
                }

                Shannon(splitted.Item1, refer);
                Shannon(splitted.Item2, refer);
                return;
            }
        }


        public static void Encode(string filepath, List<AnalyzedData> statistics) {
            //wczytac dane z pliku, zakodowac z pomoca statistics (Character, Probability), zwrocic TablicęKodowania i ZakodowanąWiadomość
            string input = "";
            try {
                input = System.IO.File.ReadAllText(filepath);

            } catch (FileNotFoundException er) {
                Console.WriteLine(er);
                MessageBox.Show("File with specifed path not exists.");
            }

            statistics.Sort((x, y) => y.Probability.CompareTo(x.Probability));

            var dictionary = new Dictionary<char, String>();
            foreach (var s in statistics) {
                dictionary[s.Character] = "";
            }
            Shannon(statistics, dictionary);

            //foreach (var key in dictionary.Keys) {
            //    Console.WriteLine(key + "\t" + dictionary[key]);
            //}

            string binaryString = "";
            foreach (char c in input) {
                binaryString += dictionary[c];
            }

            var bits = new BitArray(binaryString.Length);
            for (int i = 0; i < binaryString.Length; i++) {
                if (binaryString[i] == '0') {
                    bits[i] = false;
                }
                if (binaryString[i] == '1') {
                    bits[i] = true;
                }
            }

            byte[] bytes = new byte[bits.Length/8+1];
            bits.CopyTo(bytes, 0);

            using (FileStream stream = new FileStream("C:\\Users\\fpietraszak\\Documents\\output.txt", FileMode.Create)) {
                using (BinaryWriter writer = new BinaryWriter(stream)) {
                    writer.Write(bytes);
                    writer.Close();
                }
            }

            BinaryFormatter formatter = new BinaryFormatter();
            System.IO.Stream ms = File.OpenWrite("C:\\Users\\fpietraszak\\Documents\\output_stats.txt");
            formatter.Serialize(ms, dictionary);

            long f0 = new System.IO.FileInfo(filepath).Length;
            long f1 = new System.IO.FileInfo("C:\\Users\\fpietraszak\\Documents\\output.txt").Length;
            long f2 = new System.IO.FileInfo("C:\\Users\\fpietraszak\\Documents\\output_stats.txt").Length;

            MessageBox.Show("Compression ratio: " + f0*1.0/(f1+f2)*1.0);

        }

        private static Tuple<List<AnalyzedData>, List<AnalyzedData>> split(List<AnalyzedData> input) {
            double sum = 0;
            foreach (var p in input) {
                sum += p.Probability;
            }

            int separator = 0;
            double p1 = 0;
            double p2;
            double old_dif = 1;
            foreach (var p in input) {
                p1 += p.Probability;
                p2 = sum - p1;
                var dif = Math.Abs(p1 - p2);
                if (dif >= old_dif) {
                    break;
                }
                old_dif = dif;
                separator++;
            }
            return new Tuple<List<AnalyzedData>, List<AnalyzedData>>(input.GetRange(0, separator), input.GetRange(separator, input.Count - separator));
        }
        public static void Decode() {

        }
    }
}
