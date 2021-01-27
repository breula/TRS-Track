using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TRSTrack.Custom;
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
        #endregion

        public PopupRaceListPageController(Race currentRace)
        {
            try
            {
                SetBusyStatus(true);
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
            finally
            {
                SetBusyStatus(false);
            }
        }

        public void CatchControl(object control)
        {
            if (control.GetType() == typeof(CustomMap))
            {
                CurrentMapView = (CustomMap)control;
                CurrentMapView.IsShowingUser = false;

                Device.BeginInvokeOnMainThread(async () =>
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
                SetBusyStatus(true);
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
                SetBusyStatus(false);
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
                        cp.MarkerId = GetPinColor(wayPercurso[i].Cor).ColorValue;
                        cp.Name = string.IsNullOrEmpty(wayPercurso[i].Nome) ? $"Waypoint {waypointCount}" : wayPercurso[i].Nome;
                        cp.Label = string.IsNullOrEmpty(wayPercurso[i].Nome) ? $"Waypoint {waypointCount}" : wayPercurso[i].Nome;
                        waypointCount++;
                        CurrentMapView.Pins.Add(cp);
                        CurrentMapView.CustomPins.Add(cp);
                    }
                }

                var polyline = new Polyline
                {
                    StrokeColor = LapColor(raceLap.LapNumber),
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
                SetBusyStatus(false);
            }
        }
    }
}
