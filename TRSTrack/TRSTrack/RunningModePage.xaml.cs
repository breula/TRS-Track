using Plugin.Geolocator;
using Syncfusion.SfRadialMenu.XForms;
using TRSTrack.Controllers;
using TRSTrack.Models;
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
        }

        protected async override void OnAppearing()
        {
            var position = await CrossGeolocator.Current.GetPositionAsync();
            foreach (var newElement in ListeningPosition.MapListening().MapElements)
            {
                var jaExiste = false;
                foreach (var element in Map.MapElements)
                {
                    if (newElement == element)
                    {
                        jaExiste = true;
                        break;
                    }
                }
                if (!jaExiste)
                    Map.MapElements.Add(newElement);
            }
            Map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude), new Distance(_controller.CurrentMapZoom.Level)));
            base.OnAppearing();
        }

        private void RadialMenu_OnDragEnd(object sender, DragEndEventArgs e)
        {
            _controller.UpdateRadialMenuPosition(e.NewValue);
        }

        private void BtnSalvarRange_Clicked(object sender, System.EventArgs e)
        {
            _controller.SalvarRange((int)RangeDistance.Value);
        }
    }
}