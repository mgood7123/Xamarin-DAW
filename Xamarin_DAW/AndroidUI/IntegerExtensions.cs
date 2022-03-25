using System;
namespace Xamarin_DAW.AndroidUI
{
    public static class IntegerExtensions
    {
        public static int dipToPx(this int dip)
        {
            return (int)(Plugin.ScreenDensityAsFloat * dip + 0.5f);
        }
    }
}
