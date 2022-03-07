using System;
using Xamarin.Forms;

namespace Xamarin_DAW.UI
{
    public partial class WindowManager : AbsoluteLayout
    {
        public WindowManager()
        {
            var b = new BoxView
            {
                Color = Color.Red
            };

            var r = new ContentView
            {
                Content = b
            };

            Children.Add(
                new WindowView()
                {
                    Background = Color.Aqua,
                    ResizeTouchZone = 20,
                    WidthRequest = 200,
                    HeightRequest = 200
                }
            );

            Children.Add(
                new Window()
                {
                    WidthRequest = 200,
                    HeightRequest = 200
                }
            );
        }

        public void MoveChildTo(View child, double x, double y)
        {
            Rectangle r = new Rectangle(x, y, child.Width, child.Height);
            SetLayoutBounds(child, r);
        }

        protected override void OnChildAdded(Element child)
        {
            Console.WriteLine("Child added");
            if (child is WindowView)
            {
                WindowView w = (WindowView)child;
                w.WindowManager = this;
                base.OnChildAdded(child);
                MoveChildTo(w, 0 - w.ResizeTouchZone, 0 - w.ResizeTouchZone);
            }
            else if (child is Window)
            {
                Window w = (Window)child;
                w.WindowManager = this;
                base.OnChildAdded(child);
            }
            else
            {
                if (child is View)
                {
                    OnChildAdded(new WindowView()
                    {
                        WindowContent = (View)child
                    });
                }
                else
                {
                    base.OnChildAdded(child);
                }
            }
        }
    }
}
