using System;
namespace Xamarin_DAW.Skia_UI_Kit
{
    public static class NanoTime
    {
        public static long nanoTime()
        {
            long nano = 10000L * System.Diagnostics.Stopwatch.GetTimestamp();
            nano /= TimeSpan.TicksPerMillisecond;
            nano *= 100L;
            return nano;
        }
    }
}
