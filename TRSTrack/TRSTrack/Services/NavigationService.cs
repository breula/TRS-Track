using System.Threading.Tasks;
using App.Services;
using TRSTrack;
using TRSTrack.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(NavigationService))]
namespace App.Services
{
    public class NavigationService : INavigationService
    {
        public async Task NavigateToMainPagePushPopupAsync() => await Application.Current.MainPage.Navigation.PushModalAsync(new MainPage());
        public async Task NavigateToRunningModePageOushPopupAsync() => await Application.Current.MainPage.Navigation.PushModalAsync(new RunningModePage());
    }
}
