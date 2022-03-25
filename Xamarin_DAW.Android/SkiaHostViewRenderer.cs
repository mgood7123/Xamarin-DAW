using Android.Content;
using Android.Util;
using Android.Views;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Xamarin_DAW.AndroidUI;

[assembly: ExportRenderer(typeof(SkiaViewHost), typeof(Xamarin_DAW.Android_.SkiaViewHostRenderer))]
namespace Xamarin_DAW.Android_
{
    public class SkiaViewHostRenderer : SKGLViewRenderer
    {
        public SkiaViewHostRenderer(Context context) : base(context)
        {
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            SkiaViewHost frame = Element as SkiaViewHost;
            if (frame != null)
            {
                int[] xy = new int[2];
                GetLocationOnScreen(xy);

                DisplayMetrics m = new DisplayMetrics();
                Context.Display.GetRealMetrics(m);

                int height = m.HeightPixels;
                int width = m.WidthPixels;

                int viewX = xy[0];
                int viewY = xy[1];

                MotionEventActions maskedAction_ = e.ActionMasked;

                int pointerIndex = e.ActionIndex;

                switch (maskedAction_)
                {
                    case MotionEventActions.Down:
                    case MotionEventActions.PointerDown:
                        {
                            long identity = e.GetPointerId(pointerIndex);
                            long time = e.EventTime;
                            float x = viewX + e.GetX(pointerIndex);
                            float y = viewY + e.GetY(pointerIndex);
                            float normalized_X = x / width;
                            float normalized_Y = y / height;
                            float size = e.GetSize(pointerIndex);
                            float pressure = e.GetPressure(pointerIndex);
                            AndroidUI.Log.d("TAG", "ACTION_DOWN           pointer index : " + pointerIndex);
                            AndroidUI.Log.d("TAG", "ACTION_DOWN           pointer id    : " + identity);
                            if (frame.multiTouch.tryForcePump())
                            {
                                frame.onTouch();
                            }
                            frame.multiTouch.addTouch(
                                identity, time,
                                x, y,
                                normalized_X, normalized_Y,
                                size, pressure
                            );
                            frame.onTouch();
                            break;
                        }
                    case MotionEventActions.Move:
                        {
                            int historySize = e.HistorySize;
                            int pointerCount = e.PointerCount;
                            for (int h = 0; h < historySize; h++)
                            {
                                long time = e.GetHistoricalEventTime(h);
                                for (int p = 0; p < pointerCount; p++)
                                {
                                    long identity = e.GetPointerId(p);
                                    float x = viewX + e.GetHistoricalX(p, h);
                                    float y = viewY + e.GetHistoricalY(p, h);
                                    float normalized_X = x / width;
                                    float normalized_Y = y / height;
                                    float size = e.GetHistoricalSize(p, h);
                                    float pressure = e.GetHistoricalPressure(p, h);
                                    AndroidUI.Log.d("TAG", "ACTION_MOVE   HISTORY pointer index : " + p);
                                    AndroidUI.Log.d("TAG", "ACTION_MOVE   HISTORY pointer id    : " + identity);
                                    if (frame.multiTouch.moveTouchBatched(
                                        identity, time,
                                        x, y,
                                        normalized_X, normalized_Y,
                                        size, pressure
                                    ))
                                    {
                                        frame.onTouch();
                                    }
                                }
                            }

                            long time_ = e.EventTime;
                            for (int p = 0; p < pointerCount; p++)
                            {
                                long identity = e.GetPointerId(p);
                                float x = viewX + e.GetX(p);
                                float y = viewY + e.GetY(p);
                                float normalized_X = x / width;
                                float normalized_Y = y / height;
                                float size = e.GetSize(p);
                                float pressure = e.GetPressure(p);
                                AndroidUI.Log.d("TAG", "ACTION_MOVE           pointer index : " + p);
                                AndroidUI.Log.d("TAG", "ACTION_MOVE           pointer id    : " + identity);
                                if (frame.multiTouch.moveTouchBatched(
                                    identity, time_,
                                    x, y,
                                    normalized_X, normalized_Y,
                                    size, pressure
                                ))
                                {
                                    frame.onTouch();
                                }
                            }

                            if (frame.multiTouch.tryForcePump())
                            {
                                frame.onTouch();
                            }
                            break;
                        }
                    case MotionEventActions.Up:
                    case MotionEventActions.PointerUp:
                        {
                            long identity = e.GetPointerId(pointerIndex);
                            long time = e.EventTime;
                            float x = viewX + e.GetX(pointerIndex);
                            float y = viewY + e.GetY(pointerIndex);
                            float normalized_X = x / width;
                            float normalized_Y = y / height;
                            float size = e.GetSize(pointerIndex);
                            float pressure = e.GetPressure(pointerIndex);
                            AndroidUI.Log.d("TAG", "ACTION_UP             pointer index : " + pointerIndex);
                            AndroidUI.Log.d("TAG", "ACTION_UP             pointer id    : " + identity);
                            if (frame.multiTouch.tryForcePump())
                            {
                                frame.onTouch();
                            }
                            frame.multiTouch.removeTouch(
                                identity, time,
                                x, y,
                                normalized_X, normalized_Y,
                                size, pressure
                            );
                            frame.onTouch();
                            break;
                        }
                    case MotionEventActions.Cancel:
                        {
                            long identity = e.GetPointerId(pointerIndex);
                            long time = e.EventTime;
                            float x = viewX + e.GetX(pointerIndex);
                            float y = viewY + e.GetY(pointerIndex);
                            float normalized_X = x / width;
                            float normalized_Y = y / height;
                            float size = e.GetSize(pointerIndex);
                            float pressure = e.GetPressure(pointerIndex);
                            AndroidUI.Log.d("TAG", "ACTION_CANCEL         pointer index : " + pointerIndex);
                            AndroidUI.Log.d("TAG", "ACTION_CANCEL         pointer id    : " + identity);
                            if (frame.multiTouch.tryForcePump())
                            {
                                frame.onTouch();
                            }
                            frame.multiTouch.cancelTouch(
                                identity, time,
                                x, y,
                                normalized_X, normalized_Y,
                                size, pressure
                            );
                            frame.onTouch();
                            break;
                        }
                }
                AndroidUI.Log.d("TAG", "touch                       :\n" + frame.multiTouch.ToString() + "\n");
                if (false)
                {


                    // get masked (not specific to a pointer) action
                    MotionEventActions maskedAction = e.ActionMasked;

                    switch (maskedAction)
                    {

                        case MotionEventActions.Down:
                            {
                                int p = 0;
                                object identity = e.GetPointerId(p);
                                long time = e.EventTime;
                                float x = viewX + e.GetX(p);
                                float y = viewY + e.GetY(p);
                                float normalized_X = x / width;
                                float normalized_Y = y / height;
                                float size = e.GetSize(p);
                                float pressure = e.GetPressure(p);
                                frame.multiTouch.addTouch(
                                    identity, time,
                                    x, y,
                                    normalized_X, normalized_Y,
                                    size, pressure
                                );
                                frame.onTouch();
                                break;
                            }
                        case MotionEventActions.PointerDown:
                            {
                                int p = e.GetPointerId(pointerIndex);
                                object identity = e.GetPointerId(p);
                                long time = e.EventTime;
                                float x = viewX + e.GetX(p);
                                float y = viewY + e.GetY(p);
                                float normalized_X = x / width;
                                float normalized_Y = y / height;
                                float size = e.GetSize(p);
                                float pressure = e.GetPressure(p);
                                frame.multiTouch.addTouch(
                                    identity, time,
                                    x, y,
                                    normalized_X, normalized_Y,
                                    size, pressure
                                );
                                frame.onTouch();
                                break;
                            }
                        case MotionEventActions.Move:
                            {
                                int historySize = e.HistorySize;
                                int pointerCount = e.PointerCount;
                                for (int h = 0; h < historySize; h++)
                                {
                                    long time = e.GetHistoricalEventTime(h);
                                    for (int p = 0; p < pointerCount; p++)
                                    {
                                        object identity = e.GetPointerId(p);
                                        float x = viewX + e.GetHistoricalX(p, h);
                                        float y = viewY + e.GetHistoricalY(p, h);
                                        float normalized_X = x / width;
                                        float normalized_Y = y / height;
                                        float size = e.GetHistoricalSize(p, h);
                                        float pressure = e.GetHistoricalPressure(p, h);
                                        frame.multiTouch.moveTouch(
                                            identity, time,
                                            x, y,
                                            normalized_X, normalized_Y,
                                            size, pressure
                                        );
                                        frame.onTouch();
                                    }
                                }

                                for (int p = 0; p < pointerCount; p++)
                                {
                                    object identity = e.GetPointerId(p);
                                    long time = e.EventTime;
                                    float x = viewX + e.GetX(p);
                                    float y = viewY + e.GetY(p);
                                    float normalized_X = x / width;
                                    float normalized_Y = y / height;
                                    float size = e.GetSize(p);
                                    float pressure = e.GetPressure(p);
                                    frame.multiTouch.moveTouch(
                                        identity, time,
                                        x, y,
                                        normalized_X, normalized_Y,
                                        size, pressure
                                    );
                                    frame.onTouch();
                                }
                                break;
                            }
                        case MotionEventActions.Up:
                            {
                                int p = 0;
                                object identity = e.GetPointerId(p);
                                long time = e.EventTime;
                                float x = viewX + e.GetX(p);
                                float y = viewY + e.GetY(p);
                                float normalized_X = x / width;
                                float normalized_Y = y / height;
                                float size = e.GetSize(p);
                                float pressure = e.GetPressure(p);
                                frame.multiTouch.removeTouch(
                                    identity, time,
                                    x, y,
                                    normalized_X, normalized_Y,
                                    size, pressure
                                );
                                frame.onTouch();
                                break;
                            }
                        case MotionEventActions.PointerUp:
                            {
                                int p = e.GetPointerId(pointerIndex);
                                object identity = e.GetPointerId(p);
                                long time = e.EventTime;
                                float x = viewX + e.GetX(p);
                                float y = viewY + e.GetY(p);
                                float normalized_X = x / width;
                                float normalized_Y = y / height;
                                float size = e.GetSize(p);
                                float pressure = e.GetPressure(p);
                                frame.multiTouch.removeTouch(
                                    identity, time,
                                    x, y,
                                    normalized_X, normalized_Y,
                                    size, pressure
                                );
                                frame.onTouch();
                                break;
                            }
                        case MotionEventActions.Cancel:
                            {
                                int pointerCount = e.PointerCount;
                                for (int p = 0; p < pointerCount; p++)
                                {
                                    object identity = e.GetPointerId(p);
                                    long time = e.EventTime;
                                    float x = viewX + e.GetX(p);
                                    float y = viewY + e.GetY(p);
                                    float normalized_X = x / width;
                                    float normalized_Y = y / height;
                                    float size = e.GetSize(p);
                                    float pressure = e.GetPressure(p);
                                    frame.multiTouch.cancelTouch(
                                        identity, time,
                                        x, y,
                                        normalized_X, normalized_Y,
                                        size, pressure
                                    );
                                    frame.onTouch();
                                }
                                break;
                            }
                    }
                }
            }
            return true;
        }
    }
}
