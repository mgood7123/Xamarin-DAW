using Android.App;
using Android.Content.PM;
using Android.OS;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace Xamarin_DAW.Android
{
    [Activity(
        Label = "@string/app_name",
        Theme = "@style/AppTheme.NoActionBar",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation
        )]
    // FormsAppCompatActivity uses fast renderers
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new Xamarin_DAW());
        }

        protected override void OnStart()
        {
            base.OnStart();
            AppCenter.Start("android=7ead583e-ba27-490e-be69-1dc4ae5fe90d;",
                              typeof(Analytics), typeof(Crashes));
        }
    }
}
