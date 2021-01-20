using TRSTrack.Models;
using TRSTrack.Models.Enums;
using TRSTrack.Services;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TRSTrack.Controllers
{
    public class PopupImportarCircuitoPageController : BaseController
    {
        #region-- Properties --
        private PickOptions _pickOptions;
        public PickOptions PickOptions
        {
            get => _pickOptions;
            set
            {
                _pickOptions = value;
                OnPropertyChanged(nameof(PickOptions));
            }
        }

        private CircuitoShare _circuitoLido;
        public CircuitoShare CircuitoLido
        {
            get => _circuitoLido;
            set
            {
                _circuitoLido = value;
                OnPropertyChanged(nameof(CircuitoLido));
            }
        }

        private ImageSource _imageSaveCircuit;
        public ImageSource ImageSaveCircuit
        {
            get => _imageSaveCircuit;
            set
            {
                _imageSaveCircuit = value;
                OnPropertyChanged(nameof(ImageSaveCircuit));
            }
        }

        #endregion

        #region-- Commands --
        public Command ClosePopupCommand
        {
            get { return new Command(async obj => { await ClosePopup(); }); }
        }

        public Command ImportarCommand
        {
            get { return new Command(async obj => { await Importar(); }); }
        }

        public Command AbrirArquivoCirCommand
        {
            get { return new Command(async obj => { await AbrirArquivoCir(); }); }
        }

        #endregion

        public PopupImportarCircuitoPageController()
        {
            ImageSaveCircuit = GetImageSource(MyImageEnum.SaveCircuit);

            var customFileType =
                new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "public.my.comic.extension" } }, // or general UTType values
                    { DevicePlatform.Android, new[] { "application/comics" } }
                });
            PickOptions = new PickOptions
            {
                PickerTitle = "Please select a comic file",
                FileTypes = customFileType,
            };
        }

        private async Task ClosePopup()
        {
            await PopupNavigation.Instance.PopAsync();
        }

        private async Task AbrirArquivoCir()
        {
            try
            {
                var result = await FilePicker.PickAsync();
                if (result == null)
                {
                    await MessageService.ShowAsync("Arquivo nulo", "nenhum arquivo conhecido foi encontrado.");
                    return;
                }

                if (!result.FileName.EndsWith("cir", StringComparison.OrdinalIgnoreCase))
                {
                    await MessageService.ShowAsync("Arquivo inválido", "Um arquivo de circuito válido deve ter extesão do tipo .cir!");
                    return;
                }

                var stream = await result.OpenReadAsync();
                StreamReader reader = new StreamReader(stream);
                string json = reader.ReadToEnd();
                CircuitoLido = JsonConvert.DeserializeObject<CircuitoShare>(json);
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

        private async Task Importar()
        {
            try
            {
                if (CircuitoLido == null)
                {
                    await MessageService.ShowAsync("Falta arquivo!", "Escolha um arquivo tipo .cir antes de importar!");
                    return;
                }

                if (CircuitoLido.WayPoints.Count == 0)
                {
                    await MessageService.ShowAsync("Não pode importar!", "O circuito selecionado não tem dados de steps ou waypoints!");
                    return;
                }

                var ds = new DataStore();
                var existentes = ds.CircuitosMesmoNome(CircuitoLido.Nome);
                if (existentes > 0)
                {
                    var ask = await MessageService.ShowDialogAsync("Circuito já existe", $"Já existe um circuito com este nome! Se escolher importar seu nome será alterado para {CircuitoLido.Nome} {existentes + 1}. Importar?");
                    if (!ask) return;
                    CircuitoLido.Nome = $"{CircuitoLido.Nome} {existentes + 1}";
                }

                SetBusyStatus(true, "Importando...");

                var circuito = new Circuito
                {
                    Id = 0,
                    Nome = CircuitoLido.Nome,
                    Cidade = CircuitoLido.Cidade,
                    Distancia = CircuitoLido.Distancia,
                    Data = new DateTimeOffset(Convert.ToDateTime(CircuitoLido.Data), TimeZoneInfo.Local.GetUtcOffset(DateTime.Now))
                };
                circuito = ds.SalvarCircuito(circuito);
                var coutWaypoints = 0;
                var stepsCount = 0;
                foreach (var wpi in CircuitoLido.WayPoints)
                {
                    var wayPoint = new WayPoint
                    {
                        Id = 0,
                        Circuito = circuito.Id,
                        Nome = wpi.Nome,
                        Cor = wpi.Cor,
                        Latitude = wpi.Latitude,
                        Longitude = wpi.Longitude,
                        Chegada = wpi.Chegada,
                        Largada = wpi.Largada,
                        Distancia = wpi.Distancia,
                        IsWayPoint = wpi.IsWayPont
                    };
                    ds.SalvarWayPoint(wayPoint);
                    if (wpi.IsWayPont) coutWaypoints++; else stepsCount++;
                }
                CircuitoLido = null;
                CircuitCount = ds.CircuitosCount();
                await MessageService.ShowAsync("Sucesso!", $"Circuito {circuito.Nome} importado com {coutWaypoints} waypoints e {stepsCount} steps!");
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
