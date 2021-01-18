namespace TRSTrack.Models
{
    public class CircuitoShareWayPoint
    {
        public int Circuito { get; set; }
        public string Nome { get; set; }
        public string Cor { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool Largada { get; set; }
        public bool Chegada { get; set; }
        public bool IsWayPont { get; set; }
        public double Distancia { get; set; }
    }
}
