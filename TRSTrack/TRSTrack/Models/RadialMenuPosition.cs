using Realms;

namespace TRSTrack.Models
{
    public class RadialMenuPosition : RealmObject
    {
        [PrimaryKey]
        public int Id { get; set; }

        /// <summary>
        /// Y position from Point
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// X position from Point
        /// </summary>
        public double X { get; set; }
    }
}
