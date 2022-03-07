using System;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Xamarin_DAW.UI
{
    [ContentProperty("AreaContent")]
    public class OutsideAreaView : ContentView
    {
        public static readonly BindableProperty AreaSizeProperty = BindableProperty.Create("AreaSize", typeof(double), typeof(OutsideAreaView), default(double), propertyChanged: OnAreaSizeChanged);

        private static void OnAreaSizeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            OutsideAreaView view = (OutsideAreaView)bindable;

            // we do not need to resize AreaContent
            double v = (double)newValue;

            // value cannot go below 0
            double areaSize = v < 0 ? 0 : v;
            view.AreaSize = areaSize;

            view.WidthBeingSet = true;
            view.WidthRequest = view.AreaContainer.WidthRequest + areaSize;

            view.HeightBeingSet = true;
            view.HeightRequest = view.AreaContainer.HeightRequest + areaSize;
        }

        public double AreaSize
        {
            get => (double)GetValue(AreaSizeProperty);
            set => SetValue(AreaSizeProperty, value);
        }

        double AreaWidth;
        double AreaHeight;

        public static readonly BindableProperty AreaContentProperty = BindableProperty.Create("AreaContent", typeof(View), typeof(OutsideAreaView), default(View), propertyChanged: onAreaContentChanged);

        private static void onAreaContentChanged(BindableObject bindable, object oldValue, object newValue)
        {
            OutsideAreaView view = (OutsideAreaView)bindable;
            view.AreaContainer.Content = (View)newValue;
        }

        public View AreaContent
        {
            get => (View)GetValue(AreaContentProperty);
            set => SetValue(AreaContentProperty, value);
        }

        private readonly ContentView AreaContainer;

        public OutsideAreaView()
        {
            AreaContainer = new()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            Content = AreaContainer;
        }

        bool HeightBeingSet = false;
        bool WidthBeingSet = false;

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName == ContentProperty.PropertyName)
            {
                if (Content != AreaContainer)
                {
                    throw new NotSupportedException("Content has changed, use AreaContent to set the content instead");
                    //Content = areaContainer;
                }
            }

            if (propertyName == WidthRequestProperty.PropertyName)
            {
                if (!WidthBeingSet)
                {
                    WidthBeingSet = true;
                    AreaWidth = WidthRequest;
                    AreaContainer.WidthRequest = AreaWidth;
                    WidthRequest += AreaSize;
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
                    HeightRequest += AreaSize;
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
