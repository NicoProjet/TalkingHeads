using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkingHeads.DataStructures
{
    public class LexiconAssocation
    {
        public string StringValue { get; set; }
        public Dictionary<string, uint> Words { get; set; }

        private void Init()
        {
            Words = new Dictionary<string, uint>();
        }

        public LexiconAssocation()
        {
            Init();
        }

        public LexiconAssocation(string treeDiscriminant, double minValue, double maxValue)
        {
            Init();
            StringValue = "[" + treeDiscriminant + " " + minValue + "-" + maxValue + "]";
        }

        public string Value()
        {
            return StringValue;
        }

        public override string ToString()
        {
            string response = "";
            foreach (KeyValuePair<string, uint> item in Words)
            {
                response += item.Key + Configuration.Separator + item.Value + Configuration.Separator;
            }
            return response;
        }

        public void AddWord(string word, uint score = 0)
        {
            if (score == 0) score = Configuration.Word_Default_Score;
            if (!Words.ContainsKey(word))
            {
                Words.Add(word, score);
            }
        }

        public string GetWord()
        {
            string result = "";
            uint bestScore = 0;
            foreach (KeyValuePair<string, uint> item in Words)
            {
                if (item.Value > bestScore)
                {
                    bestScore = item.Value;
                    result = item.Key;
                }
            }
            return result;
        }

        private void AddToDictionary(Dictionary<string, uint> dictionary, KeyValuePair<string, uint> item)
        {
            if (dictionary.ContainsKey(item.Key))
            {
                dictionary[item.Key] += item.Value;
            }
            else
            {
                dictionary.Add(item.Key, item.Value);
            }
        }

        public void MergeInto(LexiconAssocation other)
        {
            foreach(KeyValuePair<string, uint> item in Words)
            {
                AddToDictionary(other.Words, item);
            }
        }
    }
}
