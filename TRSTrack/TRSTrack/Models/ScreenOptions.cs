using Realms;

namespace TRSTrack.Models
{
    public class ScreenOptions : RealmObject
    {
        [PrimaryKey]
        public int Id { get; set; }
        public bool ShowVelocimeter { get; set; }
        public bool ShowMenuBotoes { get; set; }
        public bool ShowWayPointsList { get; set; }
        public int MapType { get; set; }
        public bool IsUnlocked { get; set; }
    }
}
