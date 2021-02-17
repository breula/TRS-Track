using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using TRSTrack.Custom;
using TRSTrack.Helpers;
using TRSTrack.Services;
using Xamarin.Forms.Maps;

namespace TRSTrack.Models
{
    /// <summary>
    /// Classe para guardar dados de posicionamento de acordo com StartListeningAsync
    /// </summary>
    public static class ListeningPosition
    {
        private static readonly List<ListeningPositionData> DataList = new List<ListeningPositionData>();
        private static readonly List<RaceLapTempItem> RaceLapsList = new List<RaceLapTempItem>();
        private static List<RaceMarker> RaceMarkers = new List<RaceMarker>();
        private static CustomMap Map = new CustomMap();
        private static bool Stoped = true;
        private static int MarkLaplistCount = 0;
        private static DateTime JustPassedLargada = DateTime.MinValue;
        private static DateTime JustPassedChegada = DateTime.MinValue;
        private static DateTime JustPassedWaypoint = DateTime.MinValue;
        private static Color CurrentLapColor;
        private static int RaceAdjustValue = 5;

        public static int CurrentLapNumber = 0;

        public static void Add(int velocidade, double Latitude, double longitude)
        {
            //Para não acabar com memória do celular, deixar apenas ultimas 5 leituras de geolocalização
            //if (DataList.Count >= 10)
            //{
            //    DataList.RemoveRange(0, DataList.Count - 5);
            //}

            DataList.Add(new ListeningPositionData
            {
                Velocidade = velocidade,
                Latitude = Latitude,
                Longitude = longitude,
                LapNumber = CurrentLapNumber
            });

            if (Stoped)
            {
                System.Diagnostics.Debug.WriteLine("Serviço Parado");
                return;
            }
            System.Diagnostics.Debug.WriteLine("Serviço Rodando!");
            UpdadeReceData();
        }

        public static ListeningPositionData Last()
        {
            return DataList.LastOrDefault();
        }

        public static void Clear()
        {
            DataList.Clear();
            RaceLapsList.Clear();
            RaceMarkers.Clear();
            Map.MapElements.Clear();
        }

        public static void Stop()
        {
            Stoped = true;
        }

        public static void Set(CustomMap map, Circuito circuito)
        {
            Map = map;

            var ds = new DataStore();
            var wayPercurso = ds.GetWayPoint(circuito);
            RaceMarkers = new List<RaceMarker>();

            var largada = wayPercurso.Where(p => p.Largada == true).ToList()[0];
            var polyline = new Polyline
            {
                StrokeColor = Color.DodgerBlue,
                StrokeWidth = 20
            };

            polyline.Geopath.Clear();
            var jump = 0;
            var waypointCount = 1;
            for (var i = 0; i < wayPercurso.Count; i++)
            {
                if (!wayPercurso[i].IsWayPoint)
                {
                    var p = new Position(wayPercurso[i].Latitude, wayPercurso[i].Longitude);
                    polyline.Geopath.Insert(i - jump, p);
                }

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
                    RaceMarkers.Add(new RaceMarker 
                    { 
                        Id = wayPercurso[i].Id,
                        Circuito = wayPercurso[i].Circuito,
                        Nome = wayPercurso[i].Nome,
                        Cor = wayPercurso[i].Cor,
                        Distancia = wayPercurso[i].Distancia,
                        Largada = wayPercurso[i].Largada,
                        Chegada = wayPercurso[i].Chegada,
                        IsWayPoint = wayPercurso[i].IsWayPoint,
                        Latitude = wayPercurso[i].Latitude,
                        Longitude = wayPercurso[i].Longitude
                    });
                    Map.Pins.Add(cp);
                    Map.CustomPins.Add(cp);
                }
                else if (wayPercurso[i].Chegada == true)
                {
                    cp.MarkerId = "Chegada";
                    cp.Name = "Chegada";
                    cp.Label = "Chegada";
                    RaceMarkers.Add(new RaceMarker
                    {
                        Id = wayPercurso[i].Id,
                        Circuito = wayPercurso[i].Circuito,
                        Nome = wayPercurso[i].Nome,
                        Cor = wayPercurso[i].Cor,
                        Distancia = wayPercurso[i].Distancia,
                        Largada = wayPercurso[i].Largada,
                        Chegada = wayPercurso[i].Chegada,
                        IsWayPoint = wayPercurso[i].IsWayPoint,
                        Latitude = wayPercurso[i].Latitude,
                        Longitude = wayPercurso[i].Longitude
                    });
                    Map.Pins.Add(cp);
                    Map.CustomPins.Add(cp);
                }
                else if (wayPercurso[i].IsWayPoint == true)
                {
                    jump++;
                    cp.MarkerId = Tools.GetPinColor(wayPercurso[i].Cor).ColorValue;
                    cp.Name = string.IsNullOrEmpty(wayPercurso[i].Nome) ? $"Waypoint {waypointCount}" : wayPercurso[i].Nome;
                    cp.Label = string.IsNullOrEmpty(wayPercurso[i].Nome) ? $"Waypoint {waypointCount}" : wayPercurso[i].Nome;
                    waypointCount++;
                    RaceMarkers.Add(new RaceMarker
                    {
                        Id = wayPercurso[i].Id,
                        Circuito = wayPercurso[i].Circuito,
                        Nome = wayPercurso[i].Nome,
                        Cor = wayPercurso[i].Cor,
                        Distancia = wayPercurso[i].Distancia,
                        Largada = wayPercurso[i].Largada,
                        Chegada = wayPercurso[i].Chegada,
                        IsWayPoint = wayPercurso[i].IsWayPoint,
                        Latitude = wayPercurso[i].Latitude,
                        Longitude = wayPercurso[i].Longitude
                    });
                    Map.Pins.Add(cp);
                    Map.CustomPins.Add(cp);
                }
            }
            Map.MapElements.Add(polyline);
        }

