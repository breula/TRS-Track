using TRSTrack.Models;
using TRSTrack.Models.Enums;
using TRSTrack.Services;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using TRSTrack.Popups;
using TRSTrack.Helpers;
using System.Collections.Generic;

namespace TRSTrack.Controllers
{
    public class PopupCircuitPageController : BaseController
    {
        #region-- Properties --
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

        private ObservableCollection<Circuito> _circuitos;
        public ObservableCollection<Circuito> Circuitos
        {
            get => _circuitos;
            set
            {
                _circuitos = value;
                OnPropertyChanged(nameof(Circuitos));
            }
        }

        private bool _enableDeleteAll;
        public bool EnableDeleteAll
        {
            get => _enableDeleteAll;
            set
            {
                _enableDeleteAll = value;
                OnPropertyChanged(nameof(EnableDeleteAll));
            }
        }
    
        #endregion

        #region-- Commands --
        public Command SwipeCommand
        {
            get { return new Command(async obj => { await Swipe(); }); }
        }

        public Command ExcluirCircuitoCommand
        {
            get { return new Command<Circuito>(async obj => { await ExcluirCircuito(obj); }); }
        }

        public Command ShareCircuitoCommand
        {
            get { return new Command<Circuito>(async obj => { await ShareCircuito(obj); }); }
        }

        public Command EditarCircuitoCommand
        {
            get { return new Command<Circuito>(async obj => { await EditarCircuito(obj); }); }
        }

        public Command ExcluirTodosCircuitoCommand
        {
            get { return new Command(async obj => { await ExcluirTodosCircuitos(); }); }
        }

        public Command ImportRaceCommand
        {
            get { return new Command<Circuito>(async obj => { await ImportRace(obj); }); }
        }
        #endregion

        public PopupCircuitPageController()
        {
            ImageSaveCircuit = GetImageSource(MyImageEnum.SaveCircuit);
            var ds = new DataStore();
            Circuitos = ds.CircuitoGetList();
            CircuitCount = Circuitos.Count;
            EnableDeleteAll = CircuitCount > 0;
        }

        async Task Swipe()
        {
            await Application.Current.MainPage.Navigation.PopAllPopupAsync();
        }

        public async Task ExcluirTodosCircuitos()
        {
            try
            {
                Tools.Vibrate(new List<KeyValuePair<int, int>>
                {
                    new KeyValuePair<int, int>(30,50),
                    new KeyValuePair<int, int>(20,100)
                });

                var ask = await MessageService.ShowDialogAsync("Confirma exclusão?", "Todos os circuitos salvos serão excluídos!");
                if (!ask) return;

                await SetBusyStatus(true);

                var ds = new DataStore();
                ds.ExcluirTodosCircuitos();

                Circuitos = ds.CircuitoGetList();
                CircuitCount = Circuitos.Count;
                EnableDeleteAll = CircuitCount > 0;
            }
            catch (Exception exception)
            {
                await MessageService.ShowAsync("Erro", exception.Message);
            }
            finally
            {
                await SetBusyStatus(false);
            }
        }

        public async Task ExcluirCircuito(Circuito circuito)
        {
            try
            {
                Tools.Vibrate(40);

                var nomeCircuito = circuito.Nome;
                var ask = await MessageService.ShowDialogAsync("Confirma exclusão?", $"Excluir circuito {nomeCircuito}?");
                if (!ask) return;

                var ds = new DataStore();

                var hasStatistics = ds.CircuitoHasRaceStatistics(circuito);

                if (hasStatistics)
                {
                    ask = await MessageService.ShowDialogAsync("Circuito utilizado?", $"Este circuito tem estatísicas de corrida e ao excluir a estatísitca também será apagada!", "Pode aparagar", "Eita! estão não");
                    if (!ask) return;
                }

                await SetBusyStatus(true);

                ds.ExcluirCircuito(new Circuito { Id = circuito.Id });
                Circuitos = ds.CircuitoGetList();
                CircuitCount = Circuitos.Count;
                EnableDeleteAll = CircuitCount > 0;
            }
            catch (Exception exception)
            {
                await MessageService.ShowAsync("Erro", exception.Message);
            }
            finally
            {
                await SetBusyStatus(false);
            }
        }

