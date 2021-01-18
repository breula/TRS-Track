using Syncfusion.SfRadialMenu.XForms;
using TRSTrack.Controllers;
using Xamarin.Forms;
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

        private void RadialMenu_OnDragEnd(object sender, DragEndEventArgs e)
        {
            _controller.UpdateRadialMenuPosition(e.NewValue);
        }
    }
}