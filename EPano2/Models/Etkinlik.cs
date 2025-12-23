using Newtonsoft.Json;

namespace EPano2.Models
{
    public class Etkinlik
    {
        [JsonProperty("id")]
        public int ID { get; set; }
        
        [JsonProperty("adi")]
        public string Baslik { get; set; } = string.Empty;
        
        [JsonProperty("duyuruMetni")]
        public string Icerik { get; set; } = string.Empty;
        
        [JsonProperty("afisUrl")]
        public string Resim { get; set; } = string.Empty;
        
        [JsonProperty("tarih")]
        public string? BaslangicTarihi { get; set; }
        
        [JsonProperty("bitisTarihi")]
        public string? BitisTarihi { get; set; }
        
        [JsonProperty("kayitTarihi")]
        public string? KayitTarihi { get; set; }
    }
}

