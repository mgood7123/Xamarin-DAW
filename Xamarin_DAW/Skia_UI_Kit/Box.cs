using System;
using SkiaSharp;

namespace Xamarin_DAW.Skia_UI_Kit
{
    public class Box : View
    {
        public Box()
        {
        }

        public SKRect r = new(0, 0, 100, 100);
        public SKColorF color = SKColors.Red;

        protected override void OnDraw(SKCanvas canvas)
        {
            Console.WriteLine("Box OnPaintSurface");
            canvas.DrawRect(r, new SKPaint() { ColorF = color });
        }
    }
}
