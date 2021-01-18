using System;
using System.Collections.Generic;

namespace TRSTrack.Models
{
    public class CircuitoShare
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public CircuitoShare()
        {
            WayPoints = new List<CircuitoShareWayPoint>();
        }

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
        public DateTime Data { get; set; }

        /// <summary>
        /// Waypoints relacionados ao circuito
        /// </summary>
        public List<CircuitoShareWayPoint> WayPoints { get; set; }
    }
}
