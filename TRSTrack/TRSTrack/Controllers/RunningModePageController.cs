using Acr.UserDialogs;
using Plugin.Geolocator;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TRSTrack.Custom;
using TRSTrack.Helpers;
using TRSTrack.Models;
using TRSTrack.Popups;
using TRSTrack.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace TRSTrack.Controllers
{
    public class RunningModePageController : BaseController
    {
        #region-- Properties --
        private DateTime _justPassedLargadaLargada;
        public DateTime JustPassedLargada
        {
            get => _justPassedLargadaLargada;
            set
            {
                _justPassedLargadaLargada = value;
                OnPropertyChanged(nameof(JustPassedLargada));
            }
        }

        private DateTime _justPassedLargadaChegada;
        public DateTime JustPassedChegada
        {
            get => _justPassedLargadaChegada;
            set
            {
                _justPassedLargadaChegada = value;
                OnPropertyChanged(nameof(JustPassedChegada));
            }
        }

        private DateTime _justPassedLargadaWaypoint;
        public DateTime JustPassedChegadaWaypoint
        {
            get => _justPassedLargadaWaypoint;
            set
            {
                _justPassedLargadaWaypoint = value;
                OnPropertyChanged(nameof(JustPassedChegadaWaypoint));
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

        private Page _currentPage;
        public Page CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }

        private ScreenOptions _screenOptions;
        public ScreenOptions ScreenOptions
        {
            get => _screenOptions;
            set
            {
                _screenOptions = value;
                OnPropertyChanged(nameof(ScreenOptions));
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

        private Circuito _circuitoEscolhido;
        public Circuito CircuitoEscolhido
        {
            get => _circuitoEscolhido;
            set
            {
                _circuitoEscolhido = value;
                OnPropertyChanged(nameof(CircuitoEscolhido));
                LoadWayPoints();
            }
        }

        private ObservableCollection<WayPoint> _wayPercurso;
        public ObservableCollection<WayPoint> WayPercurso
        {
            get => _wayPercurso;
            set
            {
                _wayPercurso = value;
                OnPropertyChanged(nameof(WayPercurso));
            }
        }

        private ObservableCollection<WayPoint> _raceMarkers;
        public ObservableCollection<WayPoint> RaceMarkers
        {
            get => _raceMarkers;
            set
            {
                _raceMarkers = value;
                OnPropertyChanged(nameof(RaceMarkers));
            }
        }

        private WayPoint _largada;
        public WayPoint Largada
        {
            get => _largada;
            set
            {
                _largada = value;
                OnPropertyChanged(nameof(Largada));
            }
        }

        private WayPoint _chegada;
        public WayPoint Chegada
        {
            get => _chegada;
            set
            {
                _chegada = value;
                OnPropertyChanged(nameof(Chegada));
            }
        }

        private ObservableCollection<RaceLapTempItem> _raceLapsList;
        public ObservableCollection<RaceLapTempItem> RaceLapsList
        {
            get => _raceLapsList;
            set
            {
                _raceLapsList = value;
                OnPropertyChanged(nameof(RaceLapsList));
            }
        }

        private int _lapNumner;
        public int LapNumber
        {
            get => _lapNumner;
            set
            {
                _lapNumner = value;
                OnPropertyChanged(nameof(LapNumber));
            }
        }

        private bool _hasRaceStatistics;
        public bool HasRaceStatistics
        {
            get => _hasRaceStatistics;
            set
            {
                _hasRaceStatistics = value;
                OnPropertyChanged(nameof(HasRaceStatistics));
            }
        }

        private Color _currentLapColor;
        public Color CurrentLapColor
        {
            get => _currentLapColor;
            set
            {
                _currentLapColor = value;
                OnPropertyChanged(nameof(CurrentLapColor));
            }
        }

        private int _markLaplistCount;
        public int MarkLaplistCount
        {
            get => _markLaplistCount;
            set
            {
                _markLaplistCount = value;
                OnPropertyChanged(nameof(MarkLaplistCount));
            }
        }
        #endregion

        #region-- Commands --
        public Command ShowWayPointsListCommand
        {
            get { return new Command(obj => { ShowWayPointsList(); }); }
        }

        public Command ChangeMapTypeCommand
        {
            get { return new Command(obj => { ChangeMapType(); }); }
        }

        public Command IncreaseMapZoomCommand
        {
            get { return new Command(async obj => { await IncreaseMapZoom(); }); }
        }

        public Command DecreaseMapZoomCommand
        {
            get { return new Command(async obj => { await DecreaseMapZoom(); }); }
        }

        public Command StartRunCommand
        {
            get { return new Command(async obj => { await StartRun(); }); }
        }

        public Command StopRunCommand
        {
            get { return new Command(obj => { StopRun(); }); }
        }

        public Command OpenRaceStatisticsCommand
        {
            get { return new Command(obj => { OpenRaceStatistics(); }); }
        }
        #endregion

        public RunningModePageController()
        {
            try
            {
                SetBusyStatus(true, "Iniciando...");
                Circuitos = new ObservableCollection<Circuito>();
                
                var ds = new DataStore();
                Circuitos = ds.CircuitoGetList();
                CircuitCount = ds.CircuitosCount();
                var rmp = ds.GetRadialMenuPosition();
                RadialMenuPointPosition = new Point(rmp.X, rmp.Y);
                var cmz = ds.GetCurrentMapZoom();
                CurrentMapZoom = new MapZoom
                {
                    Id = cmz.Id,
                    Level = cmz.Level
                };
                var so = ds.GetScreenOptions();
                ScreenOptions = new ScreenOptions
                {
                    Id = so.Id,
                    MapType = so.MapType,
                    ShowMenuBotoes = so.ShowMenuBotoes,
                    ShowVelocimeter = so.ShowVelocimeter,
                    ShowWayPointsList = so.ShowWayPointsList,
                    IsUnlocked = so.IsUnlocked
                };
                CurrentMapType = ScreenOptions.MapType == 0 ? MapType.Street : MapType.Satellite;
                JustPassedLargada = DateTime.MinValue;
                JustPassedChegadaWaypoint = JustPassedLargada;
                JustPassedChegada = JustPassedChegadaWaypoint;
                CurrentLapColor = Color.Aqua;
                MarkLaplistCount = 0;
                Task.Run(StartMap).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                MessageService.Show("Erro", exception.Message);
            }
            finally
            {
                SetBusyStatus(false);
            }
        }

        public void CatchControl(object control)
        {
            if (control.GetType() == typeof(Page))
            {
                CurrentPage = (Page)control;
            }
            else if (control.GetType() == typeof(CustomMap))
            {
                CurrentMapView = (CustomMap)control;
                CurrentMapView.IsShowingUser = true;

                Device.BeginInvokeOnMainThread(async () =>
                {
                    var location = await Geolocation.GetLocationAsync();
                    CurrentMapView.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(location.Latitude, location.Longitude), Distance.FromMeters(CurrentMapZoom.Level)));
                    Velocidade = location.Speed.HasValue
                        ? location.Speed > 0
                            ? (int)Math.Ceiling(location.Speed.Value * 3.6)
                            : 0
                        : 0;
                });
            }
        }

        private async Task StartMap()
        {
            try
            {
                var position = await CrossGeolocator.Current.GetPositionAsync();
                CurrentMapView.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude), Distance.FromMeters(CurrentMapZoom.Level)));
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

        private async void LoadWayPoints()
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
                WayPercurso = ds.GetWayPoint(CircuitoEscolhido);
                RaceMarkers = new ObservableCollection<WayPoint>();

                Largada = WayPercurso.Where(p => p.Largada == true).ToList()[0];
                Chegada = WayPercurso.Where(p => p.Chegada == true).ToList()[0];

                var polyline = new Polyline
                {
                    StrokeColor = Color.DodgerBlue,
                    StrokeWidth = 20
                };

                polyline.Geopath.Clear();
                var jump = 0;
                var waypointCount = 1;
                for (var i = 0; i < WayPercurso.Count; i++)
                {
                    if (!WayPercurso[i].IsWayPoint)
                    {
                        var p = new Position(WayPercurso[i].Latitude, WayPercurso[i].Longitude);
                        polyline.Geopath.Insert(i- jump, p);
                    }

                    var cp = new CustomPin
                    {
                        Label = string.IsNullOrEmpty(WayPercurso[i].Nome)
                            ? $"Waypoint {WayPercurso[i].Id}"
                            : WayPercurso[i].Nome,
                        Position = new Position(WayPercurso[i].Latitude, WayPercurso[i].Longitude),
                        Type = PinType.SavedPin
                    };

                    if (WayPercurso[i].Largada == true)
                    {
                        cp.MarkerId = "Largada";
                        cp.Name = "Largada";
                        cp.Label = "Largada";
                        RaceMarkers.Add(WayPercurso[i]);
                        CurrentMapView.Pins.Add(cp);
                        CurrentMapView.CustomPins.Add(cp);
                    }
                    else if (WayPercurso[i].Chegada == true)
                    {
                        cp.MarkerId = "Chegada";
                        cp.Name = "Chegada";
                        cp.Label = "Chegada";
                        RaceMarkers.Add(WayPercurso[i]);
                        CurrentMapView.Pins.Add(cp);
                        CurrentMapView.CustomPins.Add(cp);
                    }
                    else if (WayPercurso[i].IsWayPoint == true)
                    {
                        jump++;
                        cp.MarkerId = GetPinColor(WayPercurso[i].Cor).ColorValue;
                        cp.Name = string.IsNullOrEmpty(WayPercurso[i].Nome) ? $"Waypoint {waypointCount}" : WayPercurso[i].Nome;
                        cp.Label = string.IsNullOrEmpty(WayPercurso[i].Nome) ? $"Waypoint {waypointCount}" : WayPercurso[i].Nome;
                        waypointCount++;
                        RaceMarkers.Add(WayPercurso[i]);
                        CurrentMapView.Pins.Add(cp);
                        CurrentMapView.CustomPins.Add(cp);
                    }
                }
                CurrentMapView.MapElements.Add(polyline);
                CurrentMapView.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(Largada.Latitude, Largada.Longitude), Distance.FromMeters(CurrentMapZoom.Level)));
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

        private void ShowWayPointsList()
        {
            //if (!ScreenOptions.IsUnlocked)
            //{
            //    MessageService.Show("Bloqueado", "Necessário desbloquear menu primeiro!");
            //    return;
            //}
            ScreenOptions.ShowWayPointsList = !ScreenOptions.ShowWayPointsList;
            SaveScreenOptions();
        }

        private void ChangeMapType()
        {
            CurrentMapType = CurrentMapType == MapType.Street
                ? MapType.Satellite
                : MapType.Street;
            SaveScreenOptions();
        }

        private async Task DecreaseMapZoom()
        {
            if (CurrentMapZoom.Level >= 1000)
            {
                UserDialogs.Instance.Toast("Zoom máximo de 1000 m", new TimeSpan(3));
                return;
            };

            CurrentMapZoom.Level += 100;
            var ds = new DataStore();
            ds.SalvarCurrentMapZoom(new MapZoom { Id = CurrentMapZoom.Id, Level = CurrentMapZoom.Level });
            if (Largada == null)
            {
                var location = await Geolocation.GetLocationAsync();
                CurrentMapView.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(location.Latitude, location.Longitude), Distance.FromMeters(CurrentMapZoom.Level)));
            }
            else
            {
                CurrentMapView.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(Largada.Latitude, Largada.Longitude), Distance.FromMeters(CurrentMapZoom.Level)));
            }
        }

        private async Task IncreaseMapZoom()
        {
            if (CurrentMapZoom.Level <= 10)
            {
                UserDialogs.Instance.Toast("Zoom máximo de 10 m", new TimeSpan(3));
                return;
            };

            if (CurrentMapZoom.Level > 100)
            {
                CurrentMapZoom.Level -= 100;
            }
            else if (CurrentMapZoom.Level == 100)
            {
                CurrentMapZoom.Level = 50;
            }
            else if (CurrentMapZoom.Level <= 50)
            {
                CurrentMapZoom.Level -= 10;
            }
            var ds = new DataStore();
            ds.SalvarCurrentMapZoom(new MapZoom { Id = CurrentMapZoom.Id, Level = CurrentMapZoom.Level });
            if (Largada == null)
            {
                var location = await Geolocation.GetLocationAsync();
                CurrentMapView.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(location.Latitude, location.Longitude), Distance.FromMeters(CurrentMapZoom.Level)));
            }
            else
            {
                CurrentMapView.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(Largada.Latitude, Largada.Longitude), Distance.FromMeters(CurrentMapZoom.Level)));
            }
        }

        private void SaveScreenOptions()
        {
            var o = new ScreenOptions
            {
                Id = 1,
                ShowVelocimeter = ScreenOptions.ShowVelocimeter,
                ShowMenuBotoes = ScreenOptions.ShowMenuBotoes,
                ShowWayPointsList = ScreenOptions.ShowWayPointsList,
                MapType = CurrentMapType == MapType.Street ? 0 : 1,
                IsUnlocked = ScreenOptions.IsUnlocked,
            };
            var ds = new DataStore();
            ds.SalvarScreenOptions(o);
        }

        private async Task StartRun()
        {
            if (WayPercurso == null)
            {
                await MessageService.ShowAsync("Opa!", "Escolha um circuito primeiro por favor");
                return;
            }
            if (LapNumber > 0)
            {
                var ask = await MessageService.ShowDialogAsync("Apagar tudo?", "Se der play novamente os dados da corrida atual serão apagados, deseja continuar?");
                if (!ask) return;
                ListeningPosition.Clear();
                CurrentMapView.MapElements.Clear();
                CurrentMapView.Pins.Clear();
                LoadWayPoints();
            }

            Tools.Vibrate(40);
            TimeLapsed.Play();
            LapNumber = 0;
            RaceLapsList = new ObservableCollection<RaceLapTempItem>();
            ChangeKeepTrakRule(true);
            MessagingCenter.Send("1", "TRSTrackService");
        }

        private void StopRun()
        {
            Tools.Vibrate(40);
            ChangeKeepTrakRule(false);
            MessagingCenter.Send("0", "TRSTrackService");
            HasRaceStatistics = RaceLapsList.Count > 0;
        }

        private void ChangeKeepTrakRule(bool keepTrack)
        {
            KeepTrakingPosition = keepTrack;
            IsRecordPaused = false;
            if (keepTrack)
            {
                ThreadChronometer = new Thread(UpdateChronometer);
                ThreadChronometer.Start();
            }
            else
            {
                if (ThreadChronometer != null)
                {
                    while (ThreadChronometer.IsAlive)
                    {
                        ThreadChronometer.Abort();
                    }
                    ThreadChronometer = null;
                }
            }
            CurrentChronometer = TimeLapsed.Display();
        }

        private void UpdateChronometer()
        {
            while (KeepTrakingPosition)
            {
                CurrentChronometer = TimeLapsed.Display();
                Thread.Sleep(1);
            }
        }

        public void UpdadeReceData()
        {
            if (!KeepTrakingPosition) return;

            var lastRecord = ListeningPosition.Last();
            if (lastRecord == null) return;

            if (RaceLapsList.Count > 0)
            {
                var lastPloting = RaceLapsList.Last();
                if (lastPloting.Latitude == lastRecord.Latitude && lastPloting.Longitude == lastRecord.Longitude) return;
            }

            //Verifica se está em um ponto do percurso(raio de 5 metros)
            string wayPointName = null;
            var isChegada = false;
            var isLargada = false;

            foreach (var ponto in RaceMarkers)
            {
                var distance = Tools.GetDistance(
                    lastRecord.Latitude,
                    lastRecord.Longitude,
                    ponto.Latitude,
                    ponto.Longitude);

                if (distance >= 5) continue;

                if (ponto.Largada)
                {
                    isLargada = true;
                    if (JustPassedLargada == DateTime.MinValue)
                    {
                        JustPassedLargada = DateTime.Now;
                        Tools.Vibrate(new List<KeyValuePair<int, int>>
                        {
                            new KeyValuePair<int, int>(30,50),
                            new KeyValuePair<int, int>(20,120),
                            new KeyValuePair<int, int>(30,50)
                        });
                        UserDialogs.Instance.Toast($"Passou pela Largada, raio de {distance:N3}m", new TimeSpan(3));
                        LapNumber++;
                        CurrentLapColor = LapColor(LapNumber);
                                
                    }
                    else if (DateTime.Now.Subtract(JustPassedLargada).TotalSeconds >= 30)
                    {
                        Tools.Vibrate(new List<KeyValuePair<int, int>>
                        {
                            new KeyValuePair<int, int>(30,50),
                            new KeyValuePair<int, int>(20,120),
                            new KeyValuePair<int, int>(30,50)
                        });
                        UserDialogs.Instance.Toast($"Passou pela Largada, raio de {distance:N3}m", new TimeSpan(3));
                        LapNumber++;
                        JustPassedLargada = DateTime.Now;
                        CurrentLapColor = LapColor(LapNumber);
                        MarkLaplistCount = RaceLapsList.Count;
                    }
                    wayPointName = "Largada";
                }
                else if (ponto.Chegada)
                {
                    isChegada = true;
                    if (JustPassedChegada == DateTime.MinValue)
                    {
                        JustPassedChegada = DateTime.Now;
                        Tools.Vibrate(new List<KeyValuePair<int, int>>
                        {
                            new KeyValuePair<int, int>(30,50),
                            new KeyValuePair<int, int>(20,80),
                            new KeyValuePair<int, int>(20,120),
                            new KeyValuePair<int, int>(20,80),
                            new KeyValuePair<int, int>(30,50)
                        });
                        UserDialogs.Instance.Toast($"Passou pela Chegada, raio de {distance:N3}m", new TimeSpan(3));
                        TimeLapsed.Reset();
                    }
                    else if (DateTime.Now.Subtract(JustPassedChegada).TotalSeconds >= 30)
                    {
                        Tools.Vibrate(new List<KeyValuePair<int, int>>
                        {
                            new KeyValuePair<int, int>(30,50),
                            new KeyValuePair<int, int>(20,80),
                            new KeyValuePair<int, int>(20,120),
                            new KeyValuePair<int, int>(20,80),
                            new KeyValuePair<int, int>(30,50)
                        });
                        UserDialogs.Instance.Toast($"Passou pela Chegada, raio de {distance:N3}m", new TimeSpan(3));
                        JustPassedChegada = DateTime.Now;
                        TimeLapsed.Reset();
                    }
                    wayPointName = "Chegada";
                }
                else
                {
                    if (JustPassedChegadaWaypoint == DateTime.MinValue)
                    {
                        JustPassedChegadaWaypoint = DateTime.Now;
                        Tools.Vibrate(100);
                        UserDialogs.Instance.Toast($"Passou por {ponto.Nome}, raio de {distance:N3}m", new TimeSpan(3));
                    }
                    else if(DateTime.Now.Subtract(JustPassedChegadaWaypoint).TotalSeconds >= 30)
                    {
                        Tools.Vibrate(100);
                        UserDialogs.Instance.Toast($"Passou por {ponto.Nome}, raio de {distance:N3}m", new TimeSpan(3));
                        JustPassedChegadaWaypoint = DateTime.Now;
                    }
                    wayPointName = ponto.Nome;
                }

                break;
            };

            if (LapNumber == 0) return;

            RaceLapsList.Add(new RaceLapTempItem
            {
                TempoParcial = TimeLapsed.Display(),
                Velocidade = lastRecord.Velocidade,
                CheckPoint = wayPointName,
                LapNumber = LapNumber,
                Latitude = lastRecord.Latitude,
                Longitude = lastRecord.Longitude,
                Largada = isLargada,
                Chegada = isChegada
            });

            var polyline = new Polyline
            {
                StrokeColor = CurrentLapColor,
                StrokeWidth = 10
            };

            for (int i = MarkLaplistCount; i < RaceLapsList.Count(); i++)
            {
                var p = new Position(RaceLapsList[i].Latitude, RaceLapsList[i].Longitude);
                polyline.Geopath.Insert(polyline.Geopath.Count, p);
            }
            CurrentMapView.MapElements.Add(polyline);
            CurrentMapView.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(lastRecord.Latitude, lastRecord.Longitude), Distance.FromMeters(CurrentMapZoom.Level)));
        }

        public void UpdateRadialMenuPosition(Point point)
        {
            RadialMenuPointPosition = new Point(point.X, point.Y);
            var o = new RadialMenuPosition
            {
                Id = 1,
                X = RadialMenuPointPosition.X,
                Y = RadialMenuPointPosition.Y
            };
            var ds = new DataStore();
            ds.SalvarRadialMenuPosition(o);
        }

        public async Task OpenRaceStatistics()
        {
            try
            {
                var ds = new DataStore();
                var race = ds.SalvarCorrida(CircuitoEscolhido, RaceLapsList);
                ListeningPosition.Clear();
                CurrentMapView.MapElements.Clear();
                CurrentMapView.Pins.Clear();
                RaceLapsList.Clear();
                HasRaceStatistics = RaceLapsList.Count > 0;
                LoadWayPoints();
                await Application.Current.MainPage.Navigation.PushPopupAsync(new PopupRaceListPage(race));
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
