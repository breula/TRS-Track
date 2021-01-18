using App.Services;
using System.Threading.Tasks;
using TRSTrack.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(MessageService))]
namespace App.Services
{
    public class MessageService : IMessageService
    {
        public void Show(string messageTitle, string messageContent, string buttonLabel = "Ok")
        {
            Application.Current.MainPage.DisplayAlert(messageTitle, messageContent, buttonLabel);
        }

        public async Task ShowAsync(string messageTitle, string messageContent, string buttonLabel = "Ok")
        {
            await Application.Current.MainPage.DisplayAlert(messageTitle, messageContent, buttonLabel);
        }

        public async Task<bool> ShowDialogAsync(string titulo, string mensagem)
        {
            return await Application.Current.MainPage.DisplayAlert(titulo, mensagem, "Sim", "Não");
        }

        public async Task<bool> ShowDialogAsync(string titulo, string mensagem, string aceptButtonCaption, string cancelButtonCaption)
        {
            return await Application.Current.MainPage.DisplayAlert(titulo, mensagem, aceptButtonCaption, cancelButtonCaption);
        }
    }
}
