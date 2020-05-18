using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using TalkingHeads.DataStructures;

namespace TalkingHeads.BodyParts
{
    public static class Memory
    {
        public static byte[] LoadImageToByte(string fileName)
        {
            return File.ReadAllBytes(fileName);
        }

        public static MemoryStream LoadImageToMemoryStream(string fileName)
        {
            byte[] bytesArray = File.ReadAllBytes(fileName);
            return new MemoryStream(bytesArray);
        }

        public static Bitmap LoadImageToBmp(string fileName)
        {
            return new Bitmap(fileName);
        }

        public static Image LoadImage(string fileName)
        {
            return Image.FromFile(fileName);
        }

        public static void SaveTalkingHead(TalkingHead th)
        {
            if (Configuration.Local)
            {
                string filePath = Configuration.LocalPath + th.Name.ToLower() + Configuration.SaveFileExt;
                string save = th.ToString();

                File.WriteAllText(filePath, save);
            }
            else
            {
                throw new NotImplementedException("TO-DO server side save");
            }
        }
        public static void LoadTalkingHead(TalkingHead th)
        {
            if (Configuration.Local)
            {
                string filePath = Configuration.LocalPath + th.Name.ToLower() + Configuration.SaveFileExt;
                DiscriminationTree currentTree = null;
                DiscriminationTree.Node currentNode = null;
                DiscriminationTree.Node currentFather = null;
                int SonsCounter = 0;
                if (File.Exists(filePath))
                {
                    string[] lines = File.ReadAllLines(filePath);
                    foreach (string line in lines)
                    {
                        string[] split  = line.Split(Configuration.Separator);
                        if (split.Length == 0) continue;
                        switch (split[0])
                        {
                            case "0": // Node is null
                                currentNode = null;
                                SonsCounter++;
                                if (SonsCounter == 2)
                                {
                                    SonsCounter = 0;
                                    currentFather = currentFather.Father;
                                }
                                break;
                            case "1": // Node is the root
                                currentNode = currentTree._root;
                                currentFather = currentTree._root;
                                break;
                            case "2": // Node data
                                if (SonsCounter == 0) // left son
                                {
                                    currentNode = new DiscriminationTree.Node()
                                    {
                                        MinValue = currentFather.MaxValue / 2,
                                        MaxValue = currentFather.MaxValue,
                                        Father = currentFather,
                                        Score = Configuration.Node_Default_Score,
                                        Data = new LexiconAssocation(currentTree.treeDiscriminant, currentFather.MaxValue / 2, currentFather.MaxValue)
                                    };
                                }
                                else // right son
                                {
                                    currentNode = new DiscriminationTree.Node()
                                    {
                                        MinValue = currentFather.MinValue,
                                        MaxValue = currentFather.MaxValue / 2,
                                        Father = currentFather,
                                        Score = Configuration.Node_Default_Score,
                                        Data = new LexiconAssocation(currentTree.treeDiscriminant, currentFather.MinValue, currentFather.MaxValue / 2)
                                    };
                                }
                                
                                for (int i = 1; i<split.Length; i+=2)
                                {
                                    if (currentTree != null) currentNode.AddWord(split[i], uint.Parse(split[i + 1]));
                                }
                                SonsCounter++;
                                if (SonsCounter == 2)
                                {
                                    SonsCounter = 0;
                                    currentFather = currentFather.Father;
                                }
                                break;
                            default: // tree discriminant
                                switch (split[0])
                                {
                                    case "Alpha":
                                        th.Alpha = new DiscriminationTree("Alpha");
                                        currentTree = th.Alpha;
                                        break;
                                    case "Red":
                                        th.Red = new DiscriminationTree("Red");
                                        currentTree = th.Red;
                                        break;
                                    case "Green":
                                        th.Green = new DiscriminationTree("Green");
                                        currentTree = th.Green;
                                        break;
                                    case "Blue":
                                        th.Blue = new DiscriminationTree("Blue");
                                        currentTree = th.Blue;
                                        break;
                                    case "Xpos":
                                        th.Xpos = new DiscriminationTree("Xpos");
                                        currentTree = th.Xpos;
                                        break;
                                    case "Ypos":
                                        th.Ypos = new DiscriminationTree("Ypos");
                                        currentTree = th.Ypos;
                                        break;
                                    case "Width":
                                        th.Width = new DiscriminationTree("Width");
                                        currentTree = th.Width;
                                        break;
                                    case "Height":
                                        th.Height = new DiscriminationTree("Height");
                                        currentTree = th.Height;
                                        break;
                                }
                                break;
                        }
                    }
                }
                else
                {
                    throw new DirectoryNotFoundException("This talking head does not exist. Check for errors in the name.");
                }
            }
            else
            {
                throw new NotImplementedException("TO-DO server side load");
            }
        }
    }
}
