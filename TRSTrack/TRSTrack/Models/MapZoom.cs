using Realms;

namespace TRSTrack.Models
{
    public class MapZoom : RealmObject
    {
        [PrimaryKey]
        public int Id { get; set; }
        public int Level { get; set; }
    }
}
