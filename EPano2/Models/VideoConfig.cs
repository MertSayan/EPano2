namespace EPano2.Models
{
    /// <summary>
    /// Admin'in video ayarlarını tuttuğumuz basit konfigürasyon tablosu.
    /// </summary>
    public class VideoConfig
    {
        public int Id { get; set; }  // Her zaman 1 kayıt olacak şekilde kullanacağız

        /// <summary>
        /// Admin hiçbir ekstra link eklemezse dönecek olan varsayılan YouTube video/linki.
        /// </summary>
        public string? DefaultVideoUrl { get; set; }

        /// <summary>
        /// Varsayılan video linki aktifse, ekstra linkler olsa bile sadece bu video dönsün.
        /// </summary>
        public bool IsDefaultActive { get; set; }

        /// <summary>
        /// Admin'in eklediği ekstra videolar (sıralı şekilde dönecek).
        /// </summary>
        public ICollection<ExtraVideoLink> ExtraVideoLinks { get; set; } = new List<ExtraVideoLink>();
    }
}


