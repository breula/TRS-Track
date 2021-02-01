using Acr.UserDialogs;
using Plugin.Geolocator;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TRSTrack.Custom;
using TRSTrack.Helpers;
using TRSTrack.Models;
using TRSTrack.Models.Enums;
using TRSTrack.Popups;
using TRSTrack.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace TRSTrack.Controllers
{
    public class MainPageController : BaseController
    {
        #region-- Properties --
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

        private bool _menuDesbloqueado = false;
        public bool MenuDesbloqueado
        {
            get => _menuDesbloqueado;
            set
            {
                _menuDesbloqueado = value;
                OnPropertyChanged(nameof(MenuDesbloqueado));
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

        private bool _canSaveCircuit;
        public bool CanSaveCircuit
        {
            get => _canSaveCircuit;
            set
            {
                _canSaveCircuit = value;
                OnPropertyChanged(nameof(CanSaveCircuit));
            }
        }

        private bool _canReset;
        public bool CanReset
        {
            get => _canReset;
            set
            {
                _canReset = value;
                OnPropertyChanged(nameof(CanReset));
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

        private ImageSource _imageAppIcon;
        public ImageSource ImageAppIcon
        {
            get => _imageAppIcon;
            set
            {
                _imageAppIcon = value;
                OnPropertyChanged(nameof(ImageAppIcon));
            }
        }

        private string _unlockScreen;
        public string UnlockScreen
        {
            get => _unlockScreen;
            set
            {
                _unlockScreen = value;
                OnPropertyChanged(nameof(UnlockScreen));
            }
        }
        #endregion

        #region-- PopupSalvarCircuito properties --
        private string _cidadeCircuito;
        public string CidadeCircuito
        {
            get => _cidadeCircuito;
            set
            {
                _cidadeCircuito = value;
                OnPropertyChanged(nameof(CidadeCircuito));
            }
        }

        private string _nomeCircuito;
        public string NomeCircuito
        {
            get => _nomeCircuito;
            set
            {
                _nomeCircuito = value;
                OnPropertyChanged(nameof(NomeCircuito));
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
        #endregion

        #region-- Commands --
        public Command StartRecordCommand
        {
            get { return new Command(obj => { StartRecord(); }); }
        }

        public Command StopRecordCommand
        {
            get { return new Command(obj => { StopRecord(); }); }
        }

        public Command SaveWayPointCommand
        {
            get { return new Command(obj => { SaveWayPoint(); }); }
        }

        public Command ResetRecordCommand
        {
            get { return new Command(async obj => { await ResetRecord(); }); }
        }

        public Command ShowHideVelocimeterCommand
        {
            get { return new Command(obj => { ShowHideVelocimeter(); }); }
        }

        public Command ShowHideMenuBotoesCommand
        {
            get { return new Command(obj => { ShowHideMenuBotoes(); }); }
        }

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
            get { return new Command(obj => { IncreaseMapZoom(); }); }
        }

        public Command DecreaseMapZoomCommand
        {
            get { return new Command(obj => { DecreaseMapZoom(); }); }
        }

        public Command OpenSaveCircuitDialogCommand
        {
            get { return new Command(async obj => { await OpenSaveCircuitDialog(); }); }
        }

        public Command ClosePopupCommand
        {
            get { return new Command(async obj => { await ClosePopup(); }); }
        }

        public Command SaveChosenColorCommand
        {
            get { return new Command<PinColor>(async obj => { await SaveChosenColor(obj); }); }
        }

        public Command OpenColorPickerDialogCommand
        {
            get { return new Command<WayPoint>(async obj => { await OpenColorPickerDialog(obj); }); }
        }

        public Command NavigateCircuitPageCommand
        {
            get { return new Command(async obj => { await NavigateCircuitPage(); }); }
        }

        public Command NavigateRacePageCommand
        {
            get { return new Command(async obj => { await NavigateRacePage(); }); }
        }

        public Command AjustRecordParamsCommand
        {
            get { return new Command(async obj => { await AjustRecordParams(); }); }
        }

        public Command OpenPopupImportarCircuitoPageCommand
        {
            get { return new Command(async obj => { await OpenPopupImportarCircuitoPage(); }); }
        }

        public Command OpenUnlockPopupOptionsCommand
        {
            get { return new Command(async obj => { await OpenUnlockPopupOptions(); }); }
        }

        public Command UnlockOptionsCommand
        {
            get { return new Command(async obj => { await UnlockOptions(); }); }
        }

        public Command OpenPopupRaceListPageCommand
        {
            get { return new Command(async obj => { await OpenPopupRaceListPage(); }); }
        }
        #endregion

        #region -- PopupSalvarCircuito commands --
        public Command SaveCircuitCommand
        {
            get { return new Command(async obj => { await SaveCircuito(); }); }
        }
        #endregion

        public MainPageController()
        {
            try
            {
                SetBusyStatus(true);

                ImageSaveCircuit = GetImageSource(MyImageEnum.SaveCircuit);
                ImageColorDialog = GetImageSource(MyImageEnum.ColorDialog);
                ImageAppIcon = GetImageSource(MyImageEnum.AppIcon);

                var ds = new DataStore();
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

                //MainThread.BeginInvokeOnMainThread(() => 
                //{
                //    Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                //    {
                //        CircuitCount = ds.CircuitosCount();
                //        RaceCount = ds.ReceCount();
                //        return true;
                //    });
                //});

                ResetParameters();
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

        //private async Task StartListening()
        //{
        //    await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromMilliseconds(500), 5, true, new Plugin.Geolocator.Abstractions.ListenerSettings
        //    {
        //        ActivityType = Plugin.Geolocator.Abstractions.ActivityType.AutomotiveNavigation,
        //        AllowBackgroundUpdates = true,
        //        DeferLocationUpdates = true,
        //        DeferralDistanceMeters = 5,
        //        DeferralTime = TimeSpan.FromMilliseconds(500),
        //        ListenForSignificantChanges = true,
        //        PauseLocationUpdatesAutomatically = false
        //    });

        //    CrossGeolocator.Current.PositionChanged += CurrentPositionChanged;
        //}

        //private void CurrentPositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        //{
        //    Device.BeginInvokeOnMainThread(() =>
        //    {
        //        if (!KeepTrakingPosition) return;

        //        var position = e.Position;
        //        CurrentLatitude = position.Latitude;
        //        CurrentLongitude = position.Longitude;
        //        CurrentMapView.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(CurrentLatitude, CurrentLongitude), Distance.FromMeters(PCurrentMapZoom)));
        //        Velocidade = (int)Math.Ceiling(position.Speed * 3.6); //Transformar em Km por hora;
        //        WayPointCount = WayPoints.Count();
        //        StepCount = Percurso.Count();
        //        DistanciaPercorrida = 0;
        //        foreach (var waypoint in Percurso)
        //        {
        //            DistanciaPercorrida += waypoint.Distancia;
        //        }
        //        foreach (var waypoint in WayPoints)
        //        {
        //            DistanciaPercorrida += waypoint.Distancia;
        //        }
        //        CurrentPosition.Latitude = CurrentLatitude;
        //        CurrentPosition.Longitude = CurrentLongitude;
        //    });
        //}

        private void ResetParameters()
        {
            PercursoCount = 0;
            WayPointCount = 0;
            Velocidade = 0;
            DistanciaPercorrida = 0;
            TimeLapsed.Pause();
            TimeLapsed.Reset();
            CurrentChronometer = TimeLapsed.Display();
            KeepTrakingPosition = false;
            CanSaveCircuit = false;
            CanReset = false;
            ListeningPosition.Clear();
            if (CurrentMapView != null)
            {
                CurrentMapView.MapElements.Clear();
                CurrentMapView.Pins.Clear();
                CurrentMapView.CustomPins.Clear();
            }
        }

        private void StartRecord()
        {
            Tools.Vibrate(40);
            if (TimeLapsed.Reseted())
            {
                TimeLapsed.Play();
                Percurso = new ObservableCollection<WayPoint>();
                WayPoints = new ObservableCollection<WayPoint>();
            }
            ChangeKeepTrakRule(true);
        }

        private void StopRecord()
        {
            Tools.Vibrate(40);
            ChangeKeepTrakRule(false);
        }

        private void SaveWayPoint()
        {
            Tools.Vibrate(40);

            var last = ListeningPosition.Last();
            var wp = new WayPoint
            {
                Id = WayPoints.Count + 1,
                Latitude = last.Latitude,
                Longitude = last.Longitude,
                IsWayPoint = true,
                Cor = Tools.GetPinColor(BitmapDescriptorFactoryColor.HueBlue).ColorHexCode
            };
            WayPoints.Add(wp);
            WayPointCount = WayPoints.Count;

            CurrentMapView.Pins.Add(new CustomPin
            {
                Label = string.IsNullOrEmpty(wp.Nome) ? $"Waypoint {wp.Id}" : wp.Nome,
                Position = new Position(wp.Latitude, wp.Longitude),
                Type = PinType.SavedPin,
                MarkerId = Tools.GetPinColor(wp.Cor).ColorValue
            });
        }

        private async Task ResetRecord()
        {
            Tools.Vibrate(new List<KeyValuePair<int, int>>
            {
                new KeyValuePair<int, int>(30,50),
                new KeyValuePair<int, int>(20,100)
            });

            if (PercursoCount > 0 || WayPointCount > 0)
            {
                var reset = await MessageService.ShowDialogAsync("Resetar percurso?", "Todos os waypoints gravados serão excluídos!");
                if (!reset) return;
            }
            
            ResetParameters();
        }

        private void ChangeKeepTrakRule(bool keepTrack)
        {
            KeepTrakingPosition = keepTrack;
            IsRecordPaused = false;
            MessagingCenter.Send(KeepTrakingPosition == true ? "1" : "0", "TRSTrackService");
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
            CanSaveCircuit = PercursoCount > 0 || WayPointCount > 0 && KeepTrakingPosition == false;
            CanReset = KeepTrakingPosition == false;
        }

        private void UpdateChronometer()
        {
            while (KeepTrakingPosition)
            {
                CurrentChronometer = TimeLapsed.Display();
                UpdadeTrackData(CurrentMapView);
                Thread.Sleep(1);
            }
        }

        public void ShowHideVelocimeter()
        {
            ScreenOptions.ShowVelocimeter = !ScreenOptions.ShowVelocimeter;
            SaveScreenOptions();
        }

        public void ShowHideMenuBotoes()
        {
            if (!ScreenOptions.IsUnlocked)
            {
                MessageService.Show("Bloqueado", "Necessário desbloquear menu primeiro!");
                return;
            }
            ScreenOptions.ShowMenuBotoes = !ScreenOptions.ShowMenuBotoes;
            SaveScreenOptions();
        }

        private void ShowWayPointsList()
        {
            if (!ScreenOptions.IsUnlocked)
            {
                MessageService.Show("Bloqueado", "Necessário desbloquear menu primeiro!");
                return;
            }
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

        private void DecreaseMapZoom()
        {
            try
            {
                if (CurrentMapZoom.Level >= 1000)
                {
                    UserDialogs.Instance.Toast("Zoom máximo de 1000 m", new TimeSpan(3));
                    return;
                };

                CurrentMapZoom.Level += 100;
                var ds = new DataStore();
                ds.SalvarCurrentMapZoom(new MapZoom { Id = CurrentMapZoom.Id, Level = CurrentMapZoom.Level });

                var last = ListeningPosition.Last();
                if (last != null)
                    CurrentMapView.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(last.Latitude, last.Longitude), Distance.FromMeters(CurrentMapZoom.Level)));
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

        private void IncreaseMapZoom()
        {
            try
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

                var last = ListeningPosition.Last();
                if (last != null)
                    CurrentMapView.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(last.Latitude, last.Longitude), Distance.FromMeters(CurrentMapZoom.Level)));
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

        private async Task ClosePopup()
        {
            await PopupNavigation.Instance.PopAsync();
        }

        public async Task OpenSaveCircuitDialog()
        {
            var last = ListeningPosition.Last();
            var local = await CrossGeolocator.Current.GetAddressesForPositionAsync(new Plugin.Geolocator.Abstractions.Position(last.Latitude, last.Longitude));
            var address = local.ToList();
            if (address.Count > 0)
                CidadeCircuito = address[0].SubAdminArea;

            await Application.Current.MainPage.Navigation.PushPopupAsync(new PopupSalvarCircuito(this));
        }

        public async Task SaveCircuito()
        {
            try
            {
                if (string.IsNullOrEmpty(NomeCircuito))
                {
                    await MessageService.ShowAsync("Falta dados", "É necessário definir um nome para o circuito!");
                    return;
                }

                //var ask = await MessageService.ShowDialogAsync("Confirma criação do Circuito?", $"{NomeCircuito} irá conter {PercursoCount} steps e {WayPointCount} waypoints.");
                //if (!ask) return;

                SetBusyStatus(true, "Aguarde...");

                var circuito = new Circuito
                {
                    Cidade = CidadeCircuito,
                    Nome = NomeCircuito,
                    Distancia = DistanciaPercorrida
                };
                var ds = new DataStore();
                circuito = ds.SalvarCircuito(circuito);

                var largada = Percurso.First();
                largada.Circuito = circuito.Id;
                largada.Largada = true;
                largada.Chegada = false;
                largada.Nome = string.IsNullOrEmpty(largada.Nome) ? "Largada" : largada.Nome;

                var chegada = Percurso.Last();
                chegada.Circuito = circuito.Id;
                chegada.Largada = false;
                chegada.Chegada = true;
                chegada.Nome = string.IsNullOrEmpty(largada.Nome) ? "Chegada" : largada.Nome;

                //Salva largada...
                ds.SalvarWayPoint(largada);

                //Salva percurso...
                foreach (var wayPoint in Percurso)
                {
                    if (wayPoint.Id == largada.Id || wayPoint.Id == chegada.Id) continue;
                    wayPoint.Circuito = circuito.Id;
                    wayPoint.Largada = false;
                    wayPoint.Chegada = false;
                    ds.SalvarWayPoint(wayPoint);
                }

                foreach (var wayPoint in WayPoints)
                {
                    wayPoint.Circuito = circuito.Id;
                    wayPoint.Largada = false;
                    wayPoint.Chegada = false;
                    ds.SalvarWayPoint(wayPoint);
                }

                //Salva chegada...
                ds.SalvarWayPoint(chegada);
                CircuitCount = ds.CircuitosCount();
                Percurso.Clear();
                WayPoints.Clear();
                ResetParameters();
                ChangeKeepTrakRule(false);
                await ClosePopup();
                await MessageService.ShowAsync("Sucesso", $"{circuito.Nome} salvo com sucesso!");
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

        private async Task SaveChosenColor(PinColor color)
        {
            WayPointForEdit.Cor = color.ColorHexCode;
            var refreshdList = new ObservableCollection<WayPoint>();
            foreach (var wayPoint in WayPoints)
            {
                refreshdList.Add(wayPoint);
                if (wayPoint.Id == WayPointForEdit.Id)
                {
                    wayPoint.Cor = WayPointForEdit.Cor;
                }
            }
            WayPoints.Clear();
            WayPoints = refreshdList;
            CurrentMapView.Pins.Clear();
            CurrentMapView.CustomPins.Clear();
            foreach (var wayPoint in WayPoints)
            {
                var cp = new CustomPin
                {
                    Label = string.IsNullOrEmpty(wayPoint.Nome) ? $"Waypoint {wayPoint.Id}" : wayPoint.Nome,
                    Position = new Position(wayPoint.Latitude, wayPoint.Longitude),
                    Type = PinType.SavedPin,
                    MarkerId = Tools.GetPinColor(wayPoint.Cor).ColorValue
                };
                CurrentMapView.Pins.Add(cp);
                CurrentMapView.CustomPins.Add(cp);
            }
            await ClosePopup();
        }

        private async Task OpenColorPickerDialog(WayPoint wayPoint)
        {
            await Application.Current.MainPage.Navigation.PushPopupAsync(new PopupColorDialog(this));
            WayPointForEdit = wayPoint;
        }

        private async Task NavigateCircuitPage()
        {
            Tools.Vibrate(40);
            var ds = new DataStore();
            CircuitCount = ds.CircuitosCount();
            if (CircuitCount == 0)
            {
                await MessageService.ShowAsync("Sem registros", "Não existem circuitos salvos ainda. Grave ou importe um antes.");
                return;
            }
            await Application.Current.MainPage.Navigation.PushPopupAsync(new PopupCircuitPage());
        }

        private async Task NavigateRacePage()
        {
            Tools.Vibrate(40);
            var ds = new DataStore();
            CircuitCount = ds.CircuitosCount();
            if (CircuitCount == 0)
            {
                await MessageService.ShowAsync("Sem registros", "Não existem circuitos salvos ainda. Grave ou importe um antes.");
                return;
            }
            await NavigationService.NavigateToRunningModePageOushPopupAsync();
        }

        private async Task AjustRecordParams()
        {
            await Application.Current.MainPage.Navigation.PushPopupAsync(new PopupAdjustRecordPage());
        }

        private async Task OpenPopupImportarCircuitoPage()
        {
            await Application.Current.MainPage.Navigation.PushPopupAsync(new PopupImportarCircuitoPage());
        }

        public async Task OpenUnlockPopupOptions()
        {
            if (ScreenOptions.IsUnlocked)
            {
                var ask = await MessageService.ShowDialogAsync("Já desbloqueado", "Opções já estão desbloqueadas, deseja bloquear novamente?.");
                if (!ask) return;
                ScreenOptions.IsUnlocked = false;
                ScreenOptions.ShowMenuBotoes = false;
                ScreenOptions.ShowWayPointsList = false;
                SaveScreenOptions();
                UserDialogs.Instance.Toast("Bloqueado!", new TimeSpan(3));
                return;
            }

            await Application.Current.MainPage.Navigation.PushPopupAsync(new PopupUnlockOptions(this));
        }

        private async Task UnlockOptions()
        {
            if (string.IsNullOrEmpty(UnlockScreen))
            {
                await MessageService.ShowAsync("Informe sua senha!", "Informe sua senha de desbloqueio.");
                return;
            }
            if (UnlockScreen != "10081982")
            {
                await MessageService.ShowAsync("Senha errada!", "Senha informada não é válida.");
                return;
            }
            ScreenOptions.IsUnlocked = true;
            SaveScreenOptions();
            await ClosePopup();
            await MessageService.ShowAsync("Sucesso", "Opções desbloqueadas.");
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

        private async Task OpenPopupRaceListPage()
        {
            var ds = new DataStore();
            RaceCount = ds.ReceCount();
            if (RaceCount == 0)
            {
                await MessageService.ShowAsync("Sem registros", "Não existem corridas gravadas ainda.");
                return;
            }
            await Application.Current.MainPage.Navigation.PushPopupAsync(new PopupRaceListPage(null));
        }
    }
}
