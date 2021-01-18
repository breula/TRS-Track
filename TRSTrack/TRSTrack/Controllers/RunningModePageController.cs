﻿using Acr.UserDialogs;
using Plugin.Geolocator;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TRSTrack.Custom;
using TRSTrack.Models;
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

                Largada = WayPercurso.Where(p => p.Largada == true).ToList()[0];
                Chegada = WayPercurso.Where(p => p.Chegada == true).ToList()[0];

                var polyline = new Polyline
                {
                    StrokeColor = Color.DodgerBlue,
                    StrokeWidth = 18
                };
                polyline.Geopath.Clear();
                for (var i = 0; i < WayPercurso.Count; i++)
                {
                    var p = new Position(WayPercurso[i].Latitude, WayPercurso[i].Longitude);
                    polyline.Geopath.Insert(i, p);

                    if (WayPercurso[i].IsWayPont || WayPercurso[i].Largada || WayPercurso[i].Chegada)
                    {
                        var cp = new CustomPin
                        {
                            Label = string.IsNullOrEmpty(WayPercurso[i].Nome) ? $"Waypoint {WayPercurso[i].Id}" : WayPercurso[i].Nome,
                            Position = new Position(WayPercurso[i].Latitude, WayPercurso[i].Longitude),
                            Type = PinType.SavedPin
                        };
                        if (WayPercurso[i].Largada)
                        {
                            cp.MarkerId = "Largada";
                        }
                        else if (WayPercurso[i].Chegada)
                        {
                            cp.MarkerId = "Chegada";
                        }
                        else
                        {
                            cp.MarkerId = GetPinColor(WayPercurso[i].Cor).ColorValue;
                        };

                        CurrentMapView.Pins.Add(cp);
                        CurrentMapView.CustomPins.Add(cp);
                    }
                }

                CurrentMapView.MapElements.Add(polyline);
                CurrentMapView.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(Largada.Latitude, Largada.Longitude), Distance.FromMeters(CurrentMapZoom.Level)));
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
    }
}
