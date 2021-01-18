using Foundation;
using Syncfusion.ListView.XForms.iOS;
using Syncfusion.SfBusyIndicator.XForms.iOS;
using Syncfusion.SfRadialMenu.XForms.iOS;
using Syncfusion.SfRangeSlider.XForms.iOS;
using Syncfusion.XForms.iOS.Backdrop;
using Syncfusion.XForms.iOS.Border;
using Syncfusion.XForms.iOS.Buttons;
using Syncfusion.XForms.iOS.Cards;
using Syncfusion.XForms.iOS.EffectsView;
using UIKit;

namespace TRSTrack.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            SfBorderRenderer.Init();
            SfButtonRenderer.Init();
            SfCardViewRenderer.Init();
            SfRadialMenuRenderer.Init();
            SfListViewRenderer.Init();
            SfEffectsViewRenderer.Init();
            SfBackdropPageRenderer.Init();
            SfEffectsViewRenderer.Init();
            new SfRangeSliderRenderer();
            new SfBusyIndicatorRenderer();
            Rg.Plugins.Popup.Popup.Init();
            Xamarin.FormsMaps.Init();
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
