using Plugin.Geolocator;
using Syncfusion.SfRadialMenu.XForms;
using Syncfusion.XForms.Backdrop;
using System;
using TRSTrack.Controllers;
using TRSTrack.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace TRSTrack
{
    public partial class MainPage : SfBackdropPage
    {
        private static MainPageController _controller;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = _controller = new MainPageController();
            _controller.CatchControl(this);
            _controller.CatchControl(Map);
            MessagingCenter.Unsubscribe<string>(this, "counterValue");
            MessagingCenter.Subscribe<string>(this, "counterValue", (value) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    _controller.UpdadeTrackData(Map);
                });
            });
        }

        protected async override void OnAppearing()
        {
            var position = await CrossGeolocator.Current.GetPositionAsync();
            Map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude), new Distance(_controller.CurrentMapZoom.Level)));
            base.OnAppearing();
        }

        private void StartService(object sender, EventArgs e)
        {
            MessagingCenter.Send("1", "TRSTrackService");
        }

        private void StopService(object sender, EventArgs e)
        {
            MessagingCenter.Send("0", "TRSTrackService");
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private void RadialMenu_OnDragEnd(object sender, DragEndEventArgs e)
        {
            _controller.UpdateRadialMenuPosition(e.NewValue);
        }
    }
}
