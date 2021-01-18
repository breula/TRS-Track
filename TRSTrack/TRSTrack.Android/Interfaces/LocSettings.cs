using Android.Content;
using TRSTrack.Droid.Interfaces;
using TRSTrack.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(LocSettings))]
namespace TRSTrack.Droid.Interfaces
{
    public class LocSettings : ILocSettings
    {
        public bool IsGpsAvailable()
        {
            Android.Locations.LocationManager manager = (Android.Locations.LocationManager)Android.App.Application.Context.GetSystemService(Context.LocationService);
            bool value;
            if (!manager.IsProviderEnabled(Android.Locations.LocationManager.GpsProvider))
            {
                //gps disable
                value = false;
            }
            else
            {
                //Gps enable
                value = true;
            }
            return value;
        }
        public void OpenSettings()
        {
            Intent intent = new Intent(Android.Provider.Settings.ActionLocat‌​ionSourceSettings);
            intent.AddFlags(ActivityFlags.NewTask);
            Android.App.Application.Context.StartActivity(intent);
        }
    }
}