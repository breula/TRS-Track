using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using TRSTrack.Custom;
using TRSTrack.Helpers;
using TRSTrack.Interfaces;
using TRSTrack.Models;
using TRSTrack.Models.Enums;
using TRSTrack.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace TRSTrack.Controllers
{
    public class BaseController : INotifyPropertyChanged
    {
        #region-- Interfaces --
        public IMessageService MessageService => DependencyService.Get<IMessageService>();
        public IAppSettings AppSettingsService => DependencyService.Get<IAppSettings>();
        public INavigationService NavigationService => DependencyService.Get<INavigationService>();
        public ICloseApplication CloseApplication => DependencyService.Get<ICloseApplication>();
        public ILocSettings LocalizationSettings => DependencyService.Get<ILocSettings>();
        public INavigation Navigation => DependencyService.Get<INavigation>();
        #endregion

        #region-- Config App Properties --
        /// <summary>
        /// App name to show when necessary
        /// </summary>
        public string AppName = "TRS Track";

        private string _currentAppVersion;
        /// <summary>
        /// Versão atual do app
        /// </summary>
        public string CurrentAppVersion
        {
            get => _currentAppVersion;
            set
            {
                _currentAppVersion = value;
                OnPropertyChanged(nameof(CurrentAppVersion));
            }
        }

        private bool _isBusy;
        /// <summary>
        /// Set if app is busie or not
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        private string _busieMessage = "Aguarde";
        /// <summary>
        /// Message to display whe app is busie
        /// </summary>
        public string BusieMessage
        {
            get => _busieMessage;
            set
            {
                _busieMessage = value;
                OnPropertyChanged(nameof(BusieMessage));
            }
        }
        #endregion

        #region-- Map Properties --

        /// <summary>
        /// Imagens usadas n o app
        /// </summary>
        public List<MyImage> AppImages { get; set; }

        /// <summary>
        /// Cores possíveis para Pin
        /// </summary>
        //public List<PinColor> AvailableColors { get; set; }

        /// <summary>
        /// Ajusta retangulo do velocimetro de acordo com resulução do device
        /// </summary>
        private Rectangle _velocimeterBoud;
        public Rectangle VelocimeterBoud
        {
            get => _velocimeterBoud;
            set
            {
                _velocimeterBoud = value;
                OnPropertyChanged(nameof(VelocimeterBoud));
            }
        }

        /// <summary>
        /// Ajusta retangulo dos botoes de gravação de acordo com resulução do device
        /// </summary>
        private Rectangle _recordButtonsBoud;
        public Rectangle RecordButtonsBoud
        {
            get => _recordButtonsBoud;
            set
            {
                _recordButtonsBoud = value;
                OnPropertyChanged(nameof(RecordButtonsBoud));
            }
        }

        /// <summary>
        /// Ajusta retangulo da lista de circuitos na pagina de corrida
        /// </summary>
        private Rectangle _circuitListBoud;
        public Rectangle CircuitListBoud
        {
            get => _circuitListBoud;
            set
            {
                _circuitListBoud = value;
                OnPropertyChanged(nameof(CircuitListBoud));
            }
        }

        private Point _radialMenuPointPosition;
        public Point RadialMenuPointPosition
        {
            get => _radialMenuPointPosition;
            set
            {
                _radialMenuPointPosition = value;
                OnPropertyChanged(nameof(RadialMenuPointPosition));
            }
        }

        private MapZoom _currentMapZoom;
        /// <summary>
        /// Zoom do mapa
        /// </summary>
        public MapZoom CurrentMapZoom
        {
            get => _currentMapZoom;
            set
            {
                _currentMapZoom = value;
                OnPropertyChanged(nameof(CurrentMapZoom));
            }
        }

        private MapType _currentMapType;
        /// <summary>
        /// Tipo de Mapa
        /// </summary>
        public MapType CurrentMapType
        {
            get => _currentMapType;
            set
            {
                _currentMapType = value;
                OnPropertyChanged(nameof(CurrentMapType));
            }
        }


        private double _currentLatitude;
        public double CurrentLatitude
        {
            get => _currentLatitude;
            set
            {
                _currentLatitude = value;
                OnPropertyChanged(nameof(CurrentLatitude));
            }
        }

        private double _currentLongitude;
        public double CurrentLongitude
        {
            get => _currentLongitude;
            set
            {
                _currentLongitude = value;
                OnPropertyChanged(nameof(CurrentLongitude));
            }
        }

        #endregion

        #region-- Database properties --
        private int _circuitCount;
        /// <summary>
        /// Contator de Circuitos
        /// </summary>
        public int CircuitCount
        {
            get => _circuitCount;
            set
            {
                _circuitCount = value;
                OnPropertyChanged(nameof(CircuitCount));
            }
        }

        public int _raceCount;
        public int RaceCount
        {
            get => _raceCount;
            set
            {
                _raceCount = value;
                OnPropertyChanged(nameof(RaceCount));
            }
        }

        #endregion

        #region-- Track Properties --
        public Thread ThreadChronometer = null;
        public Thread ThreadRefreshMap = null;

        private ObservableCollection<WayPoint> _percurso;
        /// <summary>
        /// Coordenadas por ende passou durante percurso
        /// </summary>
        public ObservableCollection<WayPoint> Percurso
        {
            get => _percurso;
            set
            {
                _percurso = value;
                OnPropertyChanged(nameof(Percurso));
            }
        }

        private ObservableCollection<WayPoint> _wayPoints;
        /// <summary>
        /// Coordenadas dos way points salvos durante percurso
        /// </summary>
        public ObservableCollection<WayPoint> WayPoints
        {
            get => _wayPoints;
            set
            {
                _wayPoints = value;
                OnPropertyChanged(nameof(WayPoints));
            }
        }

        private int _percursoCount;
        /// <summary>
        /// Contator de posicioçoes geográficas coletadas dutante percurso
        /// </summary>
        public int PercursoCount
        {
            get => _percursoCount;
            set
            {
                _percursoCount = value;
                OnPropertyChanged(nameof(PercursoCount));
            }
        }

        private int _wayPointCount;
        /// <summary>
        /// Contator de waypoints durante percurso
        /// </summary>
        public int WayPointCount
        {
            get => _wayPointCount;
            set
            {
                _wayPointCount = value;
                OnPropertyChanged(nameof(WayPointCount));
            }
        }

        private bool _isRecordPaused = false;
        /// <summary>
        /// Muda status de track para pausado se parâmetros de gravação não forem atendidos
        /// </summary>
        public bool IsRecordPaused
        {
            get => _isRecordPaused;
            set
            {
                _isRecordPaused = value;
                OnPropertyChanged(nameof(IsRecordPaused));
            }
        }

        private double _distanciaPercorrida = 0;
        /// <summary>
        /// Distancia percorrida durante percurso
        /// </summary>
        public double DistanciaPercorrida
        {
            get => _distanciaPercorrida;
            set
            {
                _distanciaPercorrida = value;
                OnPropertyChanged(nameof(DistanciaPercorrida));
            }
        }

        private int _velocidade;
        /// <summary>
        /// Velocidade atual
        /// </summary>
        public int Velocidade
        {
            get => _velocidade;
            set
            {
                _velocidade = value;
                OnPropertyChanged(nameof(Velocidade));
            }
        }

        private bool _keepTrakingPosition;
        /// <summary>
        /// Mater rastreamento feito pelo background
        /// </summary>
        public bool KeepTrakingPosition
        {
            get => _keepTrakingPosition;
            set
            {
                _keepTrakingPosition = value;
                OnPropertyChanged(nameof(KeepTrakingPosition));
            }
        }

        private string _currentChronometer;
        /// <summary>
        /// Valor atual do tempo decorrido desde o inicio da gravação
        /// </summary>
        public string CurrentChronometer
        {
            get => _currentChronometer;
            set
            {
                _currentChronometer = value;
                OnPropertyChanged(nameof(CurrentChronometer));
            }
        }
        #endregion

        public BaseController()
        {
            #region-- Set App Properties --
            CurrentAppVersion = $"{AppInfo.VersionString}.{AppInfo.BuildString}";
            var assemblyName = typeof(BaseController).GetTypeInfo().Assembly.GetName();
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;

            //var dps = Math.Round((mainDisplayInfo.Width - 0.5f) / mainDisplayInfo.Density);
            VelocimeterBoud = mainDisplayInfo.Density <= 3
                ? mainDisplayInfo.Density <= 2.7
                    ? new Rectangle(0f, 0f, 1f, 0.15f)
                    : new Rectangle(0f, 0f, 1f, 0.17f)
                : new Rectangle(0f, 0f, 1f, 0.125f);

            RecordButtonsBoud = mainDisplayInfo.Density <= 3
                ? new Rectangle(0f, 0.93f, 1f, 0.12f)
                : new Rectangle(0f, 0.93f, 1f, 0.105f);

            CircuitListBoud = mainDisplayInfo.Density <= 3
                ? mainDisplayInfo.Density <= 2.7
                    ? new Rectangle(0f, 0.02f, 0.8f, 0.105f)
                    : new Rectangle(0f, 0.04f, 0.8f, 0.143f)
                : mainDisplayInfo.Density == 3
                    ? new Rectangle(0f, 0.04f, 0.85f, 0.14f)
                    : new Rectangle(0f, 0.04f, 0.85f, 0.105f);

            AppImages = new List<MyImage>
            {
                new MyImage
                {
                    Source = ImageSource.FromResource($"{assemblyName.Name}.Resources.save_circuit.jpg"),
                    ImageName = "SaveCircuit"
                },
                new MyImage
                {
                    Source = ImageSource.FromResource($"{assemblyName.Name}.Resources.color_dialog.jpg"),
                    ImageName = "ColorDialog"
                },
                new MyImage
                {
                    Source = ImageSource.FromResource($"{assemblyName.Name}.Resources.position.gif"),
                    ImageName = "PositionGif"
                },
                new MyImage
                {
                    Source = ImageSource.FromResource($"{assemblyName.Name}.Resources.icon.png"),
                    ImageName = "AppIcon"
                },
                new MyImage
                {
                    Source = ImageSource.FromResource($"{assemblyName.Name}.Resources.race_list.jpg"),
                    ImageName = "RaceList"
                }
            };
            #endregion
        }

        public void UpdadeTrackData(CustomMap map)
        {
            if (!KeepTrakingPosition) return;

            var last = ListeningPosition.Last();
            if (last == null) return;

            CurrentLatitude = last.Latitude;
            CurrentLongitude = last.Longitude;

            var lastRecord = Percurso.LastOrDefault();
            if (lastRecord == null)
            {
                Percurso.Add(new WayPoint
                {
                    Id = Percurso.Count + 1,
                    Latitude = last.Latitude,
                    Longitude = last.Longitude,
                    IsWayPoint = false
                });
                PercursoCount = Percurso.Count();
                return;
            }

            //Se está parado retorna
            if (Math.Abs(lastRecord.Latitude - last.Latitude) <= 0 && Math.Abs(lastRecord.Longitude - last.Longitude) <= 0)
            {
                return;
            }

            Velocidade = last.Velocidade;

            var wp = new WayPoint
            {
                Id = Percurso.Count + 1,
                Latitude = last.Latitude,
                Longitude = last.Longitude,
                IsWayPoint = false
            };

            if (Percurso.Count >= 1)
            {
                wp.Distancia = Tools.GetDistance(
                    Percurso[Percurso.Count - 1].Latitude,
                    Percurso[Percurso.Count - 1].Longitude,
                    last.Latitude,
                    last.Longitude
                );
            }

            //Recupera Parametros de gravação
            var ds = new DataStore();
            var zoom = ds.GetCurrentMapZoom();
            var parametros = ds.GetRecordAdjust();

            IsRecordPaused = wp.Distancia < parametros.MinimumMeters || Velocidade < parametros.MinimumVelocity;
            if (IsRecordPaused) return;

            //Move posição apenas se estiver tudo ok
            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(last.Latitude, last.Longitude), Distance.FromMeters(zoom.Level)));

            DistanciaPercorrida += wp.Distancia;
            Percurso.Add(wp);
            PercursoCount = Percurso.Count();

            var polyline = new Polyline
            {
                StrokeColor = Color.DodgerBlue,
                StrokeWidth = 18
            };

            polyline.Geopath.Clear();
            for (var i = 0; i < Percurso.Count; i++)
            {
                polyline.Geopath.Insert(i, new Position(Percurso[i].Latitude, Percurso[i].Longitude));
            }
            map.MapElements.Add(polyline);
        }

        /// <summary>
        /// Recupera dados do resource para utilização de imagens no app
        /// </summary>
        /// <param name="myImageEnum">Imagem a ser utilizada</param>
        /// <returns>ImageSource</returns>
        public ImageSource GetImageSource(MyImageEnum myImageEnum)
        {
            ImageSource source = null;
            switch (myImageEnum)
            {
                case MyImageEnum.SaveCircuit:
                    foreach (var img in AppImages)
                    {
                        if (img.ImageName != "SaveCircuit") continue;
                        source = img.Source;
                        break;
                    }
                    break;
                case MyImageEnum.ColorDialog:
                    foreach (var img in AppImages)
                    {
                        if (img.ImageName != "ColorDialog") continue;
                        source = img.Source;
                        break;
                    }
                    break;
                case MyImageEnum.PositionGif:
                    foreach (var img in AppImages)
                    {
                        if (img.ImageName != "PositionGif") continue;
                        source = img.Source;
                        break;
                    }
                    break;
                case MyImageEnum.AppIcon:
                    foreach (var img in AppImages)
                    {
                        if (img.ImageName != "AppIcon") continue;
                        source = img.Source;
                        break;
                    }
                    break;
                case MyImageEnum.RaceList:
                    foreach (var img in AppImages)
                    {
                        if (img.ImageName != "RaceList") continue;
                        source = img.Source;
                        break;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(myImageEnum), myImageEnum, null);
            }
            return source;
        }

        /// <summary>
        /// Change app status
        /// </summary>
        /// <param name="isBusy">If busie or not</param>
        /// <param name="message">Busie message if desire</param>
        public void SetBusyStatus(bool isBusy, string message = null)
        {
            IsBusy = isBusy;
            BusieMessage = message;
        }

        /// <summary>
        /// Revo histórico de navegação
        /// </summary>
        /// <param name="target">Página a ser removida. Se for null remove tudo</param>
        public void ResetNavigationStack(Page target = null)
        {
            if (Navigation != null && Navigation.NavigationStack.Count > 0)
            {
                var existingPages = Navigation.NavigationStack.ToList();
                foreach (var page in existingPages)
                {
                    if (target != null)
                    {
                        if (target.Id == page.Id)
                        {
                            Navigation.RemovePage(page);
                            break;
                        }
                    }
                    else
                    {
                        Navigation.RemovePage(page);
                    }
                }
            }
        }

        #region -- Bind Properties (Never remove this code) --
        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            changed?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
