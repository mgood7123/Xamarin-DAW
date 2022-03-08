using AppKit;

namespace Xamarin_DAW.MacOS
{
    static class MainClass
    {
        static void Main(string[] args)
        {
            NSApplication.Init();
            NSApplication.SharedApplication.Delegate = new AppDelegate();
            NSApplication.SharedApplication.ActivateIgnoringOtherApps(true);
            NSApplication.Main(args);
        }
    }
}
