using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Plugin.Permissions;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Xamarin_DAW.Android_
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
        Xamarin_DAW daw;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            Forms.Init(this, savedInstanceState);
            DeviceDisplay.MainDisplayInfoChanged += DeviceDisplay_MainDisplayInfoChanged;
            Console.WriteLine("loading DAW");
            daw = new Xamarin_DAW();
            daw.setDensity(DeviceDisplay.MainDisplayInfo.Density);
            LoadApplication(daw);
            Console.WriteLine("loaded DAW");
        }

        private void DeviceDisplay_MainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
        {
            daw.setDensity(e.DisplayInfo.Density);
        }

        protected override async void OnResume()
        {
            base.OnResume();
            Console.WriteLine("Requesting storage");
            await requestStorageAsync();
            Console.WriteLine("[Requesting storage] has completed");
        }

        async Task requestStorageAsync()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync<StoragePermission>().ConfigureAwait(false);
                if (status != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                {
                    daw.hasStoragePermission(false);
                    status = await CrossPermissions.Current.RequestPermissionAsync<StoragePermission>();
                }

                if (status == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                {
                    daw.hasStoragePermission(true);
                }
                else if (status != Plugin.Permissions.Abstractions.PermissionStatus.Unknown)
                {
                    daw.hasStoragePermission(false);
                }
            }
            catch (System.Exception)
            {
                daw.hasStoragePermission(false);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnStart()
        {
            base.OnStart();
            AppCenter.Start("android=7ead583e-ba27-490e-be69-1dc4ae5fe90d;",
                              typeof(Analytics), typeof(Crashes));
        }
    }
}
