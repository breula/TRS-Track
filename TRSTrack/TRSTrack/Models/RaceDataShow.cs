using System.Collections.Generic;

namespace TRSTrack.Models
{
    public class RaceDataShow
    {
        public string Circuito { get; set; }
        public string CircuitoCidade { get; set; }
        public double CircuitoDistancia { get; set; }
        public string Nome { get; set; }
        public string DisplayName { get; set; }
        public string Data { get; set; }

    }

    public class RaceLapDataShow
    {
        public RaceLapDataShow()
        {
            RaceLapPartialDataList = new List<RaceLapPartialDataShow>();
        }

        public int LapNumber { get; set; }
        public string TempoTotal { get; set; }
        public int VelocidadeMedia { get; set; }
        public List<RaceLapPartialDataShow> RaceLapPartialDataList { get; set; }
    }

    public class RaceLapPartialDataShow
    {
        public int VelocidadePassagem { get; set; }
        public string TempoPassagem { get; set; }
        public string DescricaoPassagem { get; set; }
    }
}
