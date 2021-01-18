using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace TRSTrack.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupColorDialog : PopupPage
    {
        private static object _controller;

        public PopupColorDialog(object controller)
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