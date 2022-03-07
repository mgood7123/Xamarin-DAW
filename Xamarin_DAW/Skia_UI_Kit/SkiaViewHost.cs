using System;
using SkiaSharp;
using SkiaSharp.Extended;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Xaml.Internals;

namespace Xamarin_DAW.Skia_UI_Kit
{
    [ContentProperty("Content")]
    public class SkiaViewHost : SKGLView
    {
        public static readonly BindableProperty ContentProperty = BindableProperty.Create("Content", typeof(View), typeof(SkiaViewHost), default(View), propertyChanged: OnContentChanged);

        private static void OnContentChanged(BindableObject bindable, object oldValue, object newValue) => ((SkiaViewHost)bindable).Content = (View)newValue;

        public View Content
        {
            get => (View)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public static readonly BindableProperty ApplicationProperty = BindableProperty.Create("Application", typeof(Application), typeof(SkiaViewHost), default(Application), propertyChanged: OnApplicationChanged);

        private static void OnApplicationChanged(BindableObject bindable, object oldValue, object newValue)
        {
            SkiaViewHost bindable1 = (SkiaViewHost)bindable;
            bindable1.Application?.OnPause();
            bindable1.Application?.SetHost(null);
            bindable1.Application = (Application)newValue;
            bindable1.Application?.SetHost(bindable1);
        }

        public void INTERNAL_ERROR(string error)
        {
            // TODO: display error to screen
            throw new ApplicationException(error);
        }

        public Application Application
        {
            get => (Application)GetValue(ApplicationProperty);
            set => SetValue(ApplicationProperty, value);
        }

        public UI.Utils.MultiTouch multiTouch;

        SKCanvas canvas;

        public SkiaViewHost()
        {
            multiTouch = new UI.Utils.MultiTouch();
            multiTouch.setMaxSupportedTouches(10);
            multiTouch.throw_on_error = false;
            EnableTouchEvents = true;
        }

        protected override void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
        {
            Console.WriteLine("OnPaintSurface");

            base.OnPaintSurface(e);

            e.Surface.Canvas.Clear(SKColors.Black);

            if (Application is not null)
            {
                if (canvas is null)
                {
                    canvas = SKCanvasExtensions.CreateHardwareAcceleratedCanvas(null, GRContext, e.BackendRenderTarget.Width, e.BackendRenderTarget.Height);
                }

                Application.Draw(canvas);

                canvas.Flush();
                canvas.DrawToCanvas(e.Surface.Canvas, 0, 0);
            }
        }

        public void OnTouch()
        {
            //if (Application?.Content != null)
            //{
            //    Application.Content.OnTouch(multiTouch);
            //}
        }
    }
}
