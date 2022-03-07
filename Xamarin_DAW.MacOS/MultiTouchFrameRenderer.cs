using System;
using System.Linq;
using AppKit;
using CoreGraphics;
using Foundation;
using Xamarin.Forms;
using Xamarin.Forms.Platform.MacOS;

[assembly: ExportRenderer(typeof(Xamarin_DAW.UI.MultiTouchFrame), typeof(Xamarin_DAW.MacOS.MultiTouchFrameRenderer))]
namespace Xamarin_DAW.MacOS
{
    public class MultiTouchFrameRenderer : FrameRenderer
    {
        public MultiTouchFrameRenderer()
        {
            Console.WriteLine("MultiTouchFrameRenderer");
            AcceptsTouchEvents = true;
        }

        public override void TouchesBeganWithEvent(NSEvent theEvent)
        {
            Console.WriteLine("TouchesBeganWithEvent");
            base.TouchesBeganWithEvent(theEvent);

            UI.MultiTouchFrame frame = Element as UI.MultiTouchFrame;
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

                    fingerLocationInTrackPad.X *= viewRect.Width;
                    fingerLocationInTrackPad.Y *= viewRect.Height;

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
            Console.WriteLine("TouchesMovedWithEvent");
            base.TouchesMovedWithEvent(theEvent);

            UI.MultiTouchFrame frame = Element as UI.MultiTouchFrame;
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

                    fingerLocationInTrackPad.X *= viewRect.Width;
                    fingerLocationInTrackPad.Y *= viewRect.Height;

                    frame.multiTouch.moveTouch(
                        touch.Identity,
                        (float)mousePoint.X,
                        (float)mousePoint.Y,
                        (float)fingerLocationInTrackPad.X, (float)fingerLocationInTrackPad.Y
                    );
                    frame.onTouch();
                }
            }
        }

        public override void TouchesEndedWithEvent(NSEvent theEvent)
        {
            Console.WriteLine("TouchesEndedWithEvent");
            base.TouchesEndedWithEvent(theEvent);

            UI.MultiTouchFrame frame = Element as UI.MultiTouchFrame;
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

                    fingerLocationInTrackPad.X *= viewRect.Width;
                    fingerLocationInTrackPad.Y *= viewRect.Height;

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
            Console.WriteLine("TouchesCancelledWithEvent");
            base.TouchesCancelledWithEvent(theEvent);

            UI.MultiTouchFrame frame = Element as UI.MultiTouchFrame;
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

                    fingerLocationInTrackPad.X *= viewRect.Width;
                    fingerLocationInTrackPad.Y *= viewRect.Height;

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
