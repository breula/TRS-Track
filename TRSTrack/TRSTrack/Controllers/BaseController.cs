using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
        public List<PinColor> AvailableColors { get; set; }

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
        #endregion

        #region-- Track Properties --
        public Thread ThreadChronometer = null;

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

        private ListeningPositionData _locationData;
        /// <summary>
        /// Current device location
        /// </summary>
        public ListeningPositionData LocationData
        {
            get => _locationData;
            set
            {
                _locationData = value;
                OnPropertyChanged(nameof(LocationData));
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
                ? new Rectangle(0f, 0.03f, 0.85f, 0.122f)
                : new Rectangle(0f, 0.03f, 0.85f, 0.14f);

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
                }
            };

            AvailableColors = new List<PinColor>
            {
                new PinColor
                {
                    Color = Color.FromHex("#FFFF00"),
                    ColorHexCode = "#FFFF00",
                    ColorName = BitmapDescriptorFactoryColor.HueYellow,
                    ColorValue = (float) 60.0
                },
                new PinColor
                {
                    Color = Color.FromHex("#7B00F7"),
                    ColorHexCode = "#7B00F7",
                    ColorName = BitmapDescriptorFactoryColor.HueViolet,
                    ColorValue = (float) 270.0
                },
                new PinColor
                {
                    Color = Color.FromHex("#FF007F"),
                    ColorHexCode = "#FF007F",
                    ColorName = BitmapDescriptorFactoryColor.HueRose,
                    ColorValue = (float) 330.0
                },
                new PinColor
                {
                    Color = Color.FromHex("#FF0000"),
                    ColorHexCode = "#FF0000",
                    ColorName = BitmapDescriptorFactoryColor.HueRed,
                    ColorValue = (float) 0.0
                },
                new PinColor
                {
                    Color = Color.FromHex("#FF4500"),
                    ColorHexCode = "#FF4500",
                    ColorName = BitmapDescriptorFactoryColor.HueOrange,
                    ColorValue = (float) 30.0
                },
                new PinColor
                {
                    Color = Color.FromHex("#FF00FF"),
                    ColorHexCode = "#FF00FF",
                    ColorName = BitmapDescriptorFactoryColor.HueMagenta,
                    ColorValue = (float) 300.0
                },
                new PinColor
                {
                    Color = Color.FromHex("#00ff00"),
                    ColorHexCode = "#00ff00",
                    ColorName = BitmapDescriptorFactoryColor.HueGreen,
                    ColorValue = (float) 120.0
                },
                new PinColor
                {
                    Color = Color.FromHex("#00ffff"),
                    ColorHexCode = "#00ffff",
                    ColorName = BitmapDescriptorFactoryColor.HueCyan,
                    ColorValue = (float) 180.0
                },
                new PinColor
                {
                    Color = Color.FromHex("#0080FF"),
                    ColorHexCode = "#0080FF",
                    ColorName = BitmapDescriptorFactoryColor.HueBlue,
                    ColorValue = (float) 240.0
                },
                new PinColor
                {
                    Color = Color.FromHex("#007FFF"),
                    ColorHexCode = "#007FFF",
                    ColorName = BitmapDescriptorFactoryColor.HueAzure,
                    ColorValue = (float) 210.0
                }
            };
            #endregion
        }

        public void UpdadeTrackData(CustomMap map)
        {
            if (!KeepTrakingPosition) return;

            LocationData = ListeningPosition.Last();
            if (LocationData == null) return;

            var lastRecord = Percurso.LastOrDefault();
            if (lastRecord == null)
            {
                Percurso.Add(new WayPoint
                {
                    Id = Percurso.Count + 1,
                    Latitude = LocationData.Latitude,
                    Longitude = LocationData.Longitude,
                    IsWayPoint = false
                });
                PercursoCount = Percurso.Count();
                return;
            }

            //Se está parado retorna
            if (Math.Abs(lastRecord.Latitude - LocationData.Latitude) <= 0 && Math.Abs(lastRecord.Longitude - LocationData.Longitude) <= 0)
            {
                return;
            }

            Velocidade = LocationData.Velocidade;

            var wp = new WayPoint
            {
                Id = Percurso.Count + 1,
                Latitude = LocationData.Latitude,
                Longitude = LocationData.Longitude,
                IsWayPoint = false
            };

            if (Percurso.Count >= 1)
            {
                wp.Distancia = Tools.GetDistance(
                    Percurso[Percurso.Count - 1].Latitude,
                    Percurso[Percurso.Count - 1].Longitude,
                    LocationData.Latitude,
                    LocationData.Longitude
                );
            }

            //Recupera Parametros de gravação
            var ds = new DataStore();
            var zoom = ds.GetCurrentMapZoom();
            var parametros = ds.GetRecordAdjust();

            IsRecordPaused = wp.Distancia < parametros.MinimumMeters || Velocidade < parametros.MinimumVelocity;
            if (IsRecordPaused) return;

            //Move posição apenas se estiver tudo ok
            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(LocationData.Latitude, LocationData.Longitude), Distance.FromMeters(zoom.Level)));

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
        /// Retorna cor do Pim
        /// </summary>
        /// <param name="colorHexCode">Código hex</param>
        /// <returns>PinColor</returns>
        public PinColor GetPinColor(string colorHexCode)
        {
            var pinColor = AvailableColors[0];
            switch (colorHexCode)
            {
                case "#7B00F7":
                    pinColor = AvailableColors[1];
                    break;
                case "#FF007F":
                    pinColor = AvailableColors[2];
                    break;
                case "#FF0000":
                    pinColor = AvailableColors[3];
                    break;
                case "#FF4500":
                    pinColor = AvailableColors[4];
                    break;
                case "#FF00FF":
                    pinColor = AvailableColors[5];
                    break;
                case "#00ff00":
                    pinColor = AvailableColors[6];
                    break;
                case "#00ffff":
                    pinColor = AvailableColors[7];
                    break;
                case "#0080FF":
                    pinColor = AvailableColors[8];
                    break;
                case "#007FFF":
                    pinColor = AvailableColors[9];
                    break;
            }

            return pinColor;
        }

        /// <summary>
        /// Retorna cor do Pim
        /// </summary>
        /// <param name="colorName">Nome da cor</param>
        /// <returns>PinColor</returns>
        public PinColor GetPinColor(BitmapDescriptorFactoryColor colorName)
        {
            var pinColor = AvailableColors[0];
            switch (colorName)
            {
                case BitmapDescriptorFactoryColor.HueViolet:
                    pinColor = AvailableColors[1];
                    break;
                case BitmapDescriptorFactoryColor.HueRose:
                    pinColor = AvailableColors[2];
                    break;
                case BitmapDescriptorFactoryColor.HueRed:
                    pinColor = AvailableColors[3];
                    break;
                case BitmapDescriptorFactoryColor.HueOrange:
                    pinColor = AvailableColors[4];
                    break;
                case BitmapDescriptorFactoryColor.HueMagenta:
                    pinColor = AvailableColors[5];
                    break;
                case BitmapDescriptorFactoryColor.HueGreen:
                    pinColor = AvailableColors[6];
                    break;
                case BitmapDescriptorFactoryColor.HueCyan:
                    pinColor = AvailableColors[7];
                    break;
                case BitmapDescriptorFactoryColor.HueBlue:
                    pinColor = AvailableColors[8];
                    break;
                case BitmapDescriptorFactoryColor.HueAzure:
                    pinColor = AvailableColors[9];
                    break;
            }

            return pinColor;
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
