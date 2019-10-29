using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TiiK___project
{
    class QuantityCounter
    {
        public static List<AnalyzedData> CountQuantities(string filepath) {
            List<AnalyzedData> ListOfAll = new List<AnalyzedData>();
            string text;
            try
            {
            text = System.IO.File.ReadAllText(filepath);
           
            var map = new Dictionary<char, double>();
            for (int i = 0; i < text.Length; i++) {
                char c = text[i];
                if (map.ContainsKey(text[i])) {
                    map[c]++;
                } else {
                    int ascii = (int)c;
                    if ((ascii >= 65 && ascii <= 90) || (ascii >= 97 && ascii <= 122) || (ascii >= 200 && ascii <= 400)) {
                        map.Add(c, 1);
                    }
                }
            }

            foreach (KeyValuePair<char, double> letter in map) {
                double probability = CountProbability(letter.Value, text.Length);
                double quantity;
                if (probability == 0)
                {
                    quantity = 0;
                }
                else
                {
                    quantity = GetQuantity(probability);
                }
                AnalyzedData pom = new AnalyzedData { Character = letter.Key, Count = letter.Value, Probability = probability, Quantity = quantity };
                ListOfAll.Add(pom);
            }
            
            }
            catch (FileNotFoundException er)
            {
                MessageBox.Show("File with specifed path not exists.");

            }
            return ListOfAll;
        }

        static double CountProbability(double letterCount, double textLength) {
            return Math.Round(letterCount / textLength, 4);
        }

        static double GetQuantity(double probability) {
            return Math.Round(Math.Log(2, 1 / probability), 4);
        }
    }
}
