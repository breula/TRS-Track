using System;
using System.Collections.Generic;

namespace TRSTrack.Models
{
    /// <summary>
    /// Classe para exportação e importação de dados
    /// </summary>
    public class RaceShare
    {
        public RaceShare()
        {
            RaceShareLapList = new List<RaceShareLap>();
            RaceShareLapPartialList = new List<RaceShareLapPartial>();
            RaceShareLapTrackList = new List<RaceShareLapTrack>();
        }

        public string Nome { get; set; }
        public string DisplayName { get; set; }
        public string Cpf { get; set; }
        public DateTimeOffset Data { get; set; }
        public List<RaceShareLap> RaceShareLapList { get; set; }
        public List<RaceShareLapPartial> RaceShareLapPartialList { get; set; }
        public List<RaceShareLapTrack> RaceShareLapTrackList { get; set; }
    }

    public class RaceShareLap
    {
        public int LapNumber { get; set; }
        public string TempoTotal { get; set; }
        public int VelocidadeMedia { get; set; }
    }

    public class RaceShareLapPartial
    {
        public int NumeroPassagem { get; set; }
        public int VelocidadePassagem { get; set; }
        public string TempoPassagem { get; set; }
        public string DescricaoPassagem { get; set; }
    }

    public class RaceShareLapTrack
    {
        public int LapNumber { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
