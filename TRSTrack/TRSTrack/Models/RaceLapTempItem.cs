namespace TRSTrack.Models
{
    /// <summary>
    /// Voltas dadas em uma corrida
    /// </summary>
    public class RaceLapTempItem
    {
        public int LapNumber { get; set; }
        public string TempoParcial { get; set; }
        public int VelocidadeMedia { get; set; }
        public string CheckPoint { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
