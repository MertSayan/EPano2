using EPano2.Interfaces;
using EPano2.Models;
using Newtonsoft.Json;
using System.Globalization;

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

        public async Task<List<Etkinlik>> GetEtkinlikler()
        {
            // Bugünün tarihini dinamik olarak al
            var bugun = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            string url = $"https://etkinlik.isparta.edu.tr/api/services/app/EtkinlikAnonymous/GetEtkinlikler?BaslangicTarihi={bugun}&SkipCount=0&MaxResultCount=10";
            
            try
            {
                var response = await _http.GetStringAsync(url);
                
                // API'den dönen format muhtemelen bir result wrapper içinde olabilir (ASP.NET Boilerplate formatı)
                // Önce result wrapper'ı kontrol et
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(response);
                
                // Eğer result bir wrapper ise (result.result.items veya result.items)
                if (jsonObject?.result != null)
                {
                    var resultObj = jsonObject.result;
                    if (resultObj?.items != null)
                    {
                        return JsonConvert.DeserializeObject<List<Etkinlik>>(resultObj.items.ToString()) ?? new List<Etkinlik>();
                    }
                    // Eğer result direkt bir liste ise
                    if (resultObj is Newtonsoft.Json.Linq.JArray)
                    {
                        return JsonConvert.DeserializeObject<List<Etkinlik>>(resultObj.ToString()) ?? new List<Etkinlik>();
                    }
                }
                
                // Eğer result.items direkt erişilebilirse
                if (jsonObject?.items != null)
                {
                    return JsonConvert.DeserializeObject<List<Etkinlik>>(jsonObject.items.ToString()) ?? new List<Etkinlik>();
                }
                
                // Eğer direkt liste ise
                if (jsonObject is Newtonsoft.Json.Linq.JArray)
                {
                    return JsonConvert.DeserializeObject<List<Etkinlik>>(response) ?? new List<Etkinlik>();
                }
                
                // Son çare olarak direkt deserialize et
                return JsonConvert.DeserializeObject<List<Etkinlik>>(response) ?? new List<Etkinlik>();
            }
            catch (Exception ex)
            {
                // Log error if needed, but return empty list to prevent breaking the app
                System.Diagnostics.Debug.WriteLine($"Error fetching etkinlikler: {ex.Message}");
                return new List<Etkinlik>();
            }
        }
    }
}
