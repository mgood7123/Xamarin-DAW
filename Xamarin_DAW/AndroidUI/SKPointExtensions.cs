using SkiaSharp;

namespace Xamarin_DAW.AndroidUI
{
    public static class SKPointExtensions
    {
        public static void Set(this ref SKPoint point, float x, float y)
        {
            point.X = x;
            point.Y = y;
        }
    }
}