using Realms;

namespace TRSTrack.Models
{
    public class RaceLapPartial : RealmObject
    {
        public int Id { get; set; }
        public int Corrida { get; set; }
        public int NumeroPassagem { get; set; }
        public int VelocidadePassagem { get; set; }
        public string TempoPassagem{ get; set; }
        public string DescricaoPassagem { get; set; }
    }
}