        public static void Reset()
        {
            MarkLaplistCount = 0;
            CurrentLapNumber = 0;
            JustPassedLargada = DateTime.MinValue;
            JustPassedChegada = DateTime.MinValue;
            JustPassedWaypoint = DateTime.MinValue;
        }

        public static void Play()
        {
            Stoped = false;
            var ds = new DataStore();
            RaceAdjustValue = ds.RangeAdjust().Range;
        }

        public static ObservableCollection<RaceLapTempItem> Laps()
        {
            return new ObservableCollection<RaceLapTempItem>(RaceLapsList);
        }

        public static int MarkersCount()
        {
            return RaceMarkers.Count;
        }

        public static int DataListCount()
        {
            return DataList.Count;
        }

        public static CustomMap MapListening()
        {
            return Map;
        }

        private static void UpdadeReceData()
        {
            var lastRecord = Last();
            if (lastRecord == null) return;

            if (RaceLapsList.Count > 0)
            {
                var lastPloting = RaceLapsList.Last();
                ///Esta parado?
                if (lastPloting.Latitude == lastRecord.Latitude && lastPloting.Longitude == lastRecord.Longitude)
                {
                    System.Diagnostics.Debug.WriteLine("Posição geográfica inalterada!");
                    return;
                }
            }

            //Verifica se está em um ponto do percurso(raio de 5 metros)
            string wayPointName = null;
            var isChegada = false;
            var isLargada = false;

            foreach (var ponto in RaceMarkers)
            {
                var distance = Tools.GetDistance(lastRecord.Latitude, lastRecord.Longitude, ponto.Latitude, ponto.Longitude);

                System.Diagnostics.Debug.WriteLine(distance.ToString());

                if (distance >= RaceAdjustValue) continue;

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
                        CurrentLapNumber++;
                        CurrentLapColor = LapColor(CurrentLapNumber);

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
                        CurrentLapNumber++;
                        JustPassedLargada = DateTime.Now;
                        CurrentLapColor = LapColor(CurrentLapNumber);
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
                    if (JustPassedWaypoint == DateTime.MinValue)
                    {
                        JustPassedWaypoint = DateTime.Now;
                        Tools.Vibrate(100);
                        UserDialogs.Instance.Toast($"Passou por {ponto.Nome}, raio de {distance:N3}m", new TimeSpan(3));
                    }
                    else if (DateTime.Now.Subtract(JustPassedWaypoint).TotalSeconds >= 30)
                    {
                        Tools.Vibrate(100);
                        UserDialogs.Instance.Toast($"Passou por {ponto.Nome}, raio de {distance:N3}m", new TimeSpan(3));
                        JustPassedWaypoint = DateTime.Now;
                    }
                    wayPointName = ponto.Nome;
                }

                break;
            };

            if (CurrentLapNumber == 0) return;

            RaceLapsList.Add(new RaceLapTempItem
            {
                TempoParcial = TimeLapsed.Display(),
                Velocidade = lastRecord.Velocidade,
                CheckPoint = wayPointName,
                LapNumber = CurrentLapNumber,
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
            Map.MapElements.Add(polyline);
            //Map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(lastRecord.Latitude, lastRecord.Longitude), Distance.FromMeters(CurrentMapZoom.Level)));
        }

        private static Color LapColor(int lapNumber)
        {
            switch (lapNumber)
            {
                case 1: return Color.IndianRed;
                case 2: return Color.GreenYellow;
                case 3: return Color.Black;
                case 4: return Color.DeepPink;
                case 5: return Color.Firebrick;
                case 6: return Color.Turquoise;
                case 7: return Color.Brown;
                case 8: return Color.Silver;
                case 9: return Color.PapayaWhip;
                case 10: return Color.Gold;
                default:
                    return Color.Aqua;
            }
        }
    }

    public class ListeningPositionData
    {
        public int Id { get; set; }
        public int Velocidade { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        /// <summary>
        /// Ultimo ID usado para corrida ou gravação
        /// </summary>
        public int? LapNumber { get; set; }
    }
}
