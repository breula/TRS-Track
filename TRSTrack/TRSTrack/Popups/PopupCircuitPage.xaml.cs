using TRSTrack.Controllers;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace TRSTrack.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupCircuitPage : PopupPage
    {
        private static PopupCircuitPageController _controller;

        public PopupCircuitPage()
        {
            InitializeComponent();
            BindingContext = _controller = new PopupCircuitPageController();
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