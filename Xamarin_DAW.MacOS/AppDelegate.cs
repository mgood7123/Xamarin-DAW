using AppKit;
using Foundation;
using Xamarin.Forms;
using Xamarin.Forms.Platform.MacOS;

namespace Xamarin_DAW.MacOS
{
    [Register("AppDelegate")]
    public class AppDelegate : FormsApplicationDelegate
    {
        NSWindow _window;

        public override NSWindow MainWindow => _window;

        public AppDelegate()
        {
            var style = NSWindowStyle.Miniaturizable | NSWindowStyle.Resizable | NSWindowStyle.Titled;

            var rect = new CoreGraphics.CGRect(200, 200, 800, 600);
            _window = new NSWindow(rect, style, NSBackingStore.Buffered, false);
            _window.Title = "Xamarin DAW";
        }

        Xamarin_DAW daw;

        public override void DidFinishLaunching(NSNotification notification)
        {
            Forms.Init();
            daw = new Xamarin_DAW();

            // workaround for https://github.com/xamarin/Essentials/issues/1679
            daw.setDensity(NSScreen.MainScreen.UserSpaceScaleFactor);

            daw.hasStoragePermission(true);
            LoadApplication(daw);
            base.DidFinishLaunching(notification);
        }



        //private void DeviceDisplay_MainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
        //{
        //    daw.setDensity(e.DisplayInfo.Density);
        //}

        public override void WillTerminate(NSNotification notification)
        {
            Application.Current.Quit();
        }
    }
}
