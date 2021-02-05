using Realms;

namespace TRSTrack.Models
{
    public class RangeAdjust : RealmObject
    {
        [PrimaryKey]
        public int Id { get; set; }

        public int Range { get; set; }
    }
}
