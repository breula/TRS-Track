using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using TRSTrack.Models.Enums;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TRSTrack.Controllers
{
    public class GrantPositionPageController : BaseController
    {
        #region-- Properties --
        private ImageSource _positionGif;
        public ImageSource PositionGif
        {
            get => _positionGif;
            set
            {
                _positionGif = value;
                OnPropertyChanged(nameof(PositionGif));
            }
        }

        private GrantPositionPage _currentPage;
        public GrantPositionPage CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }
        #endregion

        #region-- Commands --
        public Command AllowPositionCommand
        {
            get { return new Command(async obj => { await AllowPosition(); }); }
        }
        #endregion

        public GrantPositionPageController()
        {
            PositionGif = GetImageSource(MyImageEnum.PositionGif);
        }

        public void CatchControl(object control)
        {
            if (control.GetType() == typeof(GrantPositionPage))
            {
                CurrentPage = (GrantPositionPage)control;
            }
        }

        private async Task AllowPosition()
        {
            if (!LocalizationSettings.IsGpsAvailable())
            {
                await MessageService.ShowAsync("GPS desabilitado", $"Por favor, ligue o GPS de seu dispositivo antes de usar o {AppName}.");
                LocalizationSettings.OpenSettings();
                return;
            }

            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status == PermissionStatus.Denied || status == PermissionStatus.Unknown)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    var retry = await MessageService.ShowDialogAsync("Ué?", $"Se não permitir que {AppName} acesse sua localização não será possivel utilizar! Tentar novamente?");
                    if (!retry)
                    {
                        await MessageService.ShowAsync("Então tá né!", $"{AppName} será encerrado!");
                        CloseApplication.CloseApp();
                    }
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    if (status != PermissionStatus.Granted)
                    {
                        await MessageService.ShowAsync("Mas ôôô loko!", $"{AppName} será encerrado então. Tente novamente mais tarde!");
                        CloseApplication.CloseApp();
                    }
                }
            }

            var position = await CrossGeolocator.Current.GetPositionAsync();
            var local = await CrossGeolocator.Current.GetAddressesForPositionAsync(new Position(position.Latitude, position.Longitude));
            var address = local.ToList();
            if (address.Count == 0)
            {
                await MessageService.ShowAsync("Sucesso!", $"Acesso à sua posição foi concedida com sucesso.");
            }
            else
            {
                await MessageService.ShowAsync("Sucesso!", $"Acesso à sua posição foi concedida com sucesso. Você está em {address[0].SubAdminArea}.");
            }
            //ResetNavigationStack(CurrentPage);
            Application.Current.MainPage = new NavigationPage(new MainPage());
        }
    }
}
