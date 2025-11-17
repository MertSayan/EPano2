namespace EPano2.Models.Options
{
    public class OpenWeatherOptions
    {
        public string ApiKey { get; set; } = "";
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string Units { get; set; } = "metric";
        public string Lang { get; set; } = "tr";
    }
}






