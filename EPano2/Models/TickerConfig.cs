namespace EPano2.Models
{
    /// <summary>
    /// Alt kısımdaki kayan yazı için override ayarı.
    /// Admin buraya bir metin girerse, API'den gelen duyuru başlıkları yerine bu metin kırmızı şerit olarak akar.
    /// </summary>
    public class TickerConfig
    {
        public int Id { get; set; }  // Her zaman 1 kayıt

        /// <summary>
        /// Admin'in girdiği özel kayan yazı metni. Boş ya da null ise API'den gelen başlıklar kullanılacak.
        /// </summary>
        public string? CustomText { get; set; }

        public bool IsActive { get; set; } = true;
    }
}


