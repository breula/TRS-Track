using TRSTrack.Controllers;
using TRSTrack.Models;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace TRSTrack.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupEditarCircuito : PopupPage
    {
        private static PopupEditarPageController _controller;

        public PopupEditarCircuito(Circuito circuito)
        {
            InitializeComponent();
            BindingContext = _controller = new PopupEditarPageController(circuito);
        }
    }
}