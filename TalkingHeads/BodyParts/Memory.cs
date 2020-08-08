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

        public static void SaveTalkingHead(TalkingHead th, string path)
        {
            File.WriteAllText(path, th.ToString());
        }

        private static void LoadTalkingHeadFromFile(TalkingHead th, string filePath = null, bool createIfNotExists = false)
        {
            if (filePath == null) filePath = Configuration.LocalPath + th.Name.ToLower() + Configuration.SaveFileExt;
            DiscriminationTree currentTree = null;
            DiscriminationTree.Node currentNode = null;
            bool lastNodeWasLeft = false;
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    string[] split = line.Split(Configuration.Separator);
                    if (split.Length == 0) continue;
                    switch (split[0])
                    {
                        case "0": // Node is the root
                            currentNode = currentTree._root;
                            currentNode.Score = uint.Parse(split[1]);
                            break;
                        case "1": // Node data
                            if (!lastNodeWasLeft) // left son
                            {
                                currentNode.Left = new DiscriminationTree.Node()
                                {
                                    MinValue = currentNode.MinValue,
                                    MaxValue = currentNode.GetMiddleValue(),
                                    Father = currentNode,
                                    Data = new LexiconAssocation(currentTree.treeDiscriminant, currentNode.MinValue, currentNode.GetMiddleValue()),
                                    IsLeftSon = true,
                                    Depth = currentNode.Depth + 1,
                                };
                                currentNode = currentNode.Left;
                                currentNode.Score = uint.Parse(split[1]);
                            }
                            else // right son
                            {
                                currentNode.Right = new DiscriminationTree.Node()
                                {
                                    MinValue = currentNode.GetMiddleValue(),
                                    MaxValue = currentNode.MaxValue,
                                    Father = currentNode,
                                    Data = new LexiconAssocation(currentTree.treeDiscriminant, currentNode.GetMiddleValue(), currentNode.MaxValue),
                                    IsLeftSon = false,
                                    Depth = currentNode.Depth + 1,
                                };
                                currentNode = currentNode.Right;
                                currentNode.Score = uint.Parse(split[1]);
                                lastNodeWasLeft = false;
                            }

                            for (int i = 2; i < split.Length; i += 2)
                            {
                                if (currentTree != null) currentNode.AddWord(split[i], uint.Parse(split[i + 1]));
                            }
                            break;
                        case "2": // no left son
                            lastNodeWasLeft = true;
                            break;
                        case "3": // no right son
                            lastNodeWasLeft = true;
                            currentNode = currentNode.Father;
                            break;
                        case "4": // end of right son
                            lastNodeWasLeft = true;
                            currentNode = currentNode.Father;
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
                            lastNodeWasLeft = false;
                            break;
                    }
                }
            }
            else
            {
                if (createIfNotExists)
                {
                    Memory.SaveTalkingHead(th, filePath);
                }
                else throw new DirectoryNotFoundException("This talking head does not exist. Look for errors in the name.");
            }
        }

        [Obsolete("Needs an update, look at the loading method that charges a file")]
        public static void LoadTalkingHeadFromFile(Stream str, TalkingHead th)
        {
            DiscriminationTree currentTree = null;
            DiscriminationTree.Node currentNode = null;
            bool lastNodeWasLeft = false;
            using (StreamReader sr = new StreamReader(str))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] split = line.Split(Configuration.Separator);
                    if (split.Length == 0) continue;
                    switch (split[0])
                    {
                        case "0": // Node is the root
                            currentNode = currentTree._root;
                            break;
                        case "1": // Node data
                            if (!lastNodeWasLeft) // left son
                            {
                                currentNode.Left = new DiscriminationTree.Node()
                                {
                                    MinValue = currentNode.MaxValue / 2,
                                    MaxValue = currentNode.MaxValue,
                                    Father = currentNode,
                                    Data = new LexiconAssocation(currentTree.treeDiscriminant, currentNode.MaxValue / 2, currentNode.MaxValue),
                                    IsLeftSon = true,
                                    Depth = currentNode.Depth + 1,
                                };
                                currentNode = currentNode.Left;
                                lastNodeWasLeft = true;
                            }
                            else // right son
                            {
                                currentNode.Right = new DiscriminationTree.Node()
                                {
                                    MinValue = currentNode.MinValue,
                                    MaxValue = currentNode.MaxValue / 2,
                                    Father = currentNode,
                                    Data = new LexiconAssocation(currentTree.treeDiscriminant, currentNode.MinValue, currentNode.MaxValue / 2),
                                    IsLeftSon = false,
                                    Depth = currentNode.Depth + 1,
                                };
                                currentNode = currentNode.Right;
                                lastNodeWasLeft = false;
                            }

                            for (int i = 1; i < split.Length; i += 2)
                            {
                                if (currentTree != null) currentNode.AddWord(split[i], uint.Parse(split[i + 1]));
                            }
                            break;
                        case "2":
                            lastNodeWasLeft = true;
                            break;
                        case "3":
                            lastNodeWasLeft = true;
                            currentNode = currentNode.Father;
                            break;
                        case "4": // end of right son
                            lastNodeWasLeft = true;
                            currentNode = currentNode.Father;
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
                            lastNodeWasLeft = false;
                            break;
                    }
                }
            }
        }

        public static void SaveTalkingHeadFromFile(Stream str, TalkingHead th)
        {
            using(StreamWriter sw = new StreamWriter(str))
            {
                string test = th.ToString();
                sw.Write(th.ToString());
            }
        }

        public static void LoadTalkingHead(TalkingHead th, bool createIfNotExists = false, string filePath = null)
        {
            if (filePath != null)
            {
                LoadTalkingHeadFromFile(th, filePath, createIfNotExists);
            }
            else if (Configuration.Local)
            {
                LoadTalkingHeadFromFile(th, null, createIfNotExists);
            }
            else
            {
                throw new NotImplementedException("TO-DO server side load");
            }
        }
    }
}
