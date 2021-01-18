using Plugin.Geolocator;
using Syncfusion.Licensing;
using Xamarin.Forms;

[assembly: ExportFont("FasterOne.ttf", Alias = "TitleFont")]
[assembly: ExportFont("MaterialIcons-Regular.ttf", Alias = "IconFont")]
[assembly: ExportFont("Font Awesome 5 Free-Regular-400.otf", Alias = "AwesomeRegular")]
[assembly: ExportFont("Font Awesome 5 Free-Solid-900.otf", Alias = "AwesomeBold")]
[assembly: ExportFont("Ubuntu-Bold.ttf", Alias = "BoldFont")]
[assembly: ExportFont("Ubuntu-Light.ttf", Alias = "LightFont")]
[assembly: ExportFont("Ubuntu-Regular.ttf", Alias = "RegularFont")]

namespace TRSTrack
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            SyncfusionLicenseProvider.RegisterLicense("Mzc1MjEzQDMxMzgyZTM0MmUzMEE5alBqNTlZdEpMcUJJZHo5NjJoOS9FZXArc1d1NDRWVUU1Z0NYVFNrTm89");

            if (CrossGeolocator.Current.IsGeolocationEnabled)
            {
                MainPage = new NavigationPage(new MainPage());
            }
            else
            {
                MainPage = new GrantPositionPage();
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
