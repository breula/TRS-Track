using System.Threading.Tasks;

namespace TRSTrack.Interfaces
{
    public interface INavigationService
    {
        Task NavigateToMainPagePushPopupAsync();
        Task NavigateToRunningModePageOushPopupAsync();
    }
}
