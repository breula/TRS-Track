using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using System;
using System.Threading.Tasks;
using TRSTrack.Models;
using Xamarin.Essentials;
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
            Device.StartTimer(TimeSpan.FromMilliseconds(500), () =>
            {
                MessagingCenter.Send(counter.ToString(), "counterValue");
                counter += 1;

                Task.Run(async () => {
                    var postion = await Geolocation.GetLocationAsync();
                    var last = ListeningPosition.Last();
                    if (last == null)
                    {
                        ListeningPosition.Add((int)Math.Ceiling(postion.Speed.Value * 3.6), postion.Latitude, postion.Longitude);
                    }
                    else
                    {
                        if (last.Latitude != postion.Latitude && last.Longitude != postion.Longitude)
                        {
                            ListeningPosition.Add((int)Math.Ceiling(postion.Speed.Value * 3.6), postion.Latitude, postion.Longitude);
                        }
                    }
                    return isRunningTimer;
                });

                return isRunningTimer;
            });

            return StartCommandResult.Sticky;
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
