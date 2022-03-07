using Xamarin.Forms;

namespace Xamarin_DAW
{
    public abstract class Plugin
    {
        // OS DETECTION FUNCTIONS
        public static bool Is_Android => Device.RuntimePlatform == Device.Android;
        public static bool Is_MacOS => Device.RuntimePlatform == Device.macOS;
        public static bool Is_UWP_Windows_10 => Device.RuntimePlatform == Device.UWP;
        public static bool Is_WPF_Windows_XP_Or_Later => Device.RuntimePlatform == Device.WPF;
        public static bool Is_GTK => Device.RuntimePlatform == Device.GTK;

        // SCREEN ORIENTATION
        public static bool Is_Portrait => Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Orientation == Xamarin.Essentials.DisplayOrientation.Portrait;
        public static bool Is_Landscape => Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Orientation == Xamarin.Essentials.DisplayOrientation.Landscape;

        // SCREEN DENSITY
        static double screenDensity;

        public static double ScreenDensity => screenDensity;
        public static float ScreenDensityAsFloat => (float)screenDensity;


        // FOR INTERNAL USE
        public static double XAMARIN_DAW_INTERNAL_USE_ONLY_SCREEN_DENSITY_SETTER
        {
            set => screenDensity = value;
        }


        // INTERFACE

        public abstract View onCreateView();
    }
}
