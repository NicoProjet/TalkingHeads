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
        public readonly string Name = "NotAssigned";
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

        public TalkingHead(string name)
        {
            Name = name.ToLower();
            Memory.LoadTalkingHead(this);
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
    }
}
