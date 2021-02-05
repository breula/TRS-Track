using Realms;
using System;

namespace TRSTrack.Models
{
    public class Race : RealmObject
    {
        public Race()
        {
            Data = new DateTimeOffset(DateTime.Now, TimeZoneInfo.Local.GetUtcOffset(DateTime.Now));
        }

        [PrimaryKey]
        public int Id { get; set; }
        public int Circuito { get; set; }
        public string Nome { get; set; }
        public string DisplayName { get; set; }
        public string Cpf { get; set; }
        public DateTimeOffset Data { get; set; }
    }
}
