using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkingHeads.DataStructures
{
    public class LexiconAssocation
    {
        public string TreeDiscriminant { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
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
            TreeDiscriminant = treeDiscriminant;
            MinValue = minValue;
            MaxValue = maxValue;
            StringValue = "[" + treeDiscriminant + " " + minValue + "-" + maxValue + "]";
        }

        public string Value()
        {
            return StringValue;
        }

        public override string ToString()
        {
            string response = "";
            int counter = 0;
            foreach (KeyValuePair<string, uint> item in Words)
            {
                response += item.Key + Configuration.Separator + item.Value + (counter++ < Words.Count()-1 ? "" + Configuration.Separator : "");
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

        public void AddWordOrAddScore(string word)
        {
            if (!Words.ContainsKey(word))
            {
                Words.Add(word, Configuration.Word_Score_Update_When_Correct_Form_Word_Unknown);
            }
            else
            {
                Words[word] += Configuration.Word_Score_Update_When_Correct_Form;
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

        private void AddWordScore(string word)
        {
            if (Words.ContainsKey(word))
            {
                Words[word] += Configuration.Word_Score_Update_When_Correct;
            }
        }

        private void DecreaseWordScore(string word, uint scoreToRemove)
        {
            if (Words.ContainsKey(word))
            {
                if (Words[word] > scoreToRemove)
                {
                    Words[word] -= scoreToRemove;
                    if (Words[word] <= Configuration.Word_Score_To_Trim)
                    {
                        Words.Remove(word);
                    }
                }
                else
                {
                    Words.Remove(word);
                }
            }
        }

        public void CorrectGuess(string word)
        {
            AddWordScore(word);
        }

        public void IncorrectGuess(string word)
        {
            DecreaseWordScore(word, Configuration.Word_Score_Update_When_Incorrect);
        }

        public void AnotherNodeIsCorrect(string word) // used when the word is deemed correct in another node
        {
            DecreaseWordScore(word, Configuration.Word_Score_Update_When_Other_Correct);
        }

        public uint GetScore(string word)
        {
            if (Words.ContainsKey(word))
            {
                return Words[word];
            }
            return 0;
        }
    }
}
