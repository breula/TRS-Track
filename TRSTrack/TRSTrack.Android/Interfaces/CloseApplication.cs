using Android.App;
using TRSTrack.Droid.Interfaces;
using TRSTrack.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(CloseApplication))]
namespace TRSTrack.Droid.Interfaces
{
    public class CloseApplication : ICloseApplication
    {
        public void CloseApp()
        {
            var activity = (Activity)Forms.Context;
            activity.FinishAffinity();
        }
    }
}