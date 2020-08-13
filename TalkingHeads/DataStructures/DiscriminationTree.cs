using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TalkingHeads.DataStructures
{
    public class DiscriminationTree
    {
        public class Node
        {
            public uint Depth { get; set; }
            public uint Score { get; set; }
            public uint InactiveSteps { get; set; }
            public double MinValue { get; set; }
            public double MaxValue { get; set; }
            public bool IsLeftSon { get; set; }
            public LexiconAssocation Data { get; set; }
            public Node Left { get; set; }
            public Node Right { get; set; }
            public Node Father { get; set; }
            
            private void Init()
            {
                InactiveSteps = 0;
                this.Score = Configuration.Node_Default_Score;
            }
            public Node()
            {
                Init();
                this.Data = new LexiconAssocation();
            }
            public Node(LexiconAssocation data)
            {
                Init(); 
                this.Data = data;
            }

            public bool HasSon()
            {
                return this.Left != null || this.Right != null;
            }

            public bool HasLeftSon()
            {
                return this.Left != null;
            }

            public bool HasRightSon()
            {
                return this.Right != null;
            }

            public double GetMiddleValue()
            {
                return MinValue + ( (MaxValue-MinValue) / 2);
            }

            public void Erode()
            {
                Data.Erode();
                if (InactiveSteps >= Configuration.Node_Inactive_Steps_To_Erode)
                {
                    if (Score >= Configuration.Node_Score_Erosion)
                    {
                        Score -= Configuration.Node_Score_Erosion;
                    }
                    else
                    {
                        Score = 0;
                        Reduce();
                    }
                }
                InactiveSteps++;
            }

            public void AddWord(string word)
            {
                Data.AddWord(word);
            }

            public void AddWord(string word, uint score)
            {
                Data.AddWord(word, score);
            }

            public void Reduce()
            {
                //Console.WriteLine("Reduce " + Data.StringValue + ": " + Data.ToString());
                Data.MergeInto(Father.Data);
                if (Father.Score < Configuration.Node_Default_Score) Father.Score = Configuration.Node_Default_Score;
                if (IsLeftSon) Father.Left = null;
                else Father.Right = null;
                //Console.WriteLine("Reduce result (father) " + Father.Data.StringValue + ": " + Father.Data.ToString()  
                //    + "  |   Left = " + (Father.Left == null ? "null" : "not null")
                //    + "  |   Right = " + (Father.Right == null ? "null" : "not null"));
                //Console.WriteLine("--------------");
            }

            public static Dictionary<TKey, TValue> DeepCopyICloneable<TKey, TValue> (Dictionary<TKey, TValue> original) where TValue : ICloneable
            {
                Dictionary<TKey, TValue> ret = new Dictionary<TKey, TValue>(original.Count,
                                                                        original.Comparer);
                foreach (KeyValuePair<TKey, TValue> entry in original)
                {
                    ret.Add(entry.Key, (TValue)entry.Value.Clone());
                }
                return ret;
            }

            public static Dictionary<string, uint> CustomDeepCopy(Dictionary<string, uint> original)
            {
                Dictionary<string, uint> response = new Dictionary<string, uint>();
                foreach(KeyValuePair<string, uint> item in original)
                {
                    response.Add(item.Key, item.Value);
                }
                return response;
            }

            public void Split()
            {
                //Console.WriteLine("Split " + Data.StringValue + ": " + Data.ToString());
                if (!HasLeftSon())
                {
                    Left = new Node()
                    {
                        MinValue = MinValue,
                        MaxValue = GetMiddleValue(),
                        Father = this,
                        Data = new LexiconAssocation(Data.TreeDiscriminant, MinValue, GetMiddleValue()),
                        IsLeftSon = true,
                        Depth = this.Depth + 1,
                    };
                    Left.Data.Words = CustomDeepCopy(Data.Words);
                }
                if (!HasRightSon())
                {
                    Right = new Node()
                    {
                        MinValue = GetMiddleValue(),
                        MaxValue = MaxValue,
                        Father = this,
                        Data = new LexiconAssocation(Data.TreeDiscriminant, GetMiddleValue(), MaxValue),
                        IsLeftSon = false,
                        Depth = this.Depth + 1,
                    };
                    Right.Data.Words = CustomDeepCopy(Data.Words);
                }
                Data.Words.Clear();
                Data.StepInactives.Clear();
                //Console.WriteLine("Split result self " + Data.StringValue + ": " + Data.ToString());
                //Console.WriteLine("Split result left " + Left.Data.StringValue + ": " + Left.Data.ToString());
                //Console.WriteLine("Split result right " + Right.Data.StringValue + ": " + Right.Data.ToString());
                //Console.WriteLine("--------------");
            }

            private string GetScoreForSave()
            {
                return "" + Configuration.Separator + Score;
            }

            public string ToString(bool isRoot = false)
            {
                string response = "";
                if (isRoot)
                {
                    response += "0" + GetScoreForSave() + Configuration.LineSeparator;
                }
                else
                {
                    response += "1" + GetScoreForSave() + (Data.Words.Count() != 0 ? "" + Configuration.Separator : "") +  Data.ToString() + Configuration.LineSeparator;
                }


                if (Left != null)
                {
                    response += Left.ToString();
                }
                else
                {
                    response += "2" + Configuration.LineSeparator;
                }
                if (Right != null)
                {
                    response += Right.ToString();
                    response += "4" + Configuration.LineSeparator;
                }
                else
                {
                    response += "3" + Configuration.LineSeparator;
                }
                return response;
            }

            public void IncorrectGuess()
            {
                Score -= Configuration.Node_Reward_For_Incorrect;
            }

            public void CorrectGuess()
            {
                Score += Configuration.Node_Reward_For_Correct;
            }

            public void Used()
            {
                Score += Configuration.Node_Reward_For_Use;
                InactiveSteps = 0;
            }

            public void SplitOrReduce()
            {
                if (Score < Configuration.Node_Score_To_Reduce && !HasSon())
                {
                    Reduce();
                }
                else if (Score > (Math.Pow(Configuration.Node_Score_To_Split, (double) (Depth + 3) / (double)4)))
                {
                    Split();
                    Score = 0;
                }
            }

            public void UpdateScore(bool correct)
            {
                if (!correct) IncorrectGuess();
                else CorrectGuess();
                SplitOrReduce();
            }

            public void CorrectForm(string word, double value)
            {
                Used();
                SplitOrReduce();
                if (HasLeftSon() && value < GetMiddleValue()) Left.Data.AddWordOrAddScore(word);
                else if (HasRightSon() && value >= GetMiddleValue()) Right.Data.AddWordOrAddScore(word);
                else Data.AddWordOrAddScore(word);
            }

            public string Print()
            {
                return Data.StringValue;
            }
        }



        public Node _root;
        public string treeDiscriminant = "";
        public Enumerations.Disciminants Discriminant;

        private void CreateRoot()
        {
            _root = new Node()
            {
                Score = 0, // root value
                MinValue = 0,
                MaxValue = 1,
                Data = new LexiconAssocation(treeDiscriminant != "" ? treeDiscriminant : "Non assigned", 0, 1),
                Depth = 0,
            };
        }


        public DiscriminationTree()
        {
            CreateRoot();
        }

        public DiscriminationTree(string discriminant)
        {
            treeDiscriminant = discriminant;
            CreateRoot();
            switch (discriminant)
            {
                case "Alpha":
                    Discriminant = Enumerations.Disciminants.Alpha;
                    break;
                case "Red":
                    Discriminant = Enumerations.Disciminants.Red;
                    break;
                case "Green":
                    Discriminant = Enumerations.Disciminants.Green;
                    break;
                case "Blue":
                    Discriminant = Enumerations.Disciminants.Blue;
                    break;
                case "Xpos":
                    Discriminant = Enumerations.Disciminants.Xpos;
                    break;
                case "Ypos":
                    Discriminant = Enumerations.Disciminants.Ypos;
                    break;
                case "Width":
                    Discriminant = Enumerations.Disciminants.Width;
                    break;
                case "Height":
                    Discriminant = Enumerations.Disciminants.Height;
                    break;
            }
        }
        private Node Insert(double value)
        {
            // 1. If the tree is empty, return a new, single node 
            if (_root == null)
            {
                CreateRoot();
                return _root;
            }
            // 2. Otherwise, recur down the tree 
            return InsertRec(_root, value);
        }
        private Node InsertRec(Node root, double value)
        {
            if (root == null)
                throw new ArgumentNullException("InsertRec Node argument is null");

            if (value < root.GetMiddleValue())
            {
                if (root.Left == null)
                {
                    Node newNode = new Node()
                    {
                        MinValue = root.MinValue,
                        MaxValue = root.GetMiddleValue(),
                        Father = root,
                        IsLeftSon = true,
                        Depth = root.Depth + 1,
                    };
                    newNode.Data = new LexiconAssocation(treeDiscriminant, newNode.MinValue, newNode.MaxValue);
                    root.Left = newNode;
                    return newNode;
                }
                else
                {
                    return InsertRec(root.Left, value);
                }

            }
            else
            {
                if (root.Right == null)
                {
                    Node newNode = new Node()
                    {
                        MinValue = root.GetMiddleValue(),
                        MaxValue = root.MaxValue,
                        Father = root,
                        IsLeftSon = false,
                        Depth = root.Depth + 1,
                    };
                    newNode.Data = new LexiconAssocation(treeDiscriminant, newNode.MinValue, newNode.MaxValue);
                    root.Right = newNode;
                    return newNode;
                }
                else
                {
                    return InsertRec(root.Right, value);
                }
            }
        }
        private void DisplayTree(Node root)
        {
            if (root == null) return;

            DisplayTree(root.Left);
            System.Console.Write(root.Data.ToString() + " ");
            DisplayTree(root.Right);
        }
        public void DisplayTree()
        {
            DisplayTree(_root);
        }

        public Node GetNode(double value)
        {
            Node currentNode = _root;
            while (currentNode != null)
            {
                if (value <= currentNode.GetMiddleValue())
                {
                    if (currentNode.Left != null)
                    {
                        currentNode = currentNode.Left;
                    }
                    else break;
                }
                else
                {
                    if (currentNode.Right != null)
                    {
                        currentNode = currentNode.Right;
                    }
                    else break;
                }
            }
            if (currentNode == _root)
            {
                _root.Split();
                return GetNode(value);
            }
            return currentNode;
        }

        private char RandomFromCharArray(char[] array)
        {
            int index = Configuration.seed.Next(0, array.Length);
            return array[index];
        }

        private string CreateWord()
        {
            char[] vowels = { 'a', 'e', 'i', 'o', 'u' };
            char[] consonants = { 'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v' };
            char[] rareConsonants = { 'w', 'x', 'y', 'z' };

            string newWord = "";
            bool startWithVowel = Configuration.seed.Next(2) == 0 ? true : false;
            int numberOfLetters = Configuration.seed.Next((int)Configuration.Min_Number_Letters, (int)Configuration.Max_Number_Letters);
            for (int i = 0; i < numberOfLetters; i++)
            {
                if (startWithVowel)
                {
                    if (i%2 == 0)
                    {
                        newWord += RandomFromCharArray(vowels);
                    }
                    else
                    {
                        if (Configuration.seed.Next(0,100) < Configuration.Rare_Consonant_Percentage) newWord += RandomFromCharArray(rareConsonants);
                        else newWord += RandomFromCharArray(consonants);
                    }
                }
                else
                {
                    if (i % 2 == 0)
                    {
                        if (Configuration.seed.Next(0, 100) < Configuration.Rare_Consonant_Percentage) newWord += RandomFromCharArray(rareConsonants);
                        else newWord += RandomFromCharArray(consonants);
                    }
                    else
                    {
                        newWord += RandomFromCharArray(vowels);
                    }
                }
            }
            return newWord;
        }

        public struct Guess
        {
            public Node Node { get; set; }
            public string Word { get; set; }
        }

        public Guess GetGuess(double value)
        {
            Node node = GetNode(value);
            string word = node.Data.GetWord();
            if (word == "")
            {
                word = CreateWord();
                node.AddWord(word);
            }
            node.Used();
            return new Guess
            {
                Node = node,
                Word = word,
            };
        }

        public string GetWord(double value)
        {
            Node node = GetNode(value);
            string word = node.Data.GetWord();
            if (word == "")
            {
                word = CreateWord();
                node.AddWord(word);
            }
            return word;
        }

        private void UpdateWordScoreRecursive(Node node, double value, string word)
        {
            if ((!node.HasLeftSon() &&  value < node.GetMiddleValue()) 
                || (!node.HasRightSon() && value >= node.GetMiddleValue()))
            {
                node.Data.CorrectGuess(word);
            }
            else
            {
                node.Data.AnotherNodeIsCorrect(word);
            }

            if (node.HasLeftSon())
            {
                UpdateWordScoreRecursive(node.Left, value, word);
            }
            if (node.HasRightSon())
            {
                UpdateWordScoreRecursive(node.Right, value, word);
            }
        }

        [Obsolete("Function is not up to date and does not follow all the score management rules. Use mobile version 'UpdateScore(DiscriminationTree.Guess guess, bool correct)' or update it correctly.")]
        public void UpdateWordScore(double value, string word, bool correct)
        {
            if (!correct)
            {
                Node node = GetNode(value);
                node.Data.IncorrectGuess(word);
            }
            else
            {
                UpdateWordScoreRecursive(_root, value, word);
            }
        }

        private void UpdateWordScoreRecursiveMobile(Node node, double value, string word)
        {
            if (node.GetMiddleValue() == value)
            {
                node.Data.CorrectGuess(word);
            }
            else
            {
                node.Data.AnotherNodeIsCorrect(word);
            }

            if (node.HasLeftSon())
            {
                UpdateWordScoreRecursiveMobile(node.Left, value, word);
            }
            if (node.HasRightSon())
            {
                UpdateWordScoreRecursiveMobile(node.Right, value, word);
            }
        }

        public void UpdateScore(DiscriminationTree.Guess guess, bool correct)
        {
            if (!correct)
            {
                // Word score update
                guess.Node.Data.IncorrectGuess(guess.Word);
            }
            else
            {
                // Word score update
                UpdateWordScoreRecursiveMobile(_root, guess.Node.GetMiddleValue(), guess.Word);
            }// Node score update
            guess.Node.UpdateScore(correct);
        }

        private void ErodeRecursive(Node node)
        {
            node.Erode();
            if (node.HasLeftSon()) ErodeRecursive(node.Left);
            if (node.HasRightSon()) ErodeRecursive(node.Right);
        }

        public void Erode()
        {
            if (_root.HasLeftSon()) ErodeRecursive(_root.Left);
            if (_root.HasRightSon()) ErodeRecursive(_root.Right);
        }

        private void BalanceRecursive(Node node)
        {
            if (!node.HasSon())
            {
                if (node.Score < Configuration.Node_Score_To_Reduce)
                {
                    node.Reduce();
                }
                else if (node.Score > (Math.Pow(Configuration.Node_Score_To_Split, (double)(node.Depth + 3) / (double)4)))
                {
                    node.Split();
                }
            }
            else
            {
                if (node.HasLeftSon()) BalanceRecursive(node.Left);
                if (node.HasRightSon()) BalanceRecursive(node.Right);
            }
        }

        public void Balance()
        {
            if (_root.HasLeftSon()) BalanceRecursive(_root.Left);
            if (_root.HasRightSon()) BalanceRecursive(_root.Right);
        }

        public override string ToString()
        {
            string response = "";
            response += treeDiscriminant + Configuration.LineSeparator;
            response += _root.ToString(true);
            return response;
        }

        public LexiconAssocation MakeGuess(string description)
        {
            Node bestNode = null;
            LexiconAssocation bestGuess = null;
            uint bestScore = 0;
            void GoThroughTree(Node node)
            {
                if (node.Data.GetScore(description) > bestScore)
                {
                    bestNode = node;
                    bestGuess = node.Data;
                    bestScore = node.Data.GetScore(description);
                }
                if (node.Left != null) GoThroughTree(node.Left);
                if (node.Right != null) GoThroughTree(node.Right);
            }

            GoThroughTree(_root);
            return bestGuess;
        }

        public Node MakeGuessNode(string description)
        {
            Node bestNode = null;
            uint bestScore = 0;
            void GoThroughTree(Node node)
            {
                if (node.Data.GetScore(description) > bestScore)
                {
                    bestNode = node;
                    bestScore = node.Data.GetScore(description);
                }
                if (node.Left != null) GoThroughTree(node.Left);
                if (node.Right != null) GoThroughTree(node.Right);
            }

            GoThroughTree(_root);
            return bestNode;
        }

        private void RemoveScoreForWordRecursive(Node node, string word)
        {
            if (node.Data.Words.ContainsKey(word)) node.Data.AnotherNodeIsCorrectGuesser(word);
            if (node.HasLeftSon()) RemoveScoreForWordRecursive(node.Left, word);
            if (node.HasRightSon()) RemoveScoreForWordRecursive(node.Right, word);
        }

        public void RemoveScoreForWord(string word)
        {
            RemoveScoreForWordRecursive(_root, word);
        }
    }
}
