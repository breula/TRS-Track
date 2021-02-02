using Acr.UserDialogs;
using Plugin.Geolocator;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private List<WayPoint> _raceMarkers;
        public List<WayPoint> RaceMarkers
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

        private int _lapNumner;
        public int LapNumber
        {
            get => _lapNumner;
            set
            {
                _lapNumner = value;
                OnPropertyChanged(nameof(LapNumber));
                CurrentLapColor = Tools.LapColor(LapNumber);
            }
        }

        private bool _enablaBurnButtom;
        public bool EnablaBurnButtom
        {
            get => _enablaBurnButtom;
            set
            {
                _enablaBurnButtom = value;
                OnPropertyChanged(nameof(EnablaBurnButtom));
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

                Task.Run(async () =>
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
                ListeningPosition.Set(CurrentMapView, CircuitoEscolhido);
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
            if (ListeningPosition.MarkersCount() == 0)
            {
                await MessageService.ShowAsync("Opa!", "Escolha um circuito primeiro por favor!");
                return;
            }

            if (ListeningPosition.CurrentLapNumber > 0)
            {
                var ask = await MessageService.ShowDialogAsync("Apagar tudo?", "Se der play novamente os dados da corrida atual serão apagados, deseja continuar?");
                if (!ask) return;
            }

            ListeningPosition.Stop();
            ListeningPosition.Clear();
            ListeningPosition.Reset();
            ListeningPosition.Play();
            LoadWayPoints();
            EnablaBurnButtom = ListeningPosition.MarkersCount() > 0 && KeepTrakingPosition == false;
            LapNumber = ListeningPosition.CurrentLapNumber;
            ChangeKeepTrakRule(true);

            TimeLapsed.Pause();
            TimeLapsed.Reset();
            TimeLapsed.Play();
            Tools.Vibrate(40);
        }

        private void StopRun()
        {
            Tools.Vibrate(40);
            ChangeKeepTrakRule(false);
            ListeningPosition.Stop();
            EnablaBurnButtom = ListeningPosition.MarkersCount() > 0 && KeepTrakingPosition == false;
        }

        private async void ChangeKeepTrakRule(bool keepTrack)
        {
            KeepTrakingPosition = keepTrack;
            IsRecordPaused = false;
            MessagingCenter.Send(KeepTrakingPosition == true ? "1": "0", "TRSTrackService");

            await Task.Run(async () =>
            {
                while (KeepTrakingPosition)
                {
                    CurrentChronometer = TimeLapsed.Display();
                    LapNumber = ListeningPosition.CurrentLapNumber;
                    var last = ListeningPosition.Last();
                    if (last != null)
                        CurrentMapView.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(last.Latitude, last.Longitude), new Distance(CurrentMapZoom.Level)));
                    await Task.Delay(100);
                }
            });
            CurrentChronometer = TimeLapsed.Display();
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
                SetBusyStatus(true, "Aguarde...");
                var laps = ListeningPosition.Laps();
                var ds = new DataStore();
                var race = ds.SalvarCorrida(CircuitoEscolhido, laps);
                ListeningPosition.Clear();
                CurrentMapView.MapElements.Clear();
                CurrentMapView.Pins.Clear();
                EnablaBurnButtom = ListeningPosition.MarkersCount() > 0 && KeepTrakingPosition == false;
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
