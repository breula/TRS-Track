using System.Threading;
using TRSTrack.Interfaces;
using TRSTrack.iOS.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(CloseApplication))]
namespace TRSTrack.iOS.Interfaces
{
    public class CloseApplication : ICloseApplication
    {
        public void CloseApp()
        {
            Thread.CurrentThread.Abort();
        }
    }
}