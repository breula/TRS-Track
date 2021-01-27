using System.Collections.Generic;
using System.Linq;

namespace TRSTrack.Models
{
    /// <summary>
    /// Classe para guardar dados de posicionamento de acordo com StartListeningAsync
    /// </summary>
    public static class ListeningPosition
    {
        private static readonly List<ListeningPositionData> DataList = new List<ListeningPositionData>();

        public static void Add(int velocidade, double Latitude, double longitude)
        {
            //Para não acabar com memória do celular, deixar apenas ultimas 5 leituras de geolocalização
            if (DataList.Count >= 10)
            {
                DataList.RemoveRange(0, DataList.Count-5);
            }

            DataList.Add(new ListeningPositionData
            {
                Velocidade = velocidade,
                Latitude = Latitude,
                Longitude = longitude
            });
        }

        public static ListeningPositionData Last()
        {
            return DataList.LastOrDefault();
        }

        public static void Clear()
        {
            DataList.Clear();
        }
    }

    public class ListeningPositionData
    {
        public int Id { get; set; }
        public int Velocidade { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
