using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TiiK___project
{
    class ShannonFano
    {

        private static void Shannon(List<AnalyzedData> statistics, Dictionary<char, String> refer)
        {
            if (statistics.Count == 0)
            {
                return;
            }
            if (statistics.Count == 1)
            {
                refer[statistics[0].Character] += "0";
                return;
            }
            if (statistics.Count == 2)
            {
                refer[statistics[0].Character] += "0";
                refer[statistics[1].Character] += "1";
                return;
            }
            else
            {
                var splitted = split(statistics);

                if (splitted.Item1.Count > 1)
                {
                    foreach (var x in splitted.Item1)
                    {
                        refer[x.Character] += "0";
                    }
                }
                if (splitted.Item2.Count > 1)
                {
                    foreach (var x in splitted.Item2)
                    {
                        refer[x.Character] += "1";
                    }
                }

                Shannon(splitted.Item1, refer);
                Shannon(splitted.Item2, refer);
                return;
            }
        }


        public static void Encode(string filepath, List<AnalyzedData> statistics)
        {
            //wczytac dane z pliku, zakodowac z pomoca statistics (Character, Probability), zwrocic TablicęKodowania i ZakodowanąWiadomość
            string input = "";
            try
            {
                input = System.IO.File.ReadAllText(filepath);

            }
            catch (FileNotFoundException er)
            {
                Console.WriteLine(er);
                MessageBox.Show("File with specifed path not exists.");
            }

            statistics.Sort((x, y) => y.Probability.CompareTo(x.Probability));

            var dictionary = new Dictionary<char, String>();
            foreach (var s in statistics)
            {
                dictionary[s.Character] = "";
            }
            Shannon(statistics, dictionary);

            foreach (var key in dictionary.Keys)
            {
                Console.WriteLine(key + "\t" + dictionary[key]);
            }

            string binaryString = "";
            foreach (char c in input)
            {
                binaryString += dictionary[c];
                Console.WriteLine(binaryString);
            }

            var bits = new BitArray(binaryString.Length);
            for (int i = 0; i < binaryString.Length; i++)
            {
                if (binaryString[i] == '0')
                {
                    bits[i] = false;
                }
                if (binaryString[i] == '1')
                {
                    bits[i] = true;
                }
            }
            Console.WriteLine(bits.Length);
            byte[] bytes = new byte[bits.Length / 8 + 1];
            Console.WriteLine(bytes.Length);
            bits.CopyTo(bytes, 0);





            FileStream stream;
            BinaryWriter writer;
            using (stream = new FileStream(Path.GetDirectoryName(filepath)+'\\' + Path.GetFileNameWithoutExtension(filepath) + "_output.txt", FileMode.Create))
            {
                using (writer = new BinaryWriter(stream))
                {
                    writer.Write(bytes);
                    writer.Close();

                }

            }


            BinaryFormatter formatter = new BinaryFormatter();
            System.IO.Stream ms = File.OpenWrite(Path.GetDirectoryName(filepath)+'\\' + Path.GetFileNameWithoutExtension(filepath) + "_output_stats.txt");
            formatter.Serialize(ms, dictionary);

            long f0 = new System.IO.FileInfo(filepath).Length;
            long f1 = new System.IO.FileInfo(Path.GetDirectoryName(filepath)+'\\' + Path.GetFileNameWithoutExtension(filepath) + "_output.txt").Length;
            long f2 = new System.IO.FileInfo(Path.GetDirectoryName(filepath)+'\\' + Path.GetFileNameWithoutExtension(filepath) + "_output_stats.txt").Length;

            MessageBox.Show("Compression ratio: " + f0 * 1.0 / (f1 + f2) * 1.0);
            ms.Close();
            stream.Close();
            writer.Close();

        }

        private static Tuple<List<AnalyzedData>, List<AnalyzedData>> split(List<AnalyzedData> input)
        {
            double sum = 0;
            foreach (var p in input)
            {
                sum += p.Probability;
            }

            int separator = 0;
            double p1 = 0;
            double p2;
            double old_dif = 1;
            foreach (var p in input)
            {
                p1 += p.Probability;
                p2 = sum - p1;
                var dif = Math.Abs(p1 - p2);
                if (dif >= old_dif)
                {
                    break;
                }
                old_dif = dif;
                separator++;
            }
            return new Tuple<List<AnalyzedData>, List<AnalyzedData>>(input.GetRange(0, separator), input.GetRange(separator, input.Count - separator));
        }

        public static string Dekode(string bitTekst, Dictionary<char, string> dictionary)
        {
            var convertedDictionary = ConvertDictionary(dictionary);
            string code = string.Empty;
            string output = string.Empty;
            for (int i = 0; i < bitTekst.Length; i++)
            {
                code += bitTekst[i];
                if (convertedDictionary.TryGetValue(code, out var value))
                {
                    output += value;
                    code = string.Empty;
                }
            }

            return output;
        }

        public static Dictionary<string, char> ConvertDictionary(Dictionary<char, string> dictionary)
        {
            var output = new Dictionary<string, char>();
            foreach (var pair in dictionary)
            {
                output.Add(pair.Value, pair.Key);
            }

            return output;
        }

        public static void Decode(string filepath)
        {

            BitArray bits;
            string stream1 = Path.GetDirectoryName(filepath)+'\\' + Path.GetFileNameWithoutExtension(filepath) + "_output.txt";
            string stream2 = Path.GetDirectoryName(filepath)+'\\' + Path.GetFileNameWithoutExtension(filepath) + "_output_stats.txt";

            System.IO.Stream ms = File.OpenRead(stream2);
            var dictionary = new Dictionary<char, String>();

            BinaryReader binReader = new BinaryReader(File.Open(stream1, FileMode.Open, FileAccess.Read));
            long f1 = new System.IO.FileInfo(Path.GetDirectoryName(filepath)+'\\' + Path.GetFileNameWithoutExtension(filepath) + "_output.txt").Length;
            byte[] writeArray = new byte[f1];
            //wczytywanie słownika i deserializacja
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                dictionary = (Dictionary<char, String>)formatter.Deserialize(ms);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                ms.Close();
            }
            foreach (var key in dictionary.Keys)
            {
                Console.WriteLine(key + "\t" + dictionary[key]);
            }

            string tekst = "";
            //wczytywanie tekstu 
            for (int i = 0; i < f1; i++)
            {
                writeArray[i] = binReader.ReadByte();
            }
            for (int i = 0; i < f1; i++)
            {
                tekst += Convert.ToString(writeArray[i]);
            }
            bits = new BitArray(writeArray);


            string bitTekst = "";
            for (int i = 0; i < bits.Length; i++)
            {
                if (bits[i] == false)
                {
                    bitTekst += 0;
                }
                if (bits[i] == true)
                {
                    bitTekst += 1;
                }
            }
            Console.WriteLine(bitTekst);

            string output = string.Empty;

            output = Dekode(bitTekst, dictionary);



            Console.WriteLine(output);
            System.IO.File.WriteAllText(Path.GetDirectoryName(filepath)+'\\' + Path.GetFileNameWithoutExtension(filepath) + "_decoding.txt", output);
            binReader.Close();
            ms.Close();


        }

    }
}
