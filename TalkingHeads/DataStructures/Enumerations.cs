using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkingHeads.DataStructures
{
    public class Enumerations
    {
        public enum Disciminants
        {
            Alpha,
            Red,
            Green,
            Blue,
            Xpos,
            Ypos,
            Width,
            Height,
        }

        public static Disciminants GetDiscriminant(string discriminantString)
        {
            switch (discriminantString)
            {
                case "Alpha":
                    return Disciminants.Alpha;
                case "Red":
                    return Disciminants.Red;
                case "Green":
                    return Disciminants.Green;
                case "Blue":
                    return Disciminants.Blue;
                case "Xpos":
                    return Disciminants.Xpos;
                case "Ypos":
                    return Disciminants.Ypos;
                case "Width":
                    return Disciminants.Width;
                case "Height":
                    return Disciminants.Height;
                default:
                    throw new NotSupportedException("The discriminant string was not recognized.");
            }
        }
    }
}
