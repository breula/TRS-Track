using Rg.Plugins.Popup.Pages;
using TRSTrack.Controllers;
using TRSTrack.Models;
using Xamarin.Forms.Xaml;

namespace TRSTrack.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupRaceListPage : PopupPage
    {
        private static PopupRaceListPageController _controller;

        public PopupRaceListPage(Race currentRace)
        {
            InitializeComponent();
            BindingContext = _controller = new PopupRaceListPageController(currentRace);
            _controller.CatchControl(Map);
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        protected override bool OnBackgroundClicked()
        {
            return false;
        }
    }
}