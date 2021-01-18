using System.Threading.Tasks;

namespace TRSTrack.Interfaces
{
    public interface IMessageService
    {
        void Show(string messageTitle, string messageContent, string buttonLabel = "Ok");
        Task ShowAsync(string messageTitle, string messageContent, string buttonLabel = "Ok");
        Task<bool> ShowDialogAsync(string titulo, string mensagem);
        Task<bool> ShowDialogAsync(string titulo, string mensagem, string aceptButtonCaption, string cancelButtonCaption);
    }
}
