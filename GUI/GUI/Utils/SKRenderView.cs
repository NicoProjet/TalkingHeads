using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Text;

namespace GUI.Utils
{
    class SKRenderView : SKCanvasView
    {
        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);
            new ImageRenderer().PaintSurface(e.Surface, e.Info);
        }
    }
}
