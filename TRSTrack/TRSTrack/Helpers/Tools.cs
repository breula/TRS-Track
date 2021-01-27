using System;
using System.Collections.Generic;
using System.Threading;
using Xamarin.Essentials;

namespace TRSTrack.Helpers
{
    public static class Tools
    {
        /// <summary>
        /// Calcula distãncia entre um ponto geográfico e outro
        /// </summary>
        /// <param name="LatitudeOrigem">Latitude</param>
        /// <param name="LongitudeOrigem">Logitude</param>
        /// <param name="LatitudeDestino">Latitude</param>
        /// <param name="LongitudeDestino">Logitude</param>
        /// <returns>Distancia em metros</returns>
        public static double GetDistance(double LatitudeOrigem, double LongitudeOrigem, double LatitudeDestino, double LongitudeDestino)
        {
            var d1 = LatitudeOrigem * (Math.PI / 180.0);
            var num1 = LongitudeOrigem * (Math.PI / 180.0);
            var d2 = LatitudeDestino * (Math.PI / 180.0);
            var num2 = LongitudeDestino * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        }

        /// <summary>
        /// Vibra celular
        /// </summary>
        /// <param name="milesegundos">Tempo de vicração</param>
        public static void Vibrate(int milesegundos)
        {
            TimeSpan _vibrateDuration = TimeSpan.FromMilliseconds(milesegundos);
            Vibration.Vibrate(_vibrateDuration);
        }

        /// <summary>
        /// Vibra celular
        /// </summary>
        /// <param name="simphony">Paramentros de intervalos para toque personalizado</param>
        public static void Vibrate(List<KeyValuePair<int, int>> simphony)
        {
            foreach (var nota in simphony)
            {
                TimeSpan _vibrateDuration = TimeSpan.FromMilliseconds(nota.Value);
                Vibration.Vibrate(_vibrateDuration);
                Thread.Sleep(nota.Key);
            }
        }

        public static int StringTimeToMileSeconds(string tempo)
        {
            var hr = Convert.ToInt32(tempo.Split(':')[0]) * 3600000;
            var mi = Convert.ToInt32(tempo.Split(':')[1]) * 60000;
            var se = Convert.ToInt32(tempo.Split(':')[2]) * 1000;
            var ml = Convert.ToInt32(tempo.Split(':')[3]);
            return hr + mi + se + ml;
        } 
    }
}
