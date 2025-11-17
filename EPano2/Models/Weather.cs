namespace EPano2.Models
{
    public class Weather
    {
        public string City { get; set; } = "Isparta";
        public int Temperature { get; set; }
        public string Condition { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; }
    }

    public class WeatherDay
    {
        public DateTime Date { get; set; }
        public int Temperature { get; set; }
        public string Condition { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string DayName { get; set; } = string.Empty;
        public bool IsCurrentDay { get; set; } = false;
        public bool IsPastDay { get; set; } = false;
        public bool IsFutureDay { get; set; } = false;
    }

    public class WeatherForecast
    {
        public string City { get; set; } = "Isparta";
        public List<WeatherDay> Days { get; set; } = new List<WeatherDay>();
        public DateTime LastUpdated { get; set; }
    }
}
