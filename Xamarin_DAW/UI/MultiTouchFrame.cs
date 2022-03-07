using System;
using Xamarin.Forms;
using Xamarin_DAW.UI.Utils;

namespace Xamarin_DAW.UI
{
    public class MultiTouchFrame : Frame
    {
        public MultiTouch multiTouch;

        public MultiTouchFrame()
        {
            multiTouch = new MultiTouch();
            multiTouch.setMaxSupportedTouches(10);
            multiTouch.throw_on_error = false;
            Content = new Label()
            {
                FontSize = 8,
                VerticalTextAlignment = TextAlignment.Start,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Start,
                HorizontalTextAlignment = TextAlignment.Start
            };
        }

        public void onTouch()
        {
            MultiTouch.TouchData touchData = multiTouch.getTouchAtCurrentIndex();
            ((Label)Content).Text =
                "mouse: " + touchData.MouseToPointF() + "\n" +
                "delta: " + touchData.DeltaToPointF() + "\n" +
                multiTouch.ToString();

            //string t = multiTouch.ToString();
            //((Label)Content).Text = t;
        }
    }
}
