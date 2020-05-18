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
            response += Alpha.ToString() + treeSeparator;
            response += Red.ToString() + treeSeparator;
            response += Green.ToString() + treeSeparator;
            response += Blue.ToString() + treeSeparator;
            response += Xpos.ToString() + treeSeparator;
            response += Ypos.ToString() + treeSeparator;
            response += Width.ToString() + treeSeparator;
            response += Height.ToString() + treeSeparator;
            return response;
        }
    }
}
