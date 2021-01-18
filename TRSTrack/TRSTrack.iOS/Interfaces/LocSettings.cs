using CoreLocation;
using Foundation;
using TRSTrack.Interfaces;
using TRSTrack.iOS.Interfaces;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(LocSettings))]
namespace TRSTrack.iOS.Interfaces
{
    public class LocSettings : ILocSettings
    {
        public bool IsGpsAvailable()
        {
            return CLLocationManager.LocationServicesEnabled;
        }

        public void OpenSettings()
        {
            UIApplication.SharedApplication.OpenUrl(new NSUrl(UIApplication.OpenSettingsUrlString));
        }
    }
}