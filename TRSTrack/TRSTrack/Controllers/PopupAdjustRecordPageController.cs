using TRSTrack.Models;
using TRSTrack.Services;
using Rg.Plugins.Popup.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TRSTrack.Controllers
{
    public class PopupAdjustRecordPageController : BaseController
    {
        #region-- Properties --
        private RecordAdjust _recordAdjust;
        public RecordAdjust RecordAdjust
        {
            get => _recordAdjust;
            set
            {
                _recordAdjust = value;
                OnPropertyChanged(nameof(RecordAdjust));
            }
        }
        #endregion

        #region-- Commands --
        public Command ClosePopupCommand
        {
            get { return new Command(async obj => { await ClosePopup(); }); }
        }

        public Command SaveRecordAjustmentCommand
        {
            get { return new Command(async obj => { await SaveRecordAjustment(); }); }
        }

        #endregion

        public PopupAdjustRecordPageController()
        {
            var ds = new DataStore();
            RecordAdjust = ds.GetRecordAdjust();
        }

        private async Task ClosePopup()
        {
            await PopupNavigation.Instance.PopAsync();
        }

        private async Task SaveRecordAjustment()
        {
            try
            {
                var ds = new DataStore();
                var o = new RecordAdjust
                {
                    Id = RecordAdjust.Id,
                    MinimumMeters = RecordAdjust.MinimumMeters,
                    MinimumVelocity = RecordAdjust.MinimumVelocity
                };
                ds.SalvarRecordAdjust(o);
                await MessageService.ShowAsync("Sucesso!", "Ajustes de gravação salvos com sucesso e serão usados imediatamente!");
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
