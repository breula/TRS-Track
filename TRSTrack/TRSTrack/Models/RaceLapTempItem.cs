namespace TRSTrack.Models
{
    /// <summary>
    /// Voltas dadas em uma corrida
    /// </summary>
    public class RaceLapTempItem
    {
        public RaceLapTempItem()
        {
            Chegada = false;
            Largada = false;
            TempoParcial = "00:00:00";
            Velocidade = 0;
            Latitude = 0;
            Longitude = 0;
        }

        public int LapNumber { get; set; }
        public string TempoParcial { get; set; }
        public int Velocidade { get; set; }
        public string CheckPoint { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool Chegada { get; set; }
        public bool Largada { get; set; }
    }
}
