using System;
using System.Collections.Generic;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Topten.RichTextKit;
using Xamarin.Forms;

namespace Xamarin_DAW.UI
{
    class SKLabel : SKCanvasView
    {
        Dictionary<long, SKPoint> dragDictionary = new Dictionary<long, SKPoint>();

        protected override void OnTouch(SKTouchEventArgs e)
        {
            Console.WriteLine("override OnTouch");
            base.OnTouch(e);
        }

        private void OnTouch(object sender, SKTouchEventArgs e)
        {
            Console.WriteLine("OnTouch");
            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:
                    dragDictionary[e.Id] = e.Location;
                    Console.WriteLine("press: e.Id = " + e.Id);
                    Console.WriteLine("press: keys = " + dragDictionary.Keys.Count);
                    break;
                case SKTouchAction.Entered:
                    break;
                case SKTouchAction.Moved:
                    Console.WriteLine("move: keys = " + dragDictionary.Keys.Count);
                    if (dragDictionary.Keys.Count > 1)
                    {
                        dragDictionary[e.Id] = e.Location;
                        SKPoint? p1 = null;
                        SKPoint? p2 = null;
                        foreach (long key in dragDictionary.Keys)
                        {
                            if (p1 == null)
                            {
                                p1 = dragDictionary[key];
                            }
                            else if (p2 == null)
                            {
                                p2 = dragDictionary[key];
                            }
                        }
                        //MultiTouch handle
                    }
                    else
                    {
                        //SingleTouch handle
                    }
                    break;
                case SKTouchAction.Released:
                    dragDictionary.Remove(e.Id);
                    Console.WriteLine("release: e.Id = " + e.Id);
                    Console.WriteLine("release: keys = " + dragDictionary.Keys.Count);
                    break;
                case SKTouchAction.Exited:
                    break;
            }
            // we have handled these events
            e.Handled = true;
        }

        TextBlock textBlock;
        Topten.RichTextKit.Style style;

        public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(SKLabel), "SKLabel", propertyChanged: onTextChanged);

        private static void onTextChanged(BindableObject bindable, object oldValue, object newValue)
        {
            SKLabel sk = (SKLabel)bindable;
            sk.update();
        }

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create("FontSize", typeof(float), typeof(SKLabel), 12f, propertyChanged: onFontSizeChanged);

        private static void onFontSizeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            SKLabel sk = (SKLabel)bindable;
            sk.style.FontSize = sk.FontSize * Plugin.ScreenDensityAsFloat;
            sk.update();
        }

        public SKLabel()
        {
            textBlock = new TextBlock();
            style = new Topten.RichTextKit.Style();
            update();
            EnableTouchEvents = true;
            Touch += OnTouch;
        }

        void update()
        {
            textBlock.Clear();
            textBlock.AddText(Text, style);
            WidthRequest = measuredTextWidth;
            HeightRequest = measuredTextHeight;
            InvalidateSurface();
        }

        public float FontSize
        {
            get => (float)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public float measuredTextWidth => textBlock.MeasuredWidth;
        public float measuredTextHeight => textBlock.MeasuredHeight;

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);

            SkiaSharp.SKCanvas canvas = e.Surface.Canvas;

            canvas.Clear();
            textBlock.Paint(canvas);
        }
    }
}