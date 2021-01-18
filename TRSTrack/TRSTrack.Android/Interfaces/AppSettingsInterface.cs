using Android.Content;
using TRSTrack.Droid.Interfaces;
using TRSTrack.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppSettingsInterface))]
namespace TRSTrack.Droid.Interfaces
{
    public class AppSettingsInterface : IAppSettings
    {
        public void OpenAppSettings()
        {
            var intent = new Intent(Android.Provider.Settings.ActionApplicationDetailsSettings);
            intent.AddFlags(ActivityFlags.NewTask);
            var uri = Android.Net.Uri.FromParts("package", "com.vitsoftol.trstrack", null);
            intent.SetData(uri);
            Android.App.Application.Context.StartActivity(intent);
        }
    }
}