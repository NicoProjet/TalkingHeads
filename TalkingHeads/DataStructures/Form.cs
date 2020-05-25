using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace TalkingHeads.DataStructures
{
    public class Form
    {
        public Point TopLeft;
        public bool TopLeft_initialized = false;
        public Point BottomRight;
        public bool BottomRight_initialized = false;
        public Color Centroid;
        public bool Centroid_initialized = false;
        public UInt64 pixelNumber = 0;
        public Rectangle Rect;

        public Form(int x, int y, Color color)
        {
            Point p = new Point(x, y);
            Add(p, color);
        }

        public Form(Point p, Color color)
        {
            Add(p, color);
        }

        public void ComputeTopLeft(Point p)
        {
            if (!TopLeft_initialized)
            {
                TopLeft = new Point(p.X, p.Y);
                TopLeft_initialized = true;
            }
            else
            {
                if (TopLeft.X > p.X)
                {
                    TopLeft.X = p.X;
                }
                if (TopLeft.Y > p.Y)
                {
                    TopLeft.Y = p.Y;
                }
            }
        }

        public void ComputeBottomRight(Point p)
        {
            if (!BottomRight_initialized)
            {
                BottomRight = new Point(p.X, p.Y);
                BottomRight_initialized = true;
            }
            else
            {
                if (BottomRight.X < p.X)
                {
                    BottomRight.X = p.X;
                }
                if (BottomRight.Y < p.Y)
                {
                    BottomRight.Y = p.Y;
                }
            }
        }

        public void ComputeCentroid(Color c)
        {
            if (!Centroid_initialized)
            {
                Centroid = c;
                Centroid_initialized = true;
            }
            else
            {
                int A = Centroid.A;
                int diff = (int)Centroid.A - (int)c.A;
                if (pixelNumber < Int32.MaxValue)
                {
                    if ((ulong)Math.Abs(diff) / (pixelNumber + 1) >= 1) A -= diff / ((int)pixelNumber + 1);
                }

                int R = Centroid.R;
                diff = (int)Centroid.R - (int)c.R;
                if (pixelNumber < Int32.MaxValue)
                {
                    if ((ulong)Math.Abs(diff) / (pixelNumber + 1) >= 1) R -= diff / ((int)pixelNumber + 1);
                }

                int G = Centroid.G;
                diff = (int)Centroid.G - (int)c.G;
                if (pixelNumber < Int32.MaxValue)
                {
                    if ((ulong)Math.Abs(diff) / (pixelNumber + 1) >= 1) G -= diff / ((int)pixelNumber + 1);
                }

                int B = Centroid.B;
                diff = (int)Centroid.B - (int)c.B;
                if (pixelNumber < Int32.MaxValue)
                {
                    if ((ulong)Math.Abs(diff) / (pixelNumber + 1) >= 1) B -= diff / ((int)pixelNumber + 1);
                }

                Centroid = Color.FromArgb(A, R, G, B);
            }
        }

        public void ComputeRectangle()
        {
            Rect = Rectangle.FromLTRB(TopLeft.X, TopLeft.Y, BottomRight.X, BottomRight.Y);
        }

        public void Add(Point p, Color c)
        {
            ComputeTopLeft(p);
            ComputeBottomRight(p);
            ComputeCentroid(c);
            pixelNumber++;
        }

        public void Add(int x, int y, Color c)
        {
            Add(new Point(x, y), c);
        }

        public bool SameColor(Color c)
        {
            return Centroid.A == c.A && Centroid.R == c.R && Centroid.G == c.G && Centroid.B == c.B;
        }

        public Rectangle ToRectangle()
        {
            return Rectangle.FromLTRB(TopLeft.X, TopLeft.Y, BottomRight.X, BottomRight.Y);
        }

        public int Width()
        {
            if (!BottomRight_initialized || !TopLeft_initialized) return -1;
            else return BottomRight.X - TopLeft.X;
        }
        public int Height()
        {
            if (!BottomRight_initialized || !TopLeft_initialized) return -1;
            else return BottomRight.Y - TopLeft.Y;
        }

        public bool IntersectsWith(Form other)
        {
            return other.BottomRight.X >= TopLeft.X
                && other.TopLeft.X <= BottomRight.X
                && other.BottomRight.Y >= TopLeft.Y
                && other.TopLeft.Y <= BottomRight.Y;
        }
    }
}