        public async Task EditarCircuito(Circuito circuito)
        {
            try
            {
                await Application.Current.MainPage.Navigation.PushPopupAsync(new PopupEditarCircuito(circuito));
            }
            catch (Exception exception)
            {
                await MessageService.ShowAsync("Erro", exception.Message);
            }
            finally
            {
                await SetBusyStatus(false);
            }
        }

        public async Task ShareCircuito(Circuito circuito)
        {
            try
            {
                //Construir arquivo
                var shareFile = new CircuitoShare
                {
                    Cidade = circuito.Cidade,
                    Distancia = circuito.Distancia,
                    Nome = circuito.Nome,
                    Data = circuito.Data.UtcDateTime
                };

                var ds = new DataStore();
                var wayPoints = ds.GetWayPoint(circuito);
                foreach (var wayPoint in wayPoints)
                {
                    shareFile.WayPoints.Add(new CircuitoShareWayPoint
                        {
                            Circuito = circuito.Id,
                            Nome = wayPoint.Nome,
                            Cor = wayPoint.Cor,
                            Latitude = wayPoint.Latitude,
                            Longitude = wayPoint.Longitude,
                            Largada = wayPoint.Largada,
                            Chegada = wayPoint.Chegada,
                            IsWayPont = wayPoint.IsWayPoint,
                            Distancia = wayPoint.Distancia,
                        }
                    );
                }
                var json = JsonConvert.SerializeObject(shareFile);
                var fn = $"{circuito.Nome}.cir";
                var file = Path.Combine(FileSystem.CacheDirectory, fn);
                File.WriteAllText(file, json);

                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = circuito.Nome,
                    File = new ShareFile(file)
                });
            }
            catch (Exception exception)
            {
                await MessageService.ShowAsync("Erro", exception.Message);
            }
            finally
            {
                await SetBusyStatus(false);
            }
        }

        public async Task ImportRace(Circuito circuito)
        {
            try
            {
                var result = await FilePicker.PickAsync();
                if (result == null)
                {
                    await MessageService.ShowAsync("Arquivo nulo", "Nenhum arquivo conhecido foi encontrado.");
                    return;
                }

                if (!result.FileName.EndsWith("rce", StringComparison.OrdinalIgnoreCase))
                {
                    await MessageService.ShowAsync("Arquivo inválido", "Um arquivo de corrida válido deve ter extesão do tipo .rce!");
                    return;
                }

                await SetBusyStatus(true, "Importando...");

                var stream = await result.OpenReadAsync();
                StreamReader reader = new StreamReader(stream);
                string json = reader.ReadToEnd();

                var raceShare = JsonConvert.DeserializeObject<RaceShare>(json);

                var ds = new DataStore();

                //var dtfs = new DateTimeOffset();
                //DateTimeOffset.TryParse($"{CircuitoLido.Data:dd/MM/yyyy HH:mm:ss}", null, DateTimeStyles.AssumeUniversal, out dtfs);

                //var race = new Race
                //{
                //    Id = 0,
                //    Nome = raceShare.Nome,
                //    Circuito = circuito.Id,
                //    DisplayName = raceShare.DisplayName,
                //};
                //race = ds.SalvarCorrida(race);

                //foreach (var lap in raceShare.RaceShareLapList)
                //{
                //    ds.salvar
                //}

                //var coutWaypoints = 0;
                //var stepsCount = 0;
                //foreach (var wpi in CircuitoLido.WayPoints)
                //{
                //    var wayPoint = new WayPoint
                //    {
                //        Id = 0,
                //        Circuito = circuito.Id,
                //        Nome = wpi.Nome,
                //        Cor = wpi.Cor,
                //        Latitude = wpi.Latitude,
                //        Longitude = wpi.Longitude,
                //        Chegada = wpi.Chegada,
                //        Largada = wpi.Largada,
                //        Distancia = wpi.Distancia,
                //        IsWayPoint = wpi.IsWayPont
                //    };
                //    ds.SalvarWayPoint(wayPoint);
                //    if (wpi.IsWayPont) coutWaypoints++; else stepsCount++;
                //}
                //CircuitoLido = null;
                //CircuitCount = ds.CircuitosCount();
               // await MessageService.ShowAsync("Sucesso!", $"Circuito {circuito.Nome} importado com {coutWaypoints} waypoints e {stepsCount} steps!");
            }
            catch (Exception exception)
            {
                await MessageService.ShowAsync("Erro", exception.Message);
            }
            finally
            {
                await SetBusyStatus(false);
            }
        }
    }
}
