using Newtonsoft.Json;

namespace EPano2.Models
{
    public class Announcement
    {
        public int ID { get; set; }
        public int DuyuruHaberID { get; set; }
        public string Dil { get; set; }
        public string Baslik { get; set; }
        public string Icerik { get; set; }
        public string HaberResim { get; set; }
        public bool HaberMI { get; set; }
        public bool Onemli { get; set; }
        public string GecerlilikTarihi { get; set; }
        public string KayitTarihi { get; set; }
        public int OkunmaSayisi { get; set; }

    }
}
