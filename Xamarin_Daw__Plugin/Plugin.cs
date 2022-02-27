using Xamarin.Forms;

namespace Xamarin_DAW
{
    public abstract class Plugin
    {
        // OS DETECTION FUNCTIONS
        public static bool is_Android => Device.RuntimePlatform == Device.Android;
        public static bool is_MacOS => Device.RuntimePlatform == Device.macOS;
        public static bool is_UWP_Windows_10 => Device.RuntimePlatform == Device.UWP;
        public static bool is_WPF_Windows_XP_Or_Later => Device.RuntimePlatform == Device.WPF;
        public static bool is_GTK => Device.RuntimePlatform == Device.GTK;

        // INTERFACE
        public abstract View onCreateView();
    }
}
