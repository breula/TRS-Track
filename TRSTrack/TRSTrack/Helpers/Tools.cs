using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using TRSTrack.Models;
using TRSTrack.Models.Enums;
using Xamarin.Essentials;

namespace TRSTrack.Helpers
{
    public static class Tools
    {
        #region-- Private Methods --
        private static readonly List<PinColor> AvailableColors = new List<PinColor>
            {
                new PinColor
                {
                    Color = ColorManager.FromHex("#FFFF00"),
                    ColorHexCode = "#FFFF00",
                    ColorName = BitmapDescriptorFactoryColor.HueYellow,
                    ColorValue = (float) 60.0
                },
                new PinColor
                {
                    Color = ColorManager.FromHex("#7B00F7"),
                    ColorHexCode = "#7B00F7",
                    ColorName = BitmapDescriptorFactoryColor.HueViolet,
                    ColorValue = (float) 270.0
                },
                new PinColor
                {
                    Color = ColorManager.FromHex("#FF007F"),
                    ColorHexCode = "#FF007F",
                    ColorName = BitmapDescriptorFactoryColor.HueRose,
                    ColorValue = (float)330.0
                },
                new PinColor
                {
                    Color = ColorManager.FromHex("#FF0000"),
                    ColorHexCode = "#FF0000",
                    ColorName = BitmapDescriptorFactoryColor.HueRed,
                    ColorValue = (float)0.0
                },
                new PinColor
                {
                    Color = ColorManager.FromHex("#FF4500"),
                    ColorHexCode = "#FF4500",
                    ColorName = BitmapDescriptorFactoryColor.HueOrange,
                    ColorValue = (float)30.0
                },
                new PinColor
                {
                    Color = ColorManager.FromHex("#FF00FF"),
                    ColorHexCode = "#FF00FF",
                    ColorName = BitmapDescriptorFactoryColor.HueMagenta,
                    ColorValue = (float)300.0
                },
                new PinColor
                {
                    Color = ColorManager.FromHex("#00ff00"),
                    ColorHexCode = "#00ff00",
                    ColorName = BitmapDescriptorFactoryColor.HueGreen,
                    ColorValue = (float)120.0
                },
                new PinColor
                {
                    Color = ColorManager.FromHex("#00ffff"),
                    ColorHexCode = "#00ffff",
                    ColorName = BitmapDescriptorFactoryColor.HueCyan,
                    ColorValue = (float)180.0
                },
                new PinColor
                {
                    Color = ColorManager.FromHex("#0080FF"),
                    ColorHexCode = "#0080FF",
                    ColorName = BitmapDescriptorFactoryColor.HueBlue,
                    ColorValue = (float)240.0
                },
                new PinColor
                {
                    Color = ColorManager.FromHex("#007FFF"),
                    ColorHexCode = "#007FFF",
                    ColorName = BitmapDescriptorFactoryColor.HueAzure,
                    ColorValue = (float)210.0
                }
            };
        
        private static bool ValidaCpf(string cpf)
        {
            var valor = cpf.Replace(".", "");
            valor = valor.Replace("-", "");

            if (valor.Length != 11)
                return false;

            var igual = true;
            for (var i = 1; i < 11 && igual; i++)
                if (valor[i] != valor[0])
                    igual = false;

            if (igual || valor == "12345678909")
                return false;

            var numeros = new int[11];
            for (var i = 0; i < 11; i++)
                numeros[i] = int.Parse(
                valor[i].ToString());

            var soma = 0;
            for (var i = 0; i < 9; i++)
                soma += (10 - i) * numeros[i];

            var resultado = soma % 11;
            if (resultado == 1 || resultado == 0)
            {
                if (numeros[9] != 0)
                    return false;
            }
            else if (numeros[9] != 11 - resultado)
                return false;

            soma = 0;
            for (var i = 0; i < 10; i++)
                soma += (11 - i) * numeros[i];

            resultado = soma % 11;

            if (resultado == 1 || resultado == 0)
            {
                if (numeros[10] != 0)
                    return false;
            }
            else
                if (numeros[10] != 11 - resultado)
                return false;
            return true;
        }

        private static bool ValidaCnpj(string cnpj)
        {
            cnpj = cnpj.Replace(".", "");
            cnpj = cnpj.Replace("/", "");
            cnpj = cnpj.Replace("-", "");

            const string ftmt = "6543298765432";
            var digitos = new int[14];
            var soma = new int[2];
            soma[0] = 0;
            soma[1] = 0;
            var resultado = new int[2];
            resultado[0] = 0;
            resultado[1] = 0;
            var cnpjOk = new bool[2];
            cnpjOk[0] = false;
            cnpjOk[1] = false;

            try
            {
                int nrDig;
                for (nrDig = 0; nrDig < 14; nrDig++)
                {
                    digitos[nrDig] = int.Parse(
                     cnpj.Substring(nrDig, 1));
                    if (nrDig <= 11)
                        soma[0] += digitos[nrDig] *
                                   int.Parse(ftmt.Substring(
                                       nrDig + 1, 1));
                    if (nrDig <= 12)
                        soma[1] += digitos[nrDig] *
                                   int.Parse(ftmt.Substring(
                                       nrDig, 1));
                }

                for (nrDig = 0; nrDig < 2; nrDig++)
                {
                    resultado[nrDig] = soma[nrDig] % 11;
                    if ((resultado[nrDig] == 0) || (resultado[nrDig] == 1))
                        cnpjOk[nrDig] = digitos[12 + nrDig] == 0;

                    else
                        cnpjOk[nrDig] = digitos[12 + nrDig] == 11 - resultado[nrDig];

                }
                return cnpjOk[0] && cnpjOk[1];
            }
            catch
            {
                return false;
            }
        }
        #endregion

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

        /// <summary>
        /// Retorna cor do Pim
        /// </summary>
        /// <param name="colorHexCode">Código hex</param>
        /// <returns>PinColor</returns>
        public static PinColor GetPinColor(string colorHexCode)
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
        public static PinColor GetPinColor(BitmapDescriptorFactoryColor colorName)
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

        public static Color LapColor(int lapNumber)
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

        public static string OnlyNumbers(string valor)
        {
            return string.IsNullOrEmpty(valor) ? valor : string.Join("", Regex.Split(valor, @"[^\d]"));
        }

        /// <summary>
        /// Valida se CPF ou CNPJ informados são válidos
        /// </summary>
        /// <param name="valor"></param>
        /// <returns></returns>
        public static bool ValidaCpfCnpj(string valor)
        {
            return OnlyNumbers(valor).Length <= 11 ? ValidaCpf(valor) : ValidaCnpj(valor);
        }
    }
}
