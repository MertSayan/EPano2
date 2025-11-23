using EPano2.Interfaces;
using EPano2.Models;
using Newtonsoft.Json;

namespace EPano2.Manager
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly HttpClient _http;

        public AnnouncementService(HttpClient http)
        {
            _http = http;
        }
        public async Task<List<Announcement>> GetAnnouncements()
        {
            string url = "https://api.isparta.edu.tr/isubu.asmx/DuyuruHaberListesi?haber=false&b64=false";
            var response = await _http.GetStringAsync(url);
            return JsonConvert.DeserializeObject<List<Announcement>>(response);
        }

        public async Task<List<Announcement>> GetNews()
        {
            string url = "https://api.isparta.edu.tr/isubu.asmx/DuyuruHaberListesi?haber=true&b64=false";
            var response = await _http.GetStringAsync(url);
            return JsonConvert.DeserializeObject<List<Announcement>>(response);
        }
    }
}
