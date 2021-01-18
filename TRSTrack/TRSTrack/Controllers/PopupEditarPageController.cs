using TRSTrack.Models;
using TRSTrack.Models.Enums;
using TRSTrack.Popups;
using TRSTrack.Services;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TRSTrack.Controllers
{
    public class PopupEditarPageController : BaseController
    {
        #region-- Properties --
        private Circuito _circuito;
        public Circuito Circuito
        {
            get => _circuito;
            set
            {
                _circuito = value;
                OnPropertyChanged(nameof(Circuito));
            }
        }

        private ObservableCollection<WayPoint> _wayPoints;
        public ObservableCollection<WayPoint> WayPoints
        {
            get => _wayPoints;
            set
            {
                _wayPoints = value;
                OnPropertyChanged(nameof(WayPoints));
            }
        }
        private WayPoint _wayPointForEdit;
        public WayPoint WayPointForEdit
        {
            get => _wayPointForEdit;
            set
            {
                _wayPointForEdit = value;
                OnPropertyChanged(nameof(WayPointForEdit));
            }
        }

        private ImageSource _imageColorDialog;
        public ImageSource ImageColorDialog
        {
            get => _imageColorDialog;
            set
            {
                _imageColorDialog = value;
                OnPropertyChanged(nameof(ImageColorDialog));
            }
        }
        #endregion

        #region-- Commands --
        public Command SalvarCircuitoCommand
        {
            get { return new Command(async obj => { await SalvarCircuito(); }); }
        }

        public Command ClosePopupCommand
        {
            get { return new Command(async obj => { await ClosePopup(); }); }
        }

        public Command OpenColorPickerDialogCommand
        {
            get { return new Command<WayPoint>(async obj => { await OpenColorPickerDialog(obj); }); }
        }

        public Command CloseColorDialogCommand
        {
            get { return new Command(async obj => { await CloseColorDialog(); }); }
        }

        public Command ExcluirWayPointCommand
        {
            get { return new Command<WayPoint>(async obj => { await ExcluirWayPoint(obj); }); }
        }

        public Command SaveChosenColorCommand
        {
            get { return new Command<PinColor>(async obj => { await SaveChosenColor(obj); }); }
        }

        public Command CloseSaveCircuitDialogCommand
        {
            get { return new Command(async obj => { await CloseSaveCircuitDialog(); }); }
        }

        public Command SaveCircuitCommand
        {
            get { return new Command(async obj => { await SaveCircuito(); }); }
        }
        #endregion

        public PopupEditarPageController(Circuito circuito)
        {
            Circuito = circuito;
            var ds = new DataStore();
            WayPoints = ds.GetWayPoint(circuito, true);
            ImageColorDialog = GetImageSource(MyImageEnum.ColorDialog);
        }

        private async Task SalvarCircuito()
        {
            try
            {
                var ds = new DataStore();
                ds.UpdateCircuito(Circuito);
                foreach (var wayPoint in WayPoints)
                {
                    ds.SalvarWayPoint(wayPoint);
                }
                await MessageService.ShowAsync("Sucesso!", $"{Circuito.Nome} alterado com sucesso.");
                await ClosePopup();
            }
            catch (Exception exception)
            {
                await MessageService.ShowAsync("Erro", exception.Message);
            }
            finally
            {
                SetBusyStatus(false);
            }
        }

        private async Task ClosePopup()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        public async Task ExcluirWayPoint(WayPoint wayPoint)
        {
            var ask = await MessageService.ShowDialogAsync("Confirma exclusão?", $"Excluir waypoint {wayPoint.Id}?");
            if (!ask) return;

            var ds = new DataStore();
            ds.ExcluirWayPoint(wayPoint);
            WayPoints.Remove(wayPoint);
        }

        private async Task OpenColorPickerDialog(WayPoint wayPoint)
        {
            await Application.Current.MainPage.Navigation.PushPopupAsync(new PopupColorDialog(this));
            WayPointForEdit = wayPoint;
        }

        private async Task SaveChosenColor(PinColor color)
        {
            foreach (var wayPoint in WayPoints)
            {
                if (wayPoint.Id == WayPointForEdit.Id)
                {
                    var wp = new WayPoint
                    {
                        Id = wayPoint.Id,
                        Circuito = wayPoint.Circuito,
                        Cor = color.ColorHexCode,
                        Nome = wayPoint.Nome
                    };
                    var ds = new DataStore();
                    ds.UpdateWayPoint(wp);
                    break;
                }
            }
            await CloseColorDialog();
        }

        private async Task CloseColorDialog()
        {
            await PopupNavigation.Instance.PopAsync();
        }

        public async Task CloseSaveCircuitDialog()
        {
            await PopupNavigation.Instance.PopAsync();
        }

        public async Task SaveCircuito()
        {
            try
            {
                if (string.IsNullOrEmpty(Circuito.Nome))
                {
                    await MessageService.ShowAsync("Falta dados", "É necessário definir um nome para o circuito!");
                    return;
                }
                var ds = new DataStore();
                var c = new Circuito
                {
                    Id = Circuito.Id,
                    Nome = Circuito.Nome,
                    Cidade = Circuito.Cidade,
                    Distancia = Circuito.Distancia
                };
                ds.UpdateCircuito(c);
                await CloseSaveCircuitDialog();
                await MessageService.ShowAsync("Sucesso", $"{Circuito.Nome} salvo com sucesso!");
            }
            catch (Exception exception)
            {
                await MessageService.ShowAsync("Erro", exception.Message);
            }
            finally
            {
                SetBusyStatus(false);
            }
        }
    }
}
