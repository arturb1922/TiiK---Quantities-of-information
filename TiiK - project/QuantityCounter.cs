using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiiK___project
{
    class QuantityCounter
    {
        public static List<AnalyzedData> CountQuantities(string filepath) {
            List<AnalyzedData> ListOfAll = new List<AnalyzedData>();
            string text = System.IO.File.ReadAllText(filepath);
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
                double quantity = GetQuantity(probability);
                AnalyzedData pom = new AnalyzedData { Character = letter.Key, Count = letter.Value, Probability = probability, Quantity = quantity };
                ListOfAll.Add(pom);
            }
            return ListOfAll;
        }

        static double CountProbability(double letterCount, double textLength) {
            return Math.Round(letterCount / textLength, 3);
        }

        static double GetQuantity(double probability) {
            return Math.Round(Math.Log(2, 1 / probability), 3);
        }
    }
}
