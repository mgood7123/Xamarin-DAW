using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Topten.RichTextKit;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin_DAW.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    // container window for scrollview and SKCanvas
    //
    // since we cannot recieve multi-touch without
    // diving deep into platform-specific render code
    //
    // we opt for simplish approach:
    //
    // spotlight the canvas without changing
    // the size of the canvas itself
    //
    // we must not resize the canvas since it can be very costly
    // when rendering very large content such as 80,000 by 80,000
    //
    // we keep the canvas the same size as our FileView
    // while the Scroll View's container tries to resize
    // to fit the all of the canvas's drawn content
    //
    // this avoids creating a very large bitmap for the canvas
    // even though the canvas's viewport is much smaller
    //
    // equivilant to displaying an unscaled 4k image in a 50 by 50 image view
    // attempting to create a 4k canvas can fail on platforms such as android
    //
    // instead create 50x50 canvas and simply draw 4k image to it
    // the canvas can be translated before drawing in order to "move"
    // the image around as if it where drawn in an actual 4k canvas
    //
    //
    // this works because an empty content view is rendered for free
    // regardless of its size
    // thus we need only worry about the content itself
    //
    // structure is as follows:
    //
    // fileview - container to skia canvas
    //  scroll area - handles all scrolling, must be same size as fileview
    //   container - resizable to infinity, AT LEAST same size as fileview
    //    skia canvas - draws the actual fileview, must be same size as fileview
    //
    //
    //
    // BUGS
    //
    // there is a bug where decreasing the font can cause visual glitches
    // https://imgur.com/9hMcrPH
    //
    public partial class FileView : ContentView
    {
        ContentFileView contentFileView;

        public static readonly BindableProperty DirectoryProperty = BindableProperty.Create("Directory", typeof(string), typeof(FileView), "/", propertyChanged: OnDirectoryChanged);

        public string Directory
        {
            get => (string)GetValue(DirectoryProperty);
            set => SetValue(DirectoryProperty, value);
        }

        private static void OnDirectoryChanged(BindableObject bindable, object oldValue, object newValue)
        {
            FileView fileView = (FileView)bindable;
            fileView.contentFileView.Directory = (string)newValue;
            fileView.contentFileView.RefreshDirectoryList();
            fileView.contentFileView.update();
        }

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create("FontSize", typeof(float), typeof(FileView), 12f, propertyChanged: onFontSizeChanged);

        public float FontSize
        {
            get => (float)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        private static void onFontSizeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            FileView fileView = (FileView)bindable;
            fileView.contentFileView.FontSize = (float)newValue;
            fileView.contentFileView.update();
        }

        class Scroll : ScrollView
        {
            SKCanvasView view;

            public Scroll(SKCanvasView view)
            {
                this.view = view;
            }

            protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                if (propertyName == ScrollXProperty.PropertyName)
                {
                    Console.WriteLine("scroll X changed: " + ScrollX);
                    view.TranslationX = ScrollX;
                    view.InvalidateSurface();
                }
                if (propertyName == ScrollYProperty.PropertyName)
                {
                    Console.WriteLine("scroll Y changed: " + ScrollY);
                    view.TranslationY = ScrollY;
                    view.InvalidateSurface();
                }
                base.OnPropertyChanged(propertyName);
            }
        }

        Scroll scrollView;

        public FileView()
        {
            CopySize resizableContainer = new(this);
            contentFileView = new(resizableContainer, Directory, FontSize);
            scrollView = new(contentFileView);

            scrollView.VerticalOptions = LayoutOptions.FillAndExpand;
            scrollView.HorizontalOptions = LayoutOptions.FillAndExpand;

            scrollView.Orientation = ScrollOrientation.Both;

            scrollView.VerticalScrollBarVisibility = ScrollBarVisibility.Always;

            scrollView.Content = resizableContainer;

            resizableContainer.Content = contentFileView;

            Content = scrollView;
        }
    }

    class ContentFileView : SKCanvasView
    {
        public string Directory;
        float _;
        public float FontSize
        {
            get => _;
            set
            {
                _ = value;
                style.FontSize = _ * Plugin.ScreenDensityAsFloat;
            }
        }

        List<string> dirs;

        TextBlock textBlock;
        Topten.RichTextKit.Style style;

        ContentView resizableContainer;

        public ContentFileView(ContentView resizableContainer, string directory, float fontSize)
        {
            textBlock = new TextBlock();
            style = new Topten.RichTextKit.Style();

            Directory = directory;
            FontSize = fontSize;

            this.resizableContainer = resizableContainer;

            PaintSurface += FileView_PaintSurface;

            RefreshDirectoryList();

            update();
        }

        private void FileView_PaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();
            canvas.Save();
            canvas.Translate(-(float)TranslationX, -(float)TranslationY);
            textBlock.Paint(canvas);
            canvas.Restore();
        }

        void buildRichString()
        {
            textBlock.Clear();
            foreach (string entry in dirs)
            {
                textBlock.AddText(entry + "\n", style);
            }

            resizableContainer.WidthRequest = textBlock.MeasuredWidth;
            resizableContainer.HeightRequest = textBlock.MeasuredHeight;
        }

        public void update()
        {
            buildRichString();
            InvalidateSurface();
        }

        List<string> GetSortedList(IEnumerable<string> e, string tag = null)
        {
            List<string> list = new();
            foreach (string path in e) list.Add((tag == null ? "" : tag + " ") + System.IO.Path.GetFileName(path));
            list.Sort();
            return list;
        }

        public void RefreshDirectoryList()
        {
            string dir = Directory;

            dirs = GetSortedList(System.IO.Directory.EnumerateDirectories(dir), "[D]");
            var files = GetSortedList(System.IO.Directory.EnumerateFiles(dir), "[F]");

            dirs.AddRange(files);
        }
    }
}