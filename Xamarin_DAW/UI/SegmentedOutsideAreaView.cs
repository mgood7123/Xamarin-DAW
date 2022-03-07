using System;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Xamarin_DAW.UI
{
    [ContentProperty("AreaContent")]
    public class SegmentedOutsideAreaView : AbsoluteLayout
    {
        public static readonly BindableProperty AreaSizeProperty = BindableProperty.Create("AreaSize", typeof(double), typeof(SegmentedOutsideAreaView), default(double), propertyChanged: OnAreaSizeChanged);

        private static void OnAreaSizeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            SegmentedOutsideAreaView view = (SegmentedOutsideAreaView)bindable;

            // we do not need to resize AreaContent
            double v = (double)newValue;

            // value cannot go below 0
            double areaSize = v < 0 ? 0 : v;
            view.AreaSize = areaSize;

            view.WidthBeingSet = true;
            view.WidthRequest = view.AreaWidth + (areaSize * 2);

            view.HeightBeingSet = true;
            view.HeightRequest = view.AreaHeight + (areaSize * 2);

            view.UpdateRects();
        }

        public double AreaSize
        {
            get => (double)GetValue(AreaSizeProperty);
            set => SetValue(AreaSizeProperty, value);
        }

        double AreaWidth;
        double AreaHeight;

        public static readonly BindableProperty AreaContentProperty = BindableProperty.Create("AreaContent", typeof(View), typeof(SegmentedOutsideAreaView), default(View), propertyChanged: onAreaContentChanged);

        private static void onAreaContentChanged(BindableObject bindable, object oldValue, object newValue)
        {
            SegmentedOutsideAreaView view = (SegmentedOutsideAreaView)bindable;
            view.AreaContainer.Content = (View)newValue;
        }

        public View AreaContent
        {
            get => (View)GetValue(AreaContentProperty);
            set => SetValue(AreaContentProperty, value);
        }

        private readonly ContentView AreaContainer;
        Rectangle AreaContainerBounds;

        public readonly BoxView CornerTopLeft = new();
        Rectangle CornerTopLeftBounds;
        public readonly BoxView Top = new();
        Rectangle TopBounds;
        public readonly BoxView CornerTopRight = new();
        Rectangle CornerTopRightBounds;
        public readonly BoxView Right = new();
        Rectangle RightBounds;
        public readonly BoxView CornerBottomRight = new();
        Rectangle CornerBottomRightBounds;
        public readonly BoxView Bottom = new();
        Rectangle BottomBounds;
        public readonly BoxView CornerBottomLeft = new();
        Rectangle CornerBottomLeftBounds;
        public readonly BoxView Left = new();
        Rectangle LeftBounds;

        void UpdateRects()
        {
            CornerTopLeftBounds = new(0, 0, AreaSize, AreaSize);
            SetLayoutBounds(CornerTopLeft, CornerTopLeftBounds);

            TopBounds = new(AreaSize, 0, AreaWidth, AreaSize);
            SetLayoutBounds(Top, TopBounds);

            CornerTopRightBounds = new(AreaSize + AreaWidth, 0, AreaSize, AreaSize);
            SetLayoutBounds(CornerTopRight, CornerTopRightBounds);

            RightBounds = new(AreaSize + AreaWidth, AreaSize, AreaSize, AreaHeight);
            SetLayoutBounds(Right, RightBounds);

            CornerBottomRightBounds = new(AreaSize + AreaWidth, AreaSize + AreaHeight, AreaSize, AreaSize);
            SetLayoutBounds(CornerBottomRight, CornerBottomRightBounds);

            BottomBounds = new(AreaSize, AreaSize + AreaHeight, AreaWidth, AreaSize);
            SetLayoutBounds(Bottom, BottomBounds);

            CornerBottomLeftBounds = new(0, AreaSize + AreaHeight, AreaSize, AreaSize);
            SetLayoutBounds(CornerBottomLeft, CornerBottomLeftBounds);

            LeftBounds = new(0, AreaSize, AreaSize, AreaHeight);
            SetLayoutBounds(Left, LeftBounds);

            AreaContainerBounds = new(AreaSize, AreaSize, AreaWidth, AreaHeight);
            SetLayoutBounds(AreaContainer, AreaContainerBounds);
        }

        public SegmentedOutsideAreaView()
        {
            AreaContainer = new();
            Children.Add(AreaContainer);
            Children.Add(CornerTopLeft);
            Children.Add(Top);
            Children.Add(CornerTopRight);
            Children.Add(Right);
            Children.Add(CornerBottomRight);
            Children.Add(Bottom);
            Children.Add(CornerBottomLeft);
            Children.Add(Left);
        }

        bool HeightBeingSet = false;
        bool WidthBeingSet = false;

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName == WidthRequestProperty.PropertyName)
            {
                if (!WidthBeingSet)
                {
                    WidthBeingSet = true;
                    AreaWidth = WidthRequest;
                    AreaContainer.WidthRequest = AreaWidth;
                    WidthRequest += (AreaSize * 2);
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
                    AreaHeight = HeightRequest;
                    AreaContainer.HeightRequest = AreaHeight;
                    HeightRequest += (AreaSize * 2);
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
