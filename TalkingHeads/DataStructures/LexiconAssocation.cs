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
        public Dictionary<string, uint> StepInactives { get; set; }

        private void Init()
        {
            Words = new Dictionary<string, uint>();
            StepInactives = new Dictionary<string, uint>();
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
                StepInactives.Add(word, 0);
            }
        }

        public void AddWordOrAddScore(string word)
        {
            if (!Words.ContainsKey(word))
            {
                Words.Add(word, Configuration.Word_Score_Update_When_Correct_Form_Word_Unknown);
                StepInactives.Add(word, 0);
            }
            else
            {
                Words[word] += Configuration.Word_Score_Update_When_Correct_Form;
                if (Words[word] > Configuration.Word_Score_Max) Words[word] = Configuration.Word_Score_Max;
                StepInactives[word] = 0;
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
            StepInactives[result] = 0;
            return result;
        }

        private void AddToDictionary(Dictionary<string, uint> dictionary, KeyValuePair<string, uint> item, Dictionary<string, uint> inactive)
        {
            if (dictionary.ContainsKey(item.Key))
            {
                dictionary[item.Key] += item.Value;
                if (dictionary[item.Key] > Configuration.Word_Score_Max) dictionary[item.Key] = Configuration.Word_Score_Max;
            }
            else
            {
                dictionary.Add(item.Key, item.Value);
                inactive.Add(item.Key, 0);
            }
        }

        public void MergeInto(LexiconAssocation other)
        {
            foreach(KeyValuePair<string, uint> item in Words)
            {
                AddToDictionary(other.Words, item, other.StepInactives);
            }
        }

        private void AddWordScore(string word)
        {
            if (Words.ContainsKey(word))
            {
                Words[word] += Configuration.Word_Score_Update_When_Correct;
                if (Words[word] > Configuration.Word_Score_Max) Words[word] = Configuration.Word_Score_Max;
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
                        StepInactives.Remove(word);
                    }
                }
                else
                {
                    Words.Remove(word);
                    StepInactives.Remove(word);
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

        public void AnotherNodeIsCorrectGuesser(string word) // used when the word is deemed correct in another node
        {
            DecreaseWordScore(word, Configuration.Word_Score_Update_When_Other_Correct_Guesser);
        }

        public uint GetScore(string word)
        {
            if (Words.ContainsKey(word))
            {
                return Words[word];
            }
            return 0;
        }

        public void Erode()
        {
            List<string> WordsToTrim = new List<string>();
            foreach(KeyValuePair<string, uint> item in StepInactives)
            {
                if (item.Value >= Configuration.Word_Inactive_Steps_To_Erode)
                {
                    if (Words[item.Key] >= Configuration.Word_Score_Erosion)
                    {
                        Words[item.Key] -= Configuration.Word_Score_Erosion;
                        StepInactives[item.Key] += 1;
                    }
                    else
                    {
                        WordsToTrim.Add(item.Key);
                    }
                }
            }
            foreach(string word in WordsToTrim)
            {
                Words.Remove(word);
                StepInactives.Remove(word);
            }
        }
    }
}
