using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Plugin.Geolocator;
using System;
using System.Threading.Tasks;
using TRSTrack.Models;
using Xamarin.Forms;


namespace TRSTrack.Services
{
    [Service(Label = "BackgroundService")]
    public class BackgroundService : Service
    {
        int counter = 0;
        bool isRunningTimer = true;

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                MessagingCenter.Send(counter.ToString(), "counterValue");
                counter += 1;

                Task.Run(async () =>
                {
                    var postion = await CrossGeolocator.Current.GetPositionAsync(TimeSpan.FromMilliseconds(100));
                    var last = ListeningPosition.Last();
                    if (last == null)
                    {
                        ListeningPosition.Add((int)Math.Ceiling(postion.Speed * 3.6), postion.Latitude, postion.Longitude);
                    }
                    else
                    {
                        if (Math.Abs(last.Latitude - postion.Latitude) > 0 && Math.Abs(last.Latitude - postion.Longitude) > 0)
                        {
                            ListeningPosition.Add((int)Math.Ceiling(postion.Speed * 3.6), postion.Latitude, postion.Longitude);
                        }
                    }
                });

                return isRunningTimer;
            });

            return StartCommandResult.Sticky;
        }

        private void CurrentPositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var position = e.Position;
                var last = ListeningPosition.Last();
                if (last == null)
                {
                    ListeningPosition.Add((int)Math.Ceiling(e.Position.Speed * 3.6), e.Position.Latitude, e.Position.Longitude);
                    return;
                }
                if (Math.Abs(last.Latitude - e.Position.Latitude) <= 0 && Math.Abs(last.Longitude - e.Position.Longitude) <= 0) return;
                ListeningPosition.Add((int)Math.Ceiling(e.Position.Speed * 3.6), e.Position.Latitude, e.Position.Longitude);
            });
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnDestroy()
        {
            StopSelf();
            counter = 0;
            isRunningTimer = false;
            base.OnDestroy();
        }
    }
}
