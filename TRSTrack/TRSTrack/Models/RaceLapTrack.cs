using Realms;

namespace TRSTrack.Models
{
    public class RaceLapTrack : RealmObject
    {
        public int Id { get; set; }
        public int Corrida { get; set; }
        public int LapNumber { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
