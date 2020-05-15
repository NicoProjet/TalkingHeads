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
        public Dictionary<string, uint> WordsTempAfterSplit { get; set; }
        public Dictionary<string, uint> LeftWords { get; set; }
        public Dictionary<string, uint> RightWords { get; set; }

        private void Init()
        {
            LeftWords = new Dictionary<string, uint>();
            RightWords = new Dictionary<string, uint>();
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

        public override string ToString()
        {
            return StringValue;
        }

        public void AddLeftWord(string word)
        {
            if (!LeftWords.ContainsKey(word))
            {
                LeftWords.Add(word, Configuration.Word_Default_Score);
            }
        }

        public void AddRightWord(string word)
        {
            if (!RightWords.ContainsKey(word))
            {
                RightWords.Add(word, Configuration.Word_Default_Score);
            }
        }

        public string GetWord()
        {
            Dictionary<string, uint> words = new Dictionary<string, uint>();
            foreach(KeyValuePair<string, uint> item in LeftWords)
            {
                words.Add(item.Key, item.Value);
            }
            foreach(KeyValuePair<string, uint> item in RightWords)
            {
                if (words.ContainsKey(item.Key))
                {
                    words[item.Key] += item.Value;
                }
                else
                {
                    words.Add(item.Key, item.Value);
                }
            }

            string result = "";
            uint bestScore = 0;
            foreach (KeyValuePair<string, uint> item in words)
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

        public void MergeInto(LexiconAssocation other, bool isLeftSon)
        {
            foreach(KeyValuePair<string, uint> item in LeftWords)
            {
                if (isLeftSon)
                {
                    AddToDictionary(other.LeftWords, item);
                }
                else
                {
                    AddToDictionary(other.RightWords, item);
                }
            }

            foreach (KeyValuePair<string, uint> item in RightWords)
            {
                if (isLeftSon)
                {
                    AddToDictionary(other.LeftWords, item);
                }
                else
                {
                    AddToDictionary(other.RightWords, item);
                }
            }
        }
    }
}
