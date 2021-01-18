using Realms;

namespace TRSTrack.Models
{
    public class RecordAdjust : RealmObject
    {
        [PrimaryKey]
        public int Id { get; set; }

        public int MinimumMeters { get; set; }

        public int MinimumVelocity { get; set; }
    }
}
