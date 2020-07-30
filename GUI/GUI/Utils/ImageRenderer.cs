using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace GUI.Utils
{
    public class ImageRenderer
    {
        public void PaintSurface(SKSurface surface, SKImageInfo info)
        {
            SKCanvas canvas = surface.Canvas;
            canvas.Clear();

            SKPaint fillPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = new SKColor(128, 128, 240)
            };
            canvas.DrawCircle(info.Width / 2, info.Height / 2, 100, fillPaint);
        }
    }
}
