using System.Drawing;
using TRSTrack.Models.Enums;

namespace TRSTrack.Models
{
    /// <summary>
    /// Cores possíveis e aceitáveis pelo Maps para um marker
    /// </summary>
    public class PinColor
    {
        public Color Color { get; set; }
        public string ColorHexCode { get; set; }
        public BitmapDescriptorFactoryColor ColorName { get; set; }
        public float ColorValue { get; set; }
    }
}
