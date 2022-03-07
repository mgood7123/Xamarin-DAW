using System;
using System.Dynamic;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Xamarin_DAW.UI
{
    public partial class Window : ContentView
    {
        internal WindowManager WindowManager;
        Label label;

        public Window()
        {
            InitializeComponent();

            ClickGestureRecognizer c = new();
            c.Clicked += OnCloseButtonClicked;
            CloseButton.GestureRecognizers.Add(c);

            ClickGestureRecognizer c1 = new();
            c1.Clicked += OnMinimizeButtonClicked;
            MinimizeButton.GestureRecognizers.Add(c1);

            ClickGestureRecognizer c2 = new();
            c2.Clicked += OnMaximizeRestoreButtonClicked;
            MaximizeRestoreButton.GestureRecognizers.Add(c2);
            label = new Label();
            MainContent.Content = new ScrollView {
                Orientation = ScrollOrientation.Both,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Always,
                VerticalScrollBarVisibility = ScrollBarVisibility.Always,
                Content = label
            };
        }

        bool heightBeingSet = false;
        bool widthBeingSet = false;
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName == HeightRequestProperty.PropertyName)
            {
                if (!heightBeingSet)
                {
                    //heightBeingSet = true;
                    //HeightRequest += 20;
                    //Thickness padding = ResizeGestureContainer.Padding;
                    //ResizeGestureContainer.Padding = new Thickness(padding.HorizontalThickness, 10);
                    //ResizeContainer.Padding = -10;
                    //Thickness padding2 = ResizeContainer.Padding;
                    //ResizeContainer.Padding = new Thickness(padding2.HorizontalThickness, -10);
                }
                else
                {
                    //heightBeingSet = false;
                }
            }
            if (propertyName == WidthRequestProperty.PropertyName)
            {
                //if (!widthBeingSet)
                //{
                //    widthBeingSet = true;
                //    WidthRequest += 20;
                //    Thickness padding = ResizeContainer.Padding;
                //    ResizeContainer.Padding = new Thickness(5, padding.VerticalThickness);
                //}
                //else
                //{
                //    widthBeingSet = false;
                //}
            }
            base.OnPropertyChanged(propertyName);
        }

        void OnWindowPanned(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    break;
                case GestureStatus.Running:
                    double newX = e.TotalX;
                    // on android the Y value appears to be flipped
                    // moving down actually moves up
                    // vice versa on mac os
                    double newY = Plugin.Is_Android ? e.TotalY : -e.TotalY;

                    double nextX = X + newX;
                    double nextY = Y + newY;

                    double requiredX;
                    if (nextX < 0)
                    {
                        requiredX = -X;
                    }
                    else
                    {
                        if (nextX + Width > WindowManager.Width)
                        {
                            requiredX = (WindowManager.Width - Width) - X;
                        }
                        else
                        {
                            requiredX = newX;
                        }
                    }
                    double border_size = 10;
                    double requiredY;
                    if (nextY < border_size)
                    {
                        requiredY = (-Y) + border_size;
                    }
                    else
                    {
                        if ((nextY + Height) - border_size > WindowManager.Height)
                        {
                            requiredY = ((WindowManager.Height - Height) - Y) + border_size;
                        }
                        else
                        {
                            requiredY = newY;
                        }
                    }

                    WindowContainer.TranslationX = requiredX;
                    // on android the Y value appears to be flipped
                    // moving down actually moves up
                    // vice versa on mac os
                    WindowContainer.TranslationY = requiredY;
                    label.Text =
                        "Device Density: " + Plugin.ScreenDensity + "\n" +
                        "X: " + X + "\n" +
                        "Y: " + Y + "\n" +
                        "DeltaX: " + newX + "\n" +
                        "DeltaY: " + newY + "\n" +
                        "next X: " + (X + newX) + "\n" +
                        "next Y: " + (Y + newY) + "\n" +
                        "DeltaX Corrected: " + requiredX + "\n" +
                        "DeltaY Corrected: " + requiredY + "\n" +
                        "next X Corrected: " + (X + requiredX) + "\n" +
                        "next Y Corrected: " + (Y + requiredY) + "\n" +
                        ""
                        ;
                    //WindowContainer.TranslationX = newX;
                    //WindowContainer.TranslationY = newY;
                    break;
                case GestureStatus.Canceled:
                    WindowContainer.TranslationX = 0;
                    WindowContainer.TranslationY = 0;
                    break;
                case GestureStatus.Completed:
                    WindowManager.MoveChildTo(this, X + WindowContainer.TranslationX, Y + WindowContainer.TranslationY);
                    WindowContainer.TranslationX = 0;
                    WindowContainer.TranslationY = 0;
                    break;
            }
        }

        void OnWindowResized(object sender, PanUpdatedEventArgs e)
        {
        }

        void OnCloseButtonClicked(object sender, EventArgs e)
        {
        }

        void OnMinimizeButtonClicked(object sender, EventArgs e)
        {
        }

        void OnMaximizeRestoreButtonClicked(object sender, EventArgs e)
        {
        }
    }
}
