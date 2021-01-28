using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using TRSTrack.Custom;
using TRSTrack.Helpers;
using TRSTrack.Models;
using Xamarin.Forms.Maps;

namespace TRSTrack.Common
{
    public class CommonProperties
    {
        private static ObservableCollection<RaceLapTempItem> RaceLapList { get; set; }
        private static ObservableCollection<WayPoint> RaceMarkers { get; set; }
        public static bool KeepTrakingPosition { get; set; }
        private static DateTime JustPassedLargada { get; set; }
        private static DateTime JustPassedChegada { get; set; }
        private static DateTime JustPassedChegadaWaypoint { get; set; }
        private static int MarkLaplistCount { get; set; }
        private static Color CurrentLapColor { get; set; }
        private static CustomMap CurrentMapView { get; set; }
        private static int LapNumber { get; set; }

        public static void Start()
        {
            RaceLapList = new ObservableCollection<RaceLapTempItem>();
            RaceMarkers = new ObservableCollection<WayPoint>();
            KeepTrakingPosition = false;
            MarkLaplistCount = 0;
            JustPassedLargada = DateTime.Now;
            JustPassedChegada = JustPassedLargada;
            JustPassedChegadaWaypoint = JustPassedChegada;
        }

        public static int GetLapNumber()
        {
            return LapNumber;
        }

        public static Color GetCurrentLapColor()
        {
            return CurrentLapColor;
        }

        public static CustomMap GetCurrentMapView()
        {
            return CurrentMapView;
        }

        public static void RaceLapListAddItem(RaceLapTempItem raceLapTempItem)
        {
            RaceLapList.Add(raceLapTempItem);
        }
        public static ObservableCollection<RaceLapTempItem> RaceLapListData()
        {
            return RaceLapList;
        }
        public static void RaceLapListClear()
        {
            RaceLapList.Clear();
        }

        public static ObservableCollection<WayPoint> RaceMarkersData()
        {
            return RaceMarkers;
        }
        public static void RaceMarkersAddItem(WayPoint wayPoint)
        {
            RaceMarkers.Add(wayPoint);
        }
        public static void RaceMarkersClear()
        {
            RaceMarkers.Clear();
        }

        public static void UpdadeReceData()
        {
            if (!KeepTrakingPosition) return;

            var lastRecord = ListeningPosition.Last();
            if (lastRecord == null) return;

            if (RaceLapListData().Count > 0)
            {
                var lastPloting = RaceLapListData().Last();
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
                        MarkLaplistCount = RaceLapList.Count;
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
                    else if (DateTime.Now.Subtract(JustPassedChegadaWaypoint).TotalSeconds >= 30)
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

            RaceLapListAddItem(new RaceLapTempItem
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

            var data = RaceLapListData();
            for (int i = MarkLaplistCount; i < data.Count(); i++)
            {
                var p = new Position(data[i].Latitude, data[i].Longitude);
                polyline.Geopath.Insert(polyline.Geopath.Count, p);
            }
            CurrentMapView.MapElements.Add(polyline);
            CurrentMapView.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(lastRecord.Latitude, lastRecord.Longitude), Distance.FromMeters(50)));
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
}
