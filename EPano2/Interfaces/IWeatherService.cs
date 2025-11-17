namespace EPano2.Interfaces
{
    public interface IWeatherService
    {
        Task<List<DayDto>> Get7DayAsync();
    }

    public record DayDto(string IconUrl, int TempC);
}






