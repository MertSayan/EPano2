using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using EPano2.Models.Options;
using EPano2.Interfaces;

namespace EPano2.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _http;
        private readonly OpenWeatherOptions _opt;
        private readonly IMemoryCache _cache;

        public WeatherService(HttpClient http, IOptions<OpenWeatherOptions> opt, IMemoryCache cache)
        {
            _http = http;
            _opt = opt.Value;
            _cache = cache;
        }

        // Kept signature for compatibility, but returns *one* day (today)
        public async Task<List<DayDto>> Get7DayAsync()
        {
            if (_cache.TryGetValue("ow_today", out List<DayDto>? cached) && cached is not null)
                return cached;

            var url = $"https://api.openweathermap.org/data/2.5/weather?lat={_opt.Lat}&lon={_opt.Lon}&units={_opt.Units}&lang={_opt.Lang}&appid={_opt.ApiKey}";
            var data = await _http.GetFromJsonAsync<WeatherResponse>(url);
            if (data is null || data.weather is null || data.weather.Count == 0)
                return new();

            var icon = data.weather[0].icon;                       // e.g. 03n
            var temp = (int)Math.Round(data.main.temp);            // °C
            var iconUrl = $"https://openweathermap.org/img/wn/{icon}@2x.png";

            var result = new List<DayDto> { new(iconUrl, temp) };
            _cache.Set("ow_today", result, TimeSpan.FromMinutes(15));
            return result;
        }

        // --- minimal DTOs for /data/2.5/weather ---
        private class WeatherResponse
        {
            public List<W> weather { get; set; } = new();
            public Main main { get; set; } = new();
        }
        private class W { public string icon { get; set; } = ""; }
        private class Main { public double temp { get; set; } }
    }
}