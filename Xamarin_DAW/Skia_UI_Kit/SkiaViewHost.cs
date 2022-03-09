using System;
using System.Runtime.CompilerServices;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

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
                bool canvas_exists = canvas is not null;
                bool canvas_needs_creation = !canvas_exists
                    || canvas.getWidth() != e.BackendRenderTarget.Width
                    || canvas.getHeight() != e.BackendRenderTarget.Height;

                if (canvas_needs_creation)
                {
                    if (canvas_exists)
                    {
                        canvas.Dispose();
                        canvas.DisposeSurface();
                        canvas = null;
                    }
                    canvas = SKCanvasExtensions.CreateHardwareAcceleratedCanvas(null, GRContext, e.BackendRenderTarget.Width, e.BackendRenderTarget.Height);
                    Application.onSizeChanged(e.BackendRenderTarget.Width, e.BackendRenderTarget.Height);
                }

                Application.Draw(canvas);

                canvas.Flush();
                canvas.DrawToCanvas(e.Surface.Canvas, 0, 0);
            }
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (IsVisibleProperty.PropertyName == propertyName)
            {
                Application.handleAppVisibility(IsVisible);
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
