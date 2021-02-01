using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Xamarin.Forms;
using Android.Content;
using TRSTrack.Services;
using Plugin.CurrentActivity;
using Acr.UserDialogs;
using Android.Views;
using Plugin.Permissions;

namespace TRSTrack.Droid
{
    [Activity(Label = "TRS Track", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            Rg.Plugins.Popup.Popup.Init(this);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Xamarin.FormsMaps.Init(this, savedInstanceState);
            Forms.Init(this, savedInstanceState);
            TransparentStatusBar();
            UserDialogs.Init(this);
            LoadApplication(new App());

            MessagingCenter.Unsubscribe<string>(this, "TRSTrackService");
            MessagingCenter.Subscribe<string>(this, "TRSTrackService", (value) =>
            {
                if (value == "1")
                {
                    base.StartService(new Intent(this, typeof(BackgroundService)));
                }
                else
                {
                    base.StopService(new Intent(this, typeof(BackgroundService)));
                }
            });
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            //PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void TransparentStatusBar()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.Kitkat) return;
            Window.SetFlags(WindowManagerFlags.LayoutNoLimits, WindowManagerFlags.LayoutNoLimits);
            Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
            Window.SetStatusBarColor(Android.Graphics.Color.Transparent);
        }

        public override void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                // Do something if there are some pages in the `PopupStack`
            }
            else
            {
                // Do something if there are not any pages in the `PopupStack`
            }
        }
    }
}