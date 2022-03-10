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

        public SKPaint paint = new SKPaint() { Color = new SKColor(50, 50, 50) };

        protected override void onDraw(SKCanvas canvas)
        {
            Console.WriteLine("Box OnPaintSurface");

            // per android, canvas.getWidth/getHeight return screen width/height and NOT view width/height

            canvas.DrawRect(new SKRect(0, 0, getWidth(), getHeight()), paint);
        }
    }
}
