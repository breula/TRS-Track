using Foundation;
using TRSTrack.Interfaces;
using TRSTrack.iOS.Interfaces;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppSettingsInterface))]
namespace TRSTrack.iOS.Interfaces
{
    public class AppSettingsInterface : IAppSettings
    {
        public void OpenAppSettings()
        {
            var url = new NSUrl($"app-settings:");
            UIApplication.SharedApplication.OpenUrl(url);
        }
    }
}