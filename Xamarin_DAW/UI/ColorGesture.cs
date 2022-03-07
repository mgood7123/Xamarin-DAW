using System;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;

namespace Xamarin_DAW.UI
{
    public class ColorGesture
    {
        public readonly ClickGestureRecognizer gestureRecognizer = new();
        Random random = new();

        public ColorGesture()
        {
            gestureRecognizer.Clicked += (obj, args) =>
            {
                ClickedEventArgs t = (ClickedEventArgs)args;
                ((VisualElement)obj).Background = Color.FromUint((uint)random.Next()).WithAlpha(1.0);
            };
        }
    }
}
