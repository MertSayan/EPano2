using EPano2.Models.Dtos;

namespace EPano2.Models
{
    public class DashboardViewModel
    {
        public Video Videos { get; set; } = new Video();
        /// <summary>
        /// Dashboard'da gösterilecek video dosya yolu listesi.
        /// Aktif videolar bu listede tutulur.
        /// </summary>
        public List<string> VideoFilePaths { get; set; } = new List<string>();
        /// <summary>
        /// Varsayılan video dosya yolu. Aktif video yoksa bu kullanılacak.
        /// </summary>
        public string? DefaultVideoFilePath { get; set; }
        public List<AnnouncementDto> Announcements { get; set; } = new List<AnnouncementDto>();
        public List<AnnouncementDto> News { get; set; } = new List<AnnouncementDto>();
        public List<AnnouncementDto> Etkinlikler { get; set; } = new List<AnnouncementDto>();
        public Weather Weather { get; set; } = new Weather();
        public WeatherForecast WeatherForecast { get; set; } = new WeatherForecast();
        public List<string> ScrollingAnnouncements { get; set; } = new List<string>();
        public List<string> Credits { get; set; } = new List<string>();
    }
}
