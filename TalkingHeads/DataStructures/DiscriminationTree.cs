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
        private class Node
        {
            public uint Score { get; set; }
            public uint InactiveSteps { get; set; }
            public double MinValue { get; set; }
            public double MaxValue { get; set; }
            public LexiconAssocation Data { get; set; }
            public Node Left { get; set; }
            public Node Right { get; set; }
            public Node Father { get; set; }
            public bool IsLeftSon { get; set; }
            public Node()
            {
                InactiveSteps = 0;
            }
            public Node(LexiconAssocation data)
            {
                this.Data = data;
                this.Score = Configuration.Node_Default_Score;
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

            public void AddWord(string word, double value)
            {
                if (value < MaxValue / 2)
                {
                    Data.AddLeftWord(word);
                }
                else
                {
                    Data.AddRightWord(word);
                }
            }

            public void MergeIntoParent()
            {
                if (IsLeftSon)
                {
                    Data.MergeInto(Father.Data, true);
                }
                else
                {
                    Data.MergeInto(Father.Data, false);
                }
            }

            public void Split(string _treeDiscriminant)
            {
                // TO-TEST : s'assurer que les clear ne clear pas WordsTempAfterSplit
                Left = new Node()
                {
                    MinValue = MinValue,
                    MaxValue = MaxValue / 2,
                    Father = this,
                    Score = Configuration.Node_Default_Score,
                    Data = new LexiconAssocation(_treeDiscriminant, MinValue, MaxValue / 2),
                    IsLeftSon = true,
                };
                Left.Data.WordsTempAfterSplit = Data.LeftWords;
                Data.LeftWords.Clear();
                Right = new Node()
                {
                    MinValue = MaxValue / 2,
                    MaxValue = MaxValue,
                    Father = this,
                    Score = Configuration.Node_Default_Score,
                    Data = new LexiconAssocation(_treeDiscriminant, MaxValue / 2, MaxValue),
                    IsLeftSon = false,
                };
                Right.Data.WordsTempAfterSplit = Data.RightWords;
                Data.RightWords.Clear();
            }
        }



        private Node _root;
        public string treeDiscriminant;

        private void CreateRoot()
        {
            _root = new Node()
            {
                Score = 0, // root value
                MinValue = 0,
                MaxValue = 1,
                Data = new LexiconAssocation(treeDiscriminant, 0, 1),
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

            if (value < root.MaxValue / 2)
            {
                if (root.Left == null)
                {
                    Node newNode = new Node()
                    {
                        MinValue = root.MinValue,
                        MaxValue = root.MaxValue / 2,
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
                        MinValue = root.MaxValue / 2,
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
            if (!_root.HasSon())
            {
                return null;
            }
            Node currentNode = _root;
            while (currentNode != null)
            {
                if (value <= currentNode.MaxValue / 2)
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

        public string GetWord(double value)
        {
            Node node = GetNode(value);
            string word = node.Data.GetWord();
            if (word == "")
            {
                word = CreateWord();
                node.AddWord(word, value);
            }
            return word;
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
    }
}
