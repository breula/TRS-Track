using TRSTrack.Controllers;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace TRSTrack.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupAdjustRecordPage : PopupPage
    {
        private static PopupAdjustRecordPageController _controller;

        public PopupAdjustRecordPage()
        {
            InitializeComponent();
            BindingContext = _controller = new PopupAdjustRecordPageController();
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