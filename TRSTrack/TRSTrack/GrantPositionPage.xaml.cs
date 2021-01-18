using TRSTrack.Controllers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TRSTrack
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GrantPositionPage : ContentPage
    {
        private static GrantPositionPageController _controller;

        public GrantPositionPage()
        {
            InitializeComponent();
            BindingContext = _controller = new GrantPositionPageController();
            _controller.CatchControl(this);
        }
    }
}