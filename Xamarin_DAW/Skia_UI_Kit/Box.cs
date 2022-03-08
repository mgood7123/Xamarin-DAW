using System;
using SkiaSharp;

namespace Xamarin_DAW.Skia_UI_Kit
{
    public class Box : View
    {
        public Box()
        {
            setWillDraw(true);
        }

        SKPaint paint = new SKPaint() { ColorF = SKColors.Red };

        protected override void OnDraw(SKCanvas canvas)
        {
            Console.WriteLine("Box OnPaintSurface");
            canvas.DrawRect(new SKRect(0, 0, getWidth(), getHeight()), paint);
        }
    }
}
