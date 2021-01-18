using Realms;
using System;

namespace TRSTrack.Models
{
    public class Circuito : RealmObject
    {
        public Circuito()
        {
            Distancia = 0;
            Data = new DateTimeOffset(DateTime.Now, TimeZoneInfo.Local.GetUtcOffset(DateTime.Now));
        }

        [PrimaryKey]
        public int Id { get; set; }

        /// <summary>
        /// Home do Circuito
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Cidade em que o circuito está localizado
        /// </summary>
        public string Cidade { get; set; }

        /// <summary>
        /// Distancia total do Circuito em Metros
        /// </summary>
        public double Distancia { get; set; }

        /// <summary>
        /// Data e hora de gravação
        /// </summary>
        public DateTimeOffset Data { get; set; }
    }
}
