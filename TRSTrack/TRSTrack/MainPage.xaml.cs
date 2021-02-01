using Plugin.Geolocator;
using Syncfusion.SfRadialMenu.XForms;
using Syncfusion.XForms.Backdrop;
using TRSTrack.Controllers;
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
            //MessagingCenter.Unsubscribe<string>(this, "counterValue");
            //MessagingCenter.Subscribe<string>(this, "counterValue", (value) =>
            //{
            //    Device.BeginInvokeOnMainThread(() =>
            //    {
            //        _controller.UpdadeTrackData(Map);
            //    });
            //});
        }

        protected async override void OnAppearing()
        {
            var position = await CrossGeolocator.Current.GetPositionAsync();

            if (_controller.Percurso != null)
            {
                var polyline = new Polyline
                {
                    StrokeColor = Color.DodgerBlue,
                    StrokeWidth = 18
                };

                polyline.Geopath.Clear();
                for (var i = 0; i < _controller.Percurso.Count; i++)
                {
                    polyline.Geopath.Insert(i, new Position(_controller.Percurso[i].Latitude, _controller.Percurso[i].Longitude));
                }
                Map.MapElements.Add(polyline);
            }
            Map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude), new Distance(_controller.CurrentMapZoom.Level)));
            base.OnAppearing();
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
