namespace EPano2.Models
{
    public class DashboardViewModel
    {
        public List<Video> Videos { get; set; } = new List<Video>();
        public List<Announcement> Announcements { get; set; } = new List<Announcement>();
        public Weather Weather { get; set; } = new Weather();
        public WeatherForecast WeatherForecast { get; set; } = new WeatherForecast();
        public List<string> ScrollingAnnouncements { get; set; } = new List<string>();
        public List<string> Credits { get; set; } = new List<string>();
    }
}
