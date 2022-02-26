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

        public override void DidFinishLaunching(NSNotification notification)
        {
            Forms.Init();
            LoadApplication(new Xamarin_DAW());
            base.DidFinishLaunching(notification);
        }

        public override void WillTerminate(NSNotification notification)
        {
            Application.Current.Quit();
        }
    }
}
