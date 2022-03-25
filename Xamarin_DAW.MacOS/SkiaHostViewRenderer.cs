using System.Linq;
using AppKit;
using CoreGraphics;
using Foundation;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Xamarin_DAW.AndroidUI;

[assembly: ExportRenderer(typeof(SkiaViewHost), typeof(Xamarin_DAW.MacOS.SkiaViewHostRenderer))]
namespace Xamarin_DAW.MacOS
{
    public class SkiaViewHostRenderer : SKGLViewRenderer
    {
        public SkiaViewHostRenderer()
        {
            AcceptsTouchEvents = true;
        }

        public override void TouchesBeganWithEvent(NSEvent theEvent)
        {
            base.TouchesBeganWithEvent(theEvent);

            SkiaViewHost frame = Element as SkiaViewHost;
            if (frame != null)
            {
                NSSet touches = theEvent.TouchesMatchingPhase(NSTouchPhase.Began, (NSView)Self);
                int numberOfTouches = touches.Count();
                for (int i = 0; i < numberOfTouches; i++)
                {
                    NSTouch touch = (NSTouch)touches.ElementAt(i);

                    CGRect viewRect = ConvertRectToView(Frame, null);
                    CGPoint mousePoint = theEvent.LocationInWindow;
                    mousePoint.X -= viewRect.X;
                    mousePoint.Y -= viewRect.Y;

                    //flip Y so that 0 is at top instead of bottom
                    mousePoint.Y = viewRect.Height - mousePoint.Y;

                    CGPoint fingerLocationInTrackPad = touch.NormalizedPosition;

                    //flip Y so that 0 is at top instead of bottom
                    fingerLocationInTrackPad.Y = 1.0f - fingerLocationInTrackPad.Y;

                    if (frame.multiTouch.tryForcePump())
                    {
                        frame.onTouch();
                    }
                    frame.multiTouch.addTouch(
                        touch.Identity,
                        (float)mousePoint.X,
                        (float)mousePoint.Y,
                        (float)fingerLocationInTrackPad.X, (float)fingerLocationInTrackPad.Y
                    );
                    frame.onTouch();
                }
            }
        }

        public override void TouchesMovedWithEvent(NSEvent theEvent)
        {
            base.TouchesMovedWithEvent(theEvent);

            SkiaViewHost frame = Element as SkiaViewHost;
            if (frame != null)
            {
                NSSet touches = theEvent.TouchesMatchingPhase(NSTouchPhase.Moved, (NSView)Self);
                int numberOfTouches = touches.Count();
                for (int i = 0; i < numberOfTouches; i++)
                {
                    NSTouch touch = (NSTouch)touches.ElementAt(i);

                    CGRect viewRect = ConvertRectToView(Frame, null);
                    CGPoint mousePoint = theEvent.LocationInWindow;
                    mousePoint.X -= viewRect.X;
                    mousePoint.Y -= viewRect.Y;

                    //flip Y so that 0 is at top instead of bottom
                    mousePoint.Y = viewRect.Height - mousePoint.Y;

                    CGPoint fingerLocationInTrackPad = touch.NormalizedPosition;

                    //flip Y so that 0 is at top instead of bottom
                    fingerLocationInTrackPad.Y = 1.0f - fingerLocationInTrackPad.Y;

                    if (frame.multiTouch.moveTouchBatched(
                        touch.Identity,
                        (float)mousePoint.X,
                        (float)mousePoint.Y,
                        (float)fingerLocationInTrackPad.X, (float)fingerLocationInTrackPad.Y
                    ))
                    {
                        frame.onTouch();
                    }
                }
            }
        }

        public override void TouchesEndedWithEvent(NSEvent theEvent)
        {
            base.TouchesEndedWithEvent(theEvent);

            SkiaViewHost frame = Element as SkiaViewHost;
            if (frame != null)
            {
                NSSet touches = theEvent.TouchesMatchingPhase(NSTouchPhase.Ended, (NSView)Self);
                int numberOfTouches = touches.Count();
                for (int i = 0; i < numberOfTouches; i++)
                {
                    NSTouch touch = (NSTouch)touches.ElementAt(i);

                    CGRect viewRect = ConvertRectToView(Frame, null);
                    CGPoint mousePoint = theEvent.LocationInWindow;
                    mousePoint.X -= viewRect.X;
                    mousePoint.Y -= viewRect.Y;

                    //flip Y so that 0 is at top instead of bottom
                    mousePoint.Y = viewRect.Height - mousePoint.Y;

                    CGPoint fingerLocationInTrackPad = touch.NormalizedPosition;

                    //flip Y so that 0 is at top instead of bottom
                    fingerLocationInTrackPad.Y = 1.0f - fingerLocationInTrackPad.Y;

                    if (frame.multiTouch.tryForcePump())
                    {
                        frame.onTouch();
                    }
                    frame.multiTouch.removeTouch(
                        touch.Identity,
                        (float)mousePoint.X,
                        (float)mousePoint.Y,
                        (float)fingerLocationInTrackPad.X, (float)fingerLocationInTrackPad.Y
                    );
                    frame.onTouch();
                }
            }
        }

        public override void TouchesCancelledWithEvent(NSEvent theEvent)
        {
            base.TouchesCancelledWithEvent(theEvent);

            SkiaViewHost frame = Element as SkiaViewHost;
            if (frame != null)
            {
                NSSet touches = theEvent.TouchesMatchingPhase(NSTouchPhase.Cancelled, (NSView)Self);
                int numberOfTouches = touches.Count();
                for (int i = 0; i < numberOfTouches; i++)
                {
                    NSTouch touch = (NSTouch)touches.ElementAt(i);

                    CGRect viewRect = ConvertRectToView(Frame, null);
                    CGPoint mousePoint = theEvent.LocationInWindow;
                    mousePoint.X -= viewRect.X;
                    mousePoint.Y -= viewRect.Y;

                    //flip Y so that 0 is at top instead of bottom
                    mousePoint.Y = viewRect.Height - mousePoint.Y;

                    CGPoint fingerLocationInTrackPad = touch.NormalizedPosition;

                    //flip Y so that 0 is at top instead of bottom
                    fingerLocationInTrackPad.Y = 1.0f - fingerLocationInTrackPad.Y;

                    if (frame.multiTouch.tryForcePump())
                    {
                        frame.onTouch();
                    }
                    frame.multiTouch.cancelTouch(
                        touch.Identity,
                        (float)mousePoint.X,
                        (float)mousePoint.Y,
                        (float)fingerLocationInTrackPad.X, (float)fingerLocationInTrackPad.Y
                    );
                    frame.onTouch();
                }
            }
        }
    }
}
