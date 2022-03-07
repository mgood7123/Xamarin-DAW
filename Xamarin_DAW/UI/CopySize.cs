using System;
using Xamarin.Forms;

namespace Xamarin_DAW.UI
{
    public class CopySize : ContentView
    {
        private Func<View> viewGetter;

        public CopySize()
        {
        }

        public CopySize(View view)
        {
            ViewGetter = view;
        }

        public View ViewGetter
        {
            get => viewGetter == null ? null : viewGetter();

            set => viewGetter = () => value;
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            View view = ViewGetter;
            base.LayoutChildren(x, y, view == null ? width : view.Width, view == null ? height : view.Height);
        }
    }
}