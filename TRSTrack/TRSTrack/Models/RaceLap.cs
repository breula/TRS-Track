using Realms;

namespace TRSTrack.Models
{
    public class RaceLap : RealmObject
    {
        [PrimaryKey]
        public int Id { get; set; }
        public int Race { get; set; }
        public int LapNumber { get; set; }
        public string TempoTotal { get; set; }
        public int VelocidadeMedia { get; set; }
    }
}
