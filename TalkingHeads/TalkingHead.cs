using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TalkingHeads.DataStructures;
using TalkingHeads.BodyParts;

namespace TalkingHeads
{
    public class TalkingHead
    {
        public string Name = "NotAssigned";
        public DiscriminationTree Alpha = new DiscriminationTree("Alpha");
        public DiscriminationTree Red = new DiscriminationTree("Red");
        public DiscriminationTree Green = new DiscriminationTree("Green");
        public DiscriminationTree Blue = new DiscriminationTree("Blue");
        public DiscriminationTree Xpos = new DiscriminationTree("Xpos");
        public DiscriminationTree Ypos = new DiscriminationTree("Ypos");
        public DiscriminationTree Width = new DiscriminationTree("Width");
        public DiscriminationTree Height = new DiscriminationTree("Height");

        public TalkingHead()
        {

        }

        public TalkingHead(string name, bool createIfNotExists = false)
        {
            Name = name.ToLower();
            Memory.LoadTalkingHead(this, createIfNotExists);
        }

        public override string ToString()
        {
            string response = "";
            string treeSeparator = "\n---\n";
            foreach(DiscriminationTree tree in GetTrees())
            {
                response += tree.ToString() + treeSeparator;
            }
            return response;
        }

        public List<DiscriminationTree> GetTrees()
        {
            List<DiscriminationTree> trees = new List<DiscriminationTree>();
            trees.Add(Alpha);
            trees.Add(Red);
            trees.Add(Green);
            trees.Add(Blue);
            trees.Add(Xpos);
            trees.Add(Ypos);
            trees.Add(Width);
            trees.Add(Height);
            return trees;
        }

        public LexiconAssocation MakeGuess(string description)
        {
            LexiconAssocation bestGuess = null;
            uint bestScore = 0;
            foreach (DiscriminationTree tree in GetTrees())
            {
                LexiconAssocation currentGuess = tree.MakeGuess(description);
                if (currentGuess != null && currentGuess.Words[description] > bestScore)
                {
                    bestGuess = currentGuess;
                    bestScore = currentGuess.Words[description];
                }
            }
            return bestGuess;
        }

        public DiscriminationTree.Node MakeGuessNode(string word)
        {
            DiscriminationTree.Node bestGuess = null;
            uint bestScore = 0;
            foreach (DiscriminationTree tree in GetTrees())
            {
                DiscriminationTree.Node currentGuess = tree.MakeGuessNode(word);
                if (currentGuess != null && currentGuess.Data.Words[word] > bestScore)
                {
                    bestGuess = currentGuess;
                    bestScore = currentGuess.Data.Words[word];
                }
            }
            if (bestGuess != null)
            {
                bestGuess.Used();
                bestGuess.Data.StepInactives[word] = 0;
            }
            return bestGuess;
        }

        public void Erode()
        {
            Alpha.Erode();
            Red.Erode();
            Green.Erode();
            Blue.Erode();
            Xpos.Erode();
            Ypos.Erode();
            Width.Erode();
            Height.Erode();
        }

        public void UpdateScore(List<DiscriminationTree.Guess> processingMemory, bool correct)
        {
            if (processingMemory.Any(x => x.Node == null)) return;
            foreach (DiscriminationTree.Guess processingMemoryPart in processingMemory)
            {
                switch (Enumerations.GetDiscriminant(processingMemoryPart.Node.Data.TreeDiscriminant))
                {
                    case Enumerations.Disciminants.Alpha:
                        Alpha.UpdateScore(processingMemoryPart, correct);
                        break;
                    case Enumerations.Disciminants.Red:
                        Red.UpdateScore(processingMemoryPart, correct);
                        break;
                    case Enumerations.Disciminants.Green:
                        Green.UpdateScore(processingMemoryPart, correct);
                        break;
                    case Enumerations.Disciminants.Blue:
                        Blue.UpdateScore(processingMemoryPart, correct);
                        break;
                    case Enumerations.Disciminants.Xpos:
                        Xpos.UpdateScore(processingMemoryPart, correct);
                        break;
                    case Enumerations.Disciminants.Ypos:
                        Ypos.UpdateScore(processingMemoryPart, correct);
                        break;
                    case Enumerations.Disciminants.Width:
                        Width.UpdateScore(processingMemoryPart, correct);
                        break;
                    case Enumerations.Disciminants.Height:
                        Height.UpdateScore(processingMemoryPart, correct);
                        break;
                }
            }
            Erode();
        }

        public void RemoveScoreForWord(string word)
        {
            foreach (DiscriminationTree tree in GetTrees())
            {
                tree.RemoveScoreForWord(word);
            }
        }
    }
}
