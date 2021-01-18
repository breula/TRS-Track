using TRSTrack.Controllers;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace TRSTrack.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupImportarCircuitoPage : PopupPage
    {
        private static PopupImportarCircuitoPageController _controller;

        public PopupImportarCircuitoPage()
        {
            InitializeComponent();
            BindingContext = _controller = new PopupImportarCircuitoPageController();
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