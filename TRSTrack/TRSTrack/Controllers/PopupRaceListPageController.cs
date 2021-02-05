using Acr.UserDialogs;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TRSTrack.Custom;
using TRSTrack.Helpers;
using TRSTrack.Models;
using TRSTrack.Models.Enums;
using TRSTrack.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace TRSTrack.Controllers
{
    public class PopupRaceListPageController : BaseController
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

        private ObservableCollection<Race> _corridas;
        public ObservableCollection<Race> Corridas
        {
            get => _corridas;
            set
            {
                _corridas = value;
                OnPropertyChanged(nameof(Corridas));
            }
        }

        private Race _corridaEscolhida;
        public Race CorridaEscolhida
        {
            get => _corridaEscolhida;
            set
            {
                _corridaEscolhida = value;
                OnPropertyChanged(nameof(CorridaEscolhida));
                if (CorridaEscolhida != null)
                    LoadLaps();
            }
        }

        private CustomMap _currentMapView;
        public CustomMap CurrentMapView
        {
            get => _currentMapView;
            set
            {
                _currentMapView = value;
                OnPropertyChanged(nameof(CurrentMapView));
            }
        }

        private RaceDataShow _raceDataShow;
        public RaceDataShow RaceDataShow
        {
            get => _raceDataShow;
            set
            {
                _raceDataShow = value;
                OnPropertyChanged(nameof(RaceDataShow));
            }
        }
        private ObservableCollection<RaceLapDataShow> _raceLapDataShow;
        public ObservableCollection<RaceLapDataShow> RaceLapDataShow
        {
            get => _raceLapDataShow;
            set
            {
                _raceLapDataShow = value;
                OnPropertyChanged(nameof(RaceLapDataShow));
            }
        }
        private ObservableCollection<RaceLapPartialDataShow> _raceLapPartialDataShow;
        public ObservableCollection<RaceLapPartialDataShow> RaceLapPartialDataShow
        {
            get => _raceLapPartialDataShow;
            set
            {
                _raceLapPartialDataShow = value;
                OnPropertyChanged(nameof(RaceLapPartialDataShow));
            }
        }

        private ObservableCollection<RaceLapTrack> _lapTrack;
        public ObservableCollection<RaceLapTrack> LapTrack
        {
            get => _lapTrack;
            set
            {
                _lapTrack = value;
                OnPropertyChanged(nameof(LapTrack));
            }
        }

        private int _mapZoom;
        public int MapZoom
        {
            get => _mapZoom;
            set
            {
                _mapZoom = value;
                OnPropertyChanged(nameof(MapZoom));
                if (CurrentMapView != null)
                {
                    CurrentMapView.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(LatitudeInicial, LongitudeInicial), Distance.FromMeters(MapZoom)));
                }
            }
        }

        private double _latitudeInicial;
        public double LatitudeInicial
        {
            get => _latitudeInicial;
            set
            {
                _latitudeInicial = value;
                OnPropertyChanged(nameof(LatitudeInicial));
            }
        }
        private double _longitudeInicial;
        public double LongitudeInicial
        {
            get => _longitudeInicial;
            set
            {
                _longitudeInicial = value;
                OnPropertyChanged(nameof(LongitudeInicial));
            }
        }

        #endregion

        #region-- Commands --
        public Command SwipeCommand
        {
            get { return new Command(async obj => { await Swipe(); }); }
        }

        public Command IsolateLapCommand
        {
            get { return new Command<RaceLapDataShow>(async obj => { await IsolateLap(obj); }); }
        }

        public Command ShareRaceCommand
        {
            get { return new Command<RaceLapDataShow>(async obj => { await ShareRace(); }); }
        }

        public Command DeleteRaceCommand
        {
            get { return new Command<RaceLapDataShow>(async obj => { await DeleteRace(); }); }
        }
        #endregion

        public PopupRaceListPageController(Race currentRace)
        {
            try
            {
                UserDialogs.Instance.HideLoading();
                ImageSaveCircuit = GetImageSource(MyImageEnum.RaceList);
                MapZoom = 50;
                LatitudeInicial = 0;
                LongitudeInicial = 0;

                var ds = new DataStore();
                Corridas = currentRace != null
                    ? ds.GetReces(currentRace)
                    : ds.GetReces();
                CorridaEscolhida = Corridas[0];
            }
            catch (Exception exception)
            {
                MessageService.Show("Erro", $"{exception.Message}");
            }
        }

        public void CatchControl(object control)
        {
            if (control.GetType() == typeof(CustomMap))
            {
                CurrentMapView = (CustomMap)control;
                CurrentMapView.IsShowingUser = false;

                Task.Run(async () =>
                {
                    var location = await Geolocation.GetLocationAsync();
                    LatitudeInicial = location.Latitude;
                    LongitudeInicial = location.Longitude;
                    CurrentMapView.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(LatitudeInicial, LongitudeInicial), Distance.FromMeters(MapZoom)));
                });
            }
        }

        async Task Swipe()
        {
            await Application.Current.MainPage.Navigation.PopAllPopupAsync();
        }

        private async void LoadLaps()
        {
            try
            {
                await SetBusyStatus(true);
                var ds = new DataStore();
                var circuito = ds.CircuitoGet(CorridaEscolhida.Circuito);
                RaceDataShow = new RaceDataShow
                {
                    Circuito = circuito.Nome,
                    CircuitoCidade = circuito.Cidade,
                    CircuitoDistancia = circuito.Distancia,
                    Data = $"{CorridaEscolhida.Data:dd/MM/yyyy HH:mm:ss}",
                    DisplayName = $"Corrida {CorridaEscolhida.Nome}, {circuito.Nome} em {CorridaEscolhida.Data:dd/MM/yyyy}",
                    Nome = CorridaEscolhida.Nome
                };
                var voltas = ds.GetLaps(CorridaEscolhida);
                RaceLapDataShow = new ObservableCollection<RaceLapDataShow>();
                RaceLapPartialDataShow = new ObservableCollection<RaceLapPartialDataShow>();
                foreach (var volta in voltas)
                {
                    var jaExiste = false;
                    foreach (var x in RaceLapDataShow)
                    {
                        if (x.LapNumber == volta.LapNumber) { jaExiste = true; break; }
                    }
                    if (!jaExiste)
                    {
                        var rlds = new RaceLapDataShow
                        {
                            LapNumber = volta.LapNumber,
                            TempoTotal = volta.TempoTotal,
                            VelocidadeMedia = volta.VelocidadeMedia
                        };
                        
                        var partials = ds.GetLapPartials(volta);
                        foreach (var partial in partials)
                        {
                            if (string.IsNullOrEmpty(partial.DescricaoPassagem)) continue;
                            jaExiste = false;
                            foreach (var item in rlds.RaceLapPartialDataList)
                            {
                                if (item.DescricaoPassagem.ToLower().Trim() == "largada" && partial.DescricaoPassagem.ToLower().Trim() == "largada")
                                {
                                    jaExiste = true;
                                    break;
                                }
                            }
                            if (!jaExiste)
                            {
                                rlds.RaceLapPartialDataList.Add(new RaceLapPartialDataShow
                                {
                                    DescricaoPassagem = partial.DescricaoPassagem.Trim(),
                                    TempoPassagem = partial.TempoPassagem,
                                    VelocidadePassagem = partial.VelocidadePassagem
                                });
                            }
                        }

                        RaceLapDataShow.Add(rlds);
                    }
                }
            }
            catch (Exception exception)
            {
                await MessageService.ShowAsync("Erro", $"{exception.Message}");
            }
            finally
            {
                await SetBusyStatus(false);
            }
        }

        private async Task IsolateLap(RaceLapDataShow raceLap)
        {
            try
            {
                if (CurrentMapView != null)
                {
                    CurrentMapView.MapElements.Clear();
                    CurrentMapView.Pins.Clear();
                    CurrentMapView.CustomPins.Clear();
                }

                var ds = new DataStore();
                var wayPercurso = ds.GetWayPoint(new Circuito { Id = CorridaEscolhida.Circuito }, true);
                var waypointCount = 1;
                for (var i = 0; i < wayPercurso.Count; i++)
                {
                    var cp = new CustomPin
                    {
                        Label = string.IsNullOrEmpty(wayPercurso[i].Nome)
                            ? $"Waypoint {wayPercurso[i].Id}"
                            : wayPercurso[i].Nome,
                        Position = new Position(wayPercurso[i].Latitude, wayPercurso[i].Longitude),
                        Type = PinType.SavedPin
                    };

                    if (wayPercurso[i].Largada == true)
                    {
                        cp.MarkerId = "Largada";
                        cp.Name = "Largada";
                        cp.Label = "Largada";
                        CurrentMapView.Pins.Add(cp);
                        CurrentMapView.CustomPins.Add(cp);
                    }
                    else if (wayPercurso[i].Chegada == true)
                    {
                        cp.MarkerId = "Chegada";
                        cp.Name = "Chegada";
                        cp.Label = "Chegada";
                        CurrentMapView.Pins.Add(cp);
                        CurrentMapView.CustomPins.Add(cp);
                    }
                    else if (wayPercurso[i].IsWayPoint == true)
                    {
                        cp.MarkerId = Tools.GetPinColor(wayPercurso[i].Cor).ColorValue;
                        cp.Name = string.IsNullOrEmpty(wayPercurso[i].Nome) ? $"Waypoint {waypointCount}" : wayPercurso[i].Nome;
                        cp.Label = string.IsNullOrEmpty(wayPercurso[i].Nome) ? $"Waypoint {waypointCount}" : wayPercurso[i].Nome;
                        waypointCount++;
                        CurrentMapView.Pins.Add(cp);
                        CurrentMapView.CustomPins.Add(cp);
                    }
                }

                var polyline = new Polyline
                {
                    StrokeColor = Tools.LapColor(raceLap.LapNumber),
                    StrokeWidth = 8
                };

                var lap = ds.GetLap(CorridaEscolhida, raceLap.LapNumber);

                LapTrack = ds.GetLapTrack(CorridaEscolhida, lap);

                var inicio = LapTrack.ToList()[0];
                LatitudeInicial = inicio.Latitude;
                LongitudeInicial = inicio.Longitude;

                polyline.Geopath.Clear();
                for (var i = 0; i < LapTrack.Count; i++)
                {
                    var p = new Position(LapTrack[i].Latitude, LapTrack[i].Longitude);
                    polyline.Geopath.Insert(i, p);
                }
                CurrentMapView.MapElements.Add(polyline);
                CurrentMapView.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(LatitudeInicial, LongitudeInicial), Distance.FromMeters(MapZoom)));

            }
            catch (Exception exception)
            {
                await MessageService.ShowAsync("Erro", $"{exception.Message}");
            }
            finally
            {
                await SetBusyStatus(false);
            }
        }

        private async Task ShareRace()
        {
            try
            {
                var ds = new DataStore();

                var race = ds.GetReces(CorridaEscolhida);
                var raceName = string.IsNullOrEmpty(race[0].Nome) ? $"Export {race[0].Id}" : race[0].Nome;
                //Construir arquivo
                var shareFile = new RaceShare
                {
                    Nome = raceName,
                    DisplayName = race[0].DisplayName,
                    Cpf = race[0].Cpf,
                    Data = race[0].Data
                };

                var laps = ds.GetLaps(CorridaEscolhida);
                foreach (var lap in laps)
                {
                    shareFile.RaceShareLapList.Add(new RaceShareLap
                    {
                        LapNumber = lap.LapNumber,
                        TempoTotal = lap.TempoTotal,
                        VelocidadeMedia = lap.VelocidadeMedia
                    });

                    var partials = ds.GetLapPartials(lap);
                    foreach (var partial in partials)
                    {
                        shareFile.RaceShareLapPartialList.Add(new RaceShareLapPartial
                        {
                            NumeroPassagem = partial.NumeroPassagem,
                            VelocidadePassagem = partial.NumeroPassagem,
                            TempoPassagem = partial.TempoPassagem,
                            DescricaoPassagem = partial.DescricaoPassagem
                        });
                    };

                    var tracks = ds.GetLapTrack(race[0], lap);
                    foreach (var track in tracks)
                    {
                        shareFile.RaceShareLapTrackList.Add(new RaceShareLapTrack
                        {
                            LapNumber = track.LapNumber,
                            Latitude = track.Latitude,
                            Longitude = track.Longitude
                        });
                    }
                }

                var json = JsonConvert.SerializeObject(shareFile);
                var fn = $"{raceName}.rce";
                var file = Path.Combine(FileSystem.CacheDirectory, fn);
                File.WriteAllText(file, json);

                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = raceName,
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

        private async Task DeleteRace()
        {
            try
            {
                Tools.Vibrate(new List<KeyValuePair<int, int>>
                {
                    new KeyValuePair<int, int>(30,50),
                    new KeyValuePair<int, int>(20,100)
                });

                var ask = await MessageService.ShowDialogAsync("Confirma exclusão?", "Excluir esta corrida!");
                if (!ask) return;

                await SetBusyStatus(true);

                var ds = new DataStore();
                ds.ExcluirCorrida(CorridaEscolhida);
                Corridas = ds.GetReces();
                if (Corridas.Count > 0)
                {
                    CorridaEscolhida = Corridas[0];
                    LoadLaps();
                    return;
                }
                await MessageService.ShowAsync("Sem corridas", "Não existem corridas para listar");
                await PopupNavigation.Instance.PopAsync();
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
