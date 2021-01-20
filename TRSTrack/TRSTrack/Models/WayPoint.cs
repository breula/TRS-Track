using Realms;

namespace TRSTrack.Models
{
    public class WayPoint : RealmObject
    {
        public WayPoint()
        {
            Cor = "#2196F3";
            Latitude = 0;
            Longitude = 0;
            Largada = false;
            Chegada = false;
            IsWayPoint = false;
            Distancia = 0;
        }

        [PrimaryKey]
        public int Id { get; set; }
        /// <summary>
        /// Id do circuito/pista em que o waypoint está vinculado
        /// </summary>
        public int Circuito { get; set; }
        /// <summary>
        /// Nome do waypiont
        /// </summary>
        public string Nome { get; set; }
        public string Cor { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public bool Largada { get; set; }

        public bool Chegada { get; set; }

        public bool IsWayPoint { get; set; }

        /// <summary>
        /// Distancia total do Circuito em Metros
        /// </summary>
        public double Distancia { get; set; }
    }
}
