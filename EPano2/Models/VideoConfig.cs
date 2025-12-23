namespace EPano2.Models
{
    /// <summary>
    /// Admin'in video ayarlarını tuttuğumuz basit konfigürasyon tablosu.
    /// </summary>
    public class VideoConfig
    {
        public int Id { get; set; }  // Her zaman 1 kayıt olacak şekilde kullanacağız

        /// <summary>
        /// Varsayılan video dosyasının yolu (örn: /videos/default.mp4).
        /// Aktif video yoksa bu video sürekli oynatılacak.
        /// </summary>
        public string? DefaultVideoFilePath { get; set; }

        /// <summary>
        /// Admin'in eklediği ekstra videolar (sıralı şekilde dönecek).
        /// </summary>
        public ICollection<ExtraVideoLink> ExtraVideoLinks { get; set; } = new List<ExtraVideoLink>();
    }
}


