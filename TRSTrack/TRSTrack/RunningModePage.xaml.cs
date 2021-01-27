using Plugin.Geolocator;
using Syncfusion.SfRadialMenu.XForms;
using TRSTrack.Controllers;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace TRSTrack
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RunningModePage : ContentPage
    {
        private static RunningModePageController _controller;

        public RunningModePage()
        {
            InitializeComponent();
            BindingContext = _controller = new RunningModePageController();
            _controller.CatchControl(this);
            _controller.CatchControl(Map);
            MessagingCenter.Unsubscribe<string>(this, "counterValue");
            MessagingCenter.Subscribe<string>(this, "counterValue", (value) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    _controller.UpdadeReceData();
                });
            });
        }

        protected async override void OnAppearing()
        {
            var position = await CrossGeolocator.Current.GetPositionAsync();
            Map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude), new Distance(_controller.CurrentMapZoom.Level)));
            _controller.UpdadeReceData();
            base.OnAppearing();
        }

        private void RadialMenu_OnDragEnd(object sender, DragEndEventArgs e)
        {
            _controller.UpdateRadialMenuPosition(e.NewValue);
        }
    }
}