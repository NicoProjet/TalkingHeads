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
            public uint Score { get; set; }
            public uint InactiveSteps { get; set; }
            public double MinValue { get; set; }
            public double MaxValue { get; set; }
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
                return MaxValue / 2;
            }

            public void Erode()
            {
                if (InactiveSteps >= Configuration.Node_Inactive_Steps_To_Erode)
                {
                    if (Score >= Configuration.Node_Score_Erosion)
                    {
                        Score -= Configuration.Node_Score_Erosion;
                    }
                    else
                    {
                        Score = 0;
                    }
                }
            }

            public void AddWord(string word)
            {
                Data.AddWord(word);
            }

            public void AddWord(string word, uint score)
            {
                Data.AddWord(word, score);
            }

            public void MergeIntoParent()
            {
                Data.MergeInto(Father.Data);
            }

            public void Split(string _treeDiscriminant)
            {
                // TO-TEST : s'assurer que le clear ne vide pas les enfants (shallow copy)
                if (!HasLeftSon())
                {
                    Left = new Node()
                    {
                        MinValue = MinValue,
                        MaxValue = GetMiddleValue(),
                        Father = this,
                        Score = Configuration.Node_Default_Score,
                        Data = new LexiconAssocation(_treeDiscriminant, MinValue, GetMiddleValue()),
                    };
                    Left.Data.Words = Data.Words;
                }
                if (!HasRightSon())
                {
                    Right = new Node()
                    {
                        MinValue = GetMiddleValue(),
                        MaxValue = MaxValue,
                        Father = this,
                        Score = Configuration.Node_Default_Score,
                        Data = new LexiconAssocation(_treeDiscriminant, GetMiddleValue(), MaxValue),
                    };
                    Right.Data.Words = Data.Words;
                }
                Data.Words.Clear();
            }

            public string ToString(bool isRoot = false)
            {
                string response = "";
                if (isRoot)
                {
                    response += "1" + Configuration.LineSeparator;
                }
                else
                {
                    response += "2" + Configuration.Separator +  Data.ToString() + Configuration.LineSeparator;
                }


                if (Left != null)
                {
                    response += Left.ToString();
                }
                else
                {
                    response += "0" + Configuration.LineSeparator;
                }
                if (Right != null)
                {
                    response += Right.ToString();
                }
                else
                {
                    response += "0" + Configuration.LineSeparator;
                }
                return response;
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
            };
        }


        public DiscriminationTree()
        {
            CreateRoot();
        }

        public DiscriminationTree(string discriminant)
        {
            CreateRoot();
            treeDiscriminant = discriminant;
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
                        Score = Configuration.Node_Default_Score,
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
                        Score = Configuration.Node_Default_Score,
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

        private Node GetNode(double value)
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
                _root.Split(treeDiscriminant);
                return GetNode(value);
            }
            return currentNode;
        }

        private char RandomFromCharArray(char[] array, Random seed)
        {
            int index = seed.Next(0, array.Length);
            return array[index];
        }

        private string CreateWord()
        {
            char[] vowels = { 'a', 'e', 'i', 'o', 'u' };
            char[] consonants = { 'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v' };
            char[] rareConsonants = { 'w', 'x', 'y', 'z' };

            string newWord = "";
            Random rand = new Random();
            bool startWithVowel = false;
            int numberOfLetters = rand.Next((int)Configuration.Min_Number_Letters, (int)Configuration.Max_Number_Letters);
            for (int i = 0; i < numberOfLetters; i++)
            {
                if (startWithVowel)
                {
                    if (i%2 == 0)
                    {
                        newWord += RandomFromCharArray(vowels, rand);
                    }
                    else
                    {
                        if (rand.Next(0,100) < Configuration.Rare_Consonant_Percentage) newWord += RandomFromCharArray(rareConsonants, rand);
                        else newWord += RandomFromCharArray(consonants, rand);
                    }
                }
                else
                {
                    if (i % 2 == 0)
                    {
                        if (rand.Next(0, 100) < Configuration.Rare_Consonant_Percentage) newWord += RandomFromCharArray(rareConsonants, rand);
                        else newWord += RandomFromCharArray(consonants, rand);
                    }
                    else
                    {
                        newWord += RandomFromCharArray(vowels, rand);
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
                    node.MergeIntoParent();
                }
                else if (node.Score > Configuration.Node_Score_To_Split)
                {
                    node.Split(treeDiscriminant);
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
    }
}
