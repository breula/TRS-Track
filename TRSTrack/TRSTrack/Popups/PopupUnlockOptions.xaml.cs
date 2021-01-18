using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace TRSTrack.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupUnlockOptions : PopupPage
    {
        private static object _controller;

        public PopupUnlockOptions(object controller)
        {
            InitializeComponent();
            BindingContext = _controller = controller;
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