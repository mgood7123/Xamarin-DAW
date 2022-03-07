using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Xamarin_DAW.UI
{
    [ContentProperty("WindowContent")]
    public class WindowView : AbsoluteLayout
    {
        public static readonly BindableProperty ResizeTouchZoneProperty = BindableProperty.Create("ResizeTouchZone", typeof(double), typeof(WindowView), default(double), propertyChanged: OnResizeTouchZoneChanged);

        private static void OnResizeTouchZoneChanged(BindableObject bindable, object oldValue, object newValue)
        {
            WindowView view = (WindowView)bindable;

            // we do not need to resize WindowContent
            double v = (double)newValue;

            // value cannot go below 0
            double ResizeTouchZone = v < 0 ? 0 : v;
            view.ResizeTouchZone = ResizeTouchZone;

            view.WidthBeingSet = true;
            view.WidthRequest = view.WindowContentContainerWidth + (ResizeTouchZone * 2);

            view.HeightBeingSet = true;
            view.HeightRequest = view.WindowContentContainerHeight + (ResizeTouchZone * 2) + view.TitleBarHeight;

            view.UpdateRects();
        }

        public double ResizeTouchZone
        {
            get => (double)GetValue(ResizeTouchZoneProperty);
            set => SetValue(ResizeTouchZoneProperty, value);
        }

        public static readonly BindableProperty TitleBarHeightProperty = BindableProperty.Create("TitleBarHeight", typeof(double), typeof(WindowView), 20.0, propertyChanged: OnTitleBarHeightChanged);

        private static void OnTitleBarHeightChanged(BindableObject bindable, object oldValue, object newValue)
        {
            WindowView view = (WindowView)bindable;

            // we do not need to resize WindowContent
            double v = (double)newValue;

            // value cannot go below 0
            double TitleBarHeight = v < 0 ? 0 : v;
            view.TitleBarHeight = TitleBarHeight;

            view.HeightBeingSet = true;
            view.HeightRequest = view.WindowContentContainerHeight + (view.ResizeTouchZone * 2) + TitleBarHeight;
            view.TitleBar.HeightRequest = TitleBarHeight;

            view.UpdateRects();
        }

        public double TitleBarHeight
        {
            get => (double)GetValue(TitleBarHeightProperty);
            set => SetValue(TitleBarHeightProperty, value);
        }

        public static readonly BindableProperty WindowContentProperty = BindableProperty.Create("WindowContent", typeof(View), typeof(WindowView), default(View), propertyChanged: onWindowContentChanged);

        private static void onWindowContentChanged(BindableObject bindable, object oldValue, object newValue)
        {
            WindowView view = (WindowView)bindable;
            view.WindowContentContainer.Content = (View)newValue;
        }

        public View WindowContent
        {
            get => (View)GetValue(WindowContentProperty);
            set => SetValue(WindowContentProperty, value);
        }

        double WindowContentContainerWidth;
        double WindowContentContainerHeight;

        public WindowManager WindowManager { get => windowManager; internal set => windowManager = value; }

        readonly ContentView WindowContentContainer;
        Rectangle WindowContentContainerBounds;

        public readonly BoxView TopLeft = new();
        Rectangle CornerTopLeftBounds;
        public readonly BoxView Top = new();
        Rectangle TopBounds;
        public readonly BoxView TopRight = new();
        Rectangle CornerTopRightBounds;
        public readonly BoxView Right = new();
        Rectangle RightBounds;
        public readonly BoxView BottomRight = new();
        Rectangle CornerBottomRightBounds;
        public readonly BoxView Bottom = new();
        Rectangle BottomBounds;
        public readonly BoxView BottomLeft = new();
        Rectangle CornerBottomLeftBounds;
        public readonly BoxView Left = new();
        Rectangle LeftBounds;

        readonly AbsoluteLayout TitleBar = new();
        Rectangle TitleBarBounds;

        void UpdateRects()
        {
            CornerTopLeftBounds = new(0, 0, ResizeTouchZone, ResizeTouchZone);
            SetLayoutBounds(TopLeft, CornerTopLeftBounds);

            TopBounds = new(ResizeTouchZone, 0, WindowContentContainerWidth, ResizeTouchZone);
            SetLayoutBounds(Top, TopBounds);

            CornerTopRightBounds = new(ResizeTouchZone + WindowContentContainerWidth, 0, ResizeTouchZone, ResizeTouchZone);
            SetLayoutBounds(TopRight, CornerTopRightBounds);

            RightBounds = new(ResizeTouchZone + WindowContentContainerWidth, ResizeTouchZone, ResizeTouchZone, WindowContentContainerHeight + TitleBarHeight);
            SetLayoutBounds(Right, RightBounds);

            CornerBottomRightBounds = new(ResizeTouchZone + WindowContentContainerWidth, ResizeTouchZone + WindowContentContainerHeight + TitleBarHeight, ResizeTouchZone, ResizeTouchZone);
            SetLayoutBounds(BottomRight, CornerBottomRightBounds);

            BottomBounds = new(ResizeTouchZone, ResizeTouchZone + WindowContentContainerHeight + TitleBarHeight, WindowContentContainerWidth, ResizeTouchZone);
            SetLayoutBounds(Bottom, BottomBounds);

            CornerBottomLeftBounds = new(0, ResizeTouchZone + WindowContentContainerHeight + TitleBarHeight, ResizeTouchZone, ResizeTouchZone);
            SetLayoutBounds(BottomLeft, CornerBottomLeftBounds);

            LeftBounds = new(0, ResizeTouchZone, ResizeTouchZone, WindowContentContainerHeight + TitleBarHeight);
            SetLayoutBounds(Left, LeftBounds);

            TitleBarBounds = new(ResizeTouchZone, ResizeTouchZone, WindowContentContainerWidth, TitleBarHeight);
            SetLayoutBounds(TitleBar, TitleBarBounds);

            WindowContentContainerBounds = new(ResizeTouchZone, ResizeTouchZone + TitleBarHeight, WindowContentContainerWidth, WindowContentContainerHeight);
            SetLayoutBounds(WindowContentContainer, WindowContentContainerBounds);
        }

        WindowViewResizeGestureRecognizer WindowViewResizeGesture;
        PanGestureRecognizer WindowViewMoveGesture;

        public WindowView()
        {
            WindowViewResizeGesture = new(this);
            WindowViewMoveGesture = new();

            WindowContentContainer = new();
            Children.Add(TitleBar);
            Children.Add(WindowContentContainer);
            Children.Add(TopLeft);
            Children.Add(Top);
            Children.Add(TopRight);
            Children.Add(Right);
            Children.Add(BottomRight);
            Children.Add(Bottom);
            Children.Add(BottomLeft);
            Children.Add(Left);

            WindowViewResizeGesture.WindowViewResized += OnWindowResized;

            TopLeft.GestureRecognizers.Add(WindowViewResizeGesture);
            Top.GestureRecognizers.Add(WindowViewResizeGesture);
            TopRight.GestureRecognizers.Add(WindowViewResizeGesture);
            Right.GestureRecognizers.Add(WindowViewResizeGesture);
            BottomRight.GestureRecognizers.Add(WindowViewResizeGesture);
            Bottom.GestureRecognizers.Add(WindowViewResizeGesture);
            BottomLeft.GestureRecognizers.Add(WindowViewResizeGesture);
            Left.GestureRecognizers.Add(WindowViewResizeGesture);

            WindowViewMoveGesture.PanUpdated += OnWindowMoved;

            TitleBar.Background = Color.Gray;

            TitleBar.GestureRecognizers.Add(WindowViewMoveGesture);

            //label = new Label();
            //WindowContent = new ScrollView
            //{
                //Orientation = ScrollOrientation.Both,
                //HorizontalScrollBarVisibility = ScrollBarVisibility.Always,
                //VerticalScrollBarVisibility = ScrollBarVisibility.Always,
                //Content = label
            //};
        }

        Label label;

        private void OnWindowMoved(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Running:
                    double newX = e.TotalX;
                    // on android the Y value appears to be flipped
                    // moving down actually moves up
                    // vice versa on mac os
                    double newY = Plugin.Is_Android ? e.TotalY : -e.TotalY;

                    double nextX = (X + newX);
                    double nextY = (Y + newY);

                    double requiredX;
                    if ((nextX + ResizeTouchZone) < 0)
                    {
                        requiredX = (-X) - ResizeTouchZone;
                    }
                    else
                    {
                        if (((nextX + Width) - ResizeTouchZone) > windowManager.Width)
                        {
                            requiredX = ((windowManager.Width - Width) - X) + ResizeTouchZone;
                        }
                        else
                        {
                            requiredX = newX;
                        }
                    }
                    double requiredY;
                    if ((nextY + ResizeTouchZone) < 0)
                    {
                        requiredY = (-Y) - ResizeTouchZone;
                    }
                    else
                    {
                        if ((nextY + Height) - ResizeTouchZone > windowManager.Height)
                        {
                            requiredY = ((windowManager.Height - Height) - Y) + ResizeTouchZone;
                        }
                        else
                        {
                            requiredY = newY;
                        }
                    }

                    TranslationX = requiredX;
                    // on android the Y value appears to be flipped
                    // moving down actually moves up
                    // vice versa on mac os
                    TranslationY = requiredY;
                    //label.Text =
                    //    "Device Density: " + Plugin.ScreenDensity + "\n" +
                    //    "X: " + X + "\n" +
                    //    "Y: " + Y + "\n" +
                    //    "DeltaX: " + newX + "\n" +
                    //    "DeltaY: " + newY + "\n" +
                    //    "next X: " + (X + newX) + "\n" +
                    //    "next Y: " + (Y + newY) + "\n" +
                    //    "DeltaX Corrected: " + requiredX + "\n" +
                    //    "DeltaY Corrected: " + requiredY + "\n" +
                    //    "next X Corrected: " + (X + requiredX) + "\n" +
                    //    "next Y Corrected: " + (Y + requiredY) + "\n" +
                    //    ""
                    //    ;
                    break;
                case GestureStatus.Canceled:
                    TranslationX = 0;
                    TranslationY = 0;
                    break;
                case GestureStatus.Completed:
                    windowManager.MoveChildTo(this, X + TranslationX, Y + TranslationY);
                    TranslationX = 0;
                    TranslationY = 0;
                    break;
            }
        }

        private void OnWindowResized(object sender, WindowViewResizedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Running:
                    double newX = e.TotalX;
                    // on android the Y value appears to be flipped
                    // moving down actually moves up
                    // vice versa on mac os
                    double newY = Plugin.Is_Android ? e.TotalY : -e.TotalY;

                    double nextX = (X + newX);
                    double nextY = (Y + newY);

                    double requiredX;
                    if ((nextX + ResizeTouchZone) < 0)
                    {
                        requiredX = (-X) - ResizeTouchZone;
                    }
                    else
                    {
                        if (((nextX + Width) - ResizeTouchZone) > windowManager.Width)
                        {
                            requiredX = ((windowManager.Width - Width) - X) + ResizeTouchZone;
                        }
                        else
                        {
                            requiredX = newX;
                        }
                    }
                    double requiredY;
                    if ((nextY + ResizeTouchZone) < 0)
                    {
                        requiredY = (-Y) - ResizeTouchZone;
                    }
                    else
                    {
                        if ((nextY + Height) - ResizeTouchZone > windowManager.Height)
                        {
                            requiredY = ((windowManager.Height - Height) - Y) + ResizeTouchZone;
                        }
                        else
                        {
                            requiredY = newY;
                        }
                    }

                    TranslationX = requiredX;
                    // on android the Y value appears to be flipped
                    // moving down actually moves up
                    // vice versa on mac os
                    TranslationY = requiredY;
                    //label.Text =
                    //    "Device Density: " + Plugin.ScreenDensity + "\n" +
                    //    "X: " + X + "\n" +
                    //    "Y: " + Y + "\n" +
                    //    "DeltaX: " + newX + "\n" +
                    //    "DeltaY: " + newY + "\n" +
                    //    "next X: " + (X + newX) + "\n" +
                    //    "next Y: " + (Y + newY) + "\n" +
                    //    "DeltaX Corrected: " + requiredX + "\n" +
                    //    "DeltaY Corrected: " + requiredY + "\n" +
                    //    "next X Corrected: " + (X + requiredX) + "\n" +
                    //    "next Y Corrected: " + (Y + requiredY) + "\n" +
                    //    ""
                    //    ;
                    break;
                case GestureStatus.Canceled:
                    TranslationX = 0;
                    TranslationY = 0;
                    break;
                case GestureStatus.Completed:
                    windowManager.MoveChildTo(this, X + TranslationX, Y + TranslationY);
                    TranslationX = 0;
                    TranslationY = 0;
                    break;
            }
        }

        public class WindowViewResizeGestureRecognizer : PanGestureRecognizer
        {
            private WindowView windowView;
            bool internal_modification;

            public WindowViewResizeGestureRecognizer(WindowView windowView)
            {
                this.windowView = windowView;
                internal_modification = true;
                base.PanUpdated += ForwardPanUpdatedEvent;
                internal_modification = false;
            }

            private void ForwardPanUpdatedEvent(object sender, PanUpdatedEventArgs e)
            {
                switch (e.StatusType)
                {
                    case GestureStatus.Started:
                        WindowViewResized?.Invoke(windowView, new WindowViewResizedEventArgs(e.StatusType, e.GestureId, windowView, (Element)sender));
                        break;
                    case GestureStatus.Running:
                        WindowViewResized?.Invoke(windowView, new WindowViewResizedEventArgs(e.StatusType, e.GestureId, windowView, (Element)sender, e.TotalX, e.TotalY));
                        break;
                    case GestureStatus.Completed:
                        WindowViewResized?.Invoke(windowView, new WindowViewResizedEventArgs(e.StatusType, e.GestureId, windowView, (Element)sender));
                        break;
                    case GestureStatus.Canceled:
                        WindowViewResized?.Invoke(windowView, new WindowViewResizedEventArgs(e.StatusType, e.GestureId, windowView, (Element)sender));
                        break;
                }
            }

            public event EventHandler<WindowViewResizedEventArgs> WindowViewResized;

            // hide member
            [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
            public new event EventHandler<PanUpdatedEventArgs> PanUpdated
            {
                add
                {
                    if (internal_modification)
                    {
                        base.PanUpdated += value;
                    }
                    else
                    {
                        throw new NotSupportedException("Attempting to add method via PanUpdated (PanUpdated += method), please add via WindowViewResized instead (WindowViewResized += method)");
                    }
                }
                remove
                {
                    throw new NotSupportedException("Attempting to remove method via PanUpdated (PanUpdated += method), please remove via WindowViewResized instead (WindowViewResized += method)");
                }
            }
        }

        public class WindowViewResizedEventArgs : EventArgs
        {
            public WindowViewResizedEventArgs(GestureStatus type, int gestureId, WindowView windowView, Element sender, double totalx, double totaly) : this(type, gestureId, windowView, sender)
            {
                TotalX = totalx;
                TotalY = totaly;
            }

            public WindowViewResizedEventArgs(GestureStatus type, int gestureId, WindowView windowView, Element sender)
            {
                StatusType = type;
                GestureId = gestureId;
                ResizeZone = new(windowView, sender);
            }

            public struct ResizeZoneId
            {
                public ResizeZoneId(WindowView windowView, Element sender) : this()
                {
                    if (sender == windowView.TopLeft) TopLeft = true;
                    else if (sender == windowView.Top) Top = true;
                    else if (sender == windowView.TopRight) TopRight = true;
                    else if (sender == windowView.Right) Right = true;
                    else if (sender == windowView.BottomRight) BottomRight = true;
                    else if (sender == windowView.Bottom) Bottom = true;
                    else if (sender == windowView.BottomLeft) BottomLeft = true;
                    else if (sender == windowView.Left) Left = true;
                    else throw new ArgumentException("invalid sender");
                }

                public bool TopLeft { get; }
                public bool Top { get; }
                public bool Right { get; }
                public bool TopRight { get; }
                public bool BottomRight { get; }
                public bool Bottom { get; }
                public bool BottomLeft { get; }
                public bool Left { get; }
            }

            public ResizeZoneId ResizeZone { get; }

            public int GestureId { get; }

            public GestureStatus StatusType { get; }

            public double TotalX { get; }

            public double TotalY { get; }
        }

        bool HeightBeingSet = false;
        bool WidthBeingSet = false;
        private WindowManager windowManager;

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName == WidthRequestProperty.PropertyName)
            {
                if (!WidthBeingSet)
                {
                    WidthBeingSet = true;
                    WindowContentContainerWidth = WidthRequest;
                    WindowContentContainer.WidthRequest = WindowContentContainerWidth;
                    WidthRequest += (ResizeTouchZone * 2);
                    UpdateRects();
                }
                else
                {
                    WidthBeingSet = false;
                }
            }

            if (propertyName == HeightRequestProperty.PropertyName)
            {
                if (!HeightBeingSet)
                {
                    HeightBeingSet = true;
                    WindowContentContainerHeight = HeightRequest;
                    WindowContentContainer.HeightRequest = WindowContentContainerHeight;
                    HeightRequest += (ResizeTouchZone * 2) + TitleBarHeight;
                    UpdateRects();
                }
                else
                {
                    HeightBeingSet = false;
                }
            }

            base.OnPropertyChanged(propertyName);
        }
    }
}
