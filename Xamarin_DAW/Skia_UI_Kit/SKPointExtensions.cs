﻿using SkiaSharp;

namespace Xamarin_DAW.Skia_UI_Kit
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