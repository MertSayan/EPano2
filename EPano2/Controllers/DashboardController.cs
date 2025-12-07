using EPano2.Data;
using EPano2.Interfaces;
using EPano2.Models;
using EPano2.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace EPano2.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IAnnouncementService _announcementService;
        private readonly ApplicationDbContext _dbContext;

        public DashboardController(IAnnouncementService announcementService, ApplicationDbContext dbContext)
        {
            _announcementService = announcementService;
            _dbContext = dbContext;
        }
        public async Task<IActionResult> Index()
        {
            // ---- VIDEO CONFIG (Default + Extra Links) ----
            var videoConfig = await _dbContext.VideoConfigs
                .Include(v => v.ExtraVideoLinks)
                .FirstOrDefaultAsync();

            var videoEmbedUrls = new List<string>();

            if (videoConfig != null)
            {
                // Eğer varsayılan aktifse, sadece onu kullan
                if (videoConfig.IsDefaultActive && !string.IsNullOrWhiteSpace(videoConfig.DefaultVideoUrl))
                {
                    var embed = ToYoutubeEmbedUrl(videoConfig.DefaultVideoUrl);
                    if (!string.IsNullOrWhiteSpace(embed))
                        videoEmbedUrls.Add(embed);
                }
                else
                {
                    // Önce aktif ekstra linkleri sıraya göre al
                    var activeExtras = videoConfig.ExtraVideoLinks
                        .Where(x => x.IsActive)
                        .OrderBy(x => x.DisplayOrder)
                        .Select(x => x.Url)
                        .ToList();

                    if (activeExtras.Any())
                    {
                        foreach (var url in activeExtras)
                        {
                            var embed = ToYoutubeEmbedUrl(url);
                            if (!string.IsNullOrWhiteSpace(embed))
                                videoEmbedUrls.Add(embed);
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(videoConfig.DefaultVideoUrl))
                    {
                        var embed = ToYoutubeEmbedUrl(videoConfig.DefaultVideoUrl);
                        if (!string.IsNullOrWhiteSpace(embed))
                            videoEmbedUrls.Add(embed);
                    }
                }
            }

            // JS'te kullanmak için JSON olarak da gönder
            ViewBag.VideoEmbedJson = JsonConvert.SerializeObject(videoEmbedUrls);

            // ---- VIEWMODEL ----
            var (announcements, news) = await GetAnnouncementsAndNews();
            var viewModel = new DashboardViewModel
            {
                Videos = new Video(),
                VideoEmbedUrls = videoEmbedUrls,
                Announcements = announcements,
                News = news,
                Weather = GetMockWeather(),
                WeatherForecast = GetMockWeatherForecast(),
                ScrollingAnnouncements = GetMockScrollingAnnouncements(),
                Credits = GetMockCredits()
            };

            return View(viewModel);
        }

        private string? ToYoutubeEmbedUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;

            try
            {
                var trimmed = url.Trim();

                // v= parametresinden ID çek
                if (trimmed.Contains("v="))
                {
                    var part = trimmed.Split("v=")[1];
                    var ampIndex = part.IndexOf('&');
                    var id = ampIndex >= 0 ? part[..ampIndex] : part;
                    // Tek videoyu embed ediyoruz, döngüyü kendi JS mantığımız yapıyor
                    return $"https://www.youtube.com/embed/{id}?autoplay=1&mute=1";
                }

                // youtu.be kısa linkleri
                var marker = "youtu.be/";
                if (trimmed.Contains(marker))
                {
                    var part = trimmed.Split(marker)[1];
                    var qIndex = part.IndexOfAny(new[] { '?', '&' });
                    var id = qIndex >= 0 ? part[..qIndex] : part;
                    return $"https://www.youtube.com/embed/{id}?autoplay=1&mute=1";
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        private async Task<(List<AnnouncementDto> Announcements, List<AnnouncementDto> News)> GetAnnouncementsAndNews()
        {
            // Fetch announcements (haber=false) and news (haber=true) separately
            var duyurular = await _announcementService.GetAnnouncements();
            var haberler = await _announcementService.GetNews();

            // Convert to DTOs - Keep announcements separate
            var announcementList = duyurular.Select(x => new AnnouncementDto
            {
                ID = x.ID,
                Title = x.Baslik,
                Content = x.Icerik,
                CreatedDate = x.KayitTarihi,
                PosterImageUrl = x.HaberResim,
                DisplayOrder = x.ID,
                Haber = false
            }).OrderBy(x => x.ID).ToList();

            // Convert to DTOs - Keep news separate
            var newsList = haberler.Select(x => new AnnouncementDto
            {
                ID = x.ID,
                Title = x.Baslik,
                Content = x.Icerik,
                CreatedDate = x.KayitTarihi,
                PosterImageUrl = x.HaberResim,
                DisplayOrder = x.ID,
                Haber = true
            }).OrderBy(x => x.ID).ToList();

            return (announcementList, newsList);
        }

        private Weather GetMockWeather()
        {
            return new Weather
            {
                City = "Isparta",
                Temperature = 22,
                Condition = "Parçalı Bulutlu",
                Icon = "⛅",
                LastUpdated = DateTime.Now
            };
        }

        private WeatherForecast GetMockWeatherForecast()
        {
            var today = DateTime.Today;
            var turkishDays = new[] { "Pzt", "Sal", "Çar", "Per", "Cum", "Cmt", "Paz" };
            
            var forecast = new WeatherForecast
            {
                City = "Isparta",
                LastUpdated = DateTime.Now,
                Days = new List<WeatherDay>
                {
                    // Day -3 (3 days ago)
                    new WeatherDay 
                    { 
                        Date = today.AddDays(-3), 
                        Temperature = 18, 
                        Condition = "Yağmurlu", 
                        Icon = "🌧️", 
                        DayName = turkishDays[(int)(today.AddDays(-3).DayOfWeek + 6) % 7],
                        IsPastDay = true
                    },
                    // Day -2 (2 days ago)
                    new WeatherDay 
                    { 
                        Date = today.AddDays(-2), 
                        Temperature = 20, 
                        Condition = "Bulutlu", 
                        Icon = "☁️", 
                        DayName = turkishDays[(int)(today.AddDays(-2).DayOfWeek + 6) % 7],
                        IsPastDay = true
                    },
                    // Day -1 (Yesterday)
                    new WeatherDay 
                    { 
                        Date = today.AddDays(-1), 
                        Temperature = 19, 
                        Condition = "Parçalı Bulutlu", 
                        Icon = "⛅", 
                        DayName = turkishDays[(int)(today.AddDays(-1).DayOfWeek + 6) % 7],
                        IsPastDay = true
                    },
                    // Current Day (Today)
                    new WeatherDay 
                    { 
                        Date = today, 
                        Temperature = 22, 
                        Condition = "Parçalı Bulutlu", 
                        Icon = "⛅", 
                        DayName = turkishDays[(int)(today.DayOfWeek + 6) % 7],
                        IsCurrentDay = true
                    },
                    // Day +1 (Tomorrow)
                    new WeatherDay 
                    { 
                        Date = today.AddDays(1), 
                        Temperature = 24, 
                        Condition = "Güneşli", 
                        Icon = "☀️", 
                        DayName = turkishDays[(int)(today.AddDays(1).DayOfWeek + 6) % 7],
                        IsFutureDay = true
                    },
                    // Day +2 (Day after tomorrow)
                    new WeatherDay 
                    { 
                        Date = today.AddDays(2), 
                        Temperature = 26, 
                        Condition = "Güneşli", 
                        Icon = "☀️", 
                        DayName = turkishDays[(int)(today.AddDays(2).DayOfWeek + 6) % 7],
                        IsFutureDay = true
                    },
                    // Day +3 (3 days from now)
                    new WeatherDay 
                    { 
                        Date = today.AddDays(3), 
                        Temperature = 23, 
                        Condition = "Parçalı Bulutlu", 
                        Icon = "⛅", 
                        DayName = turkishDays[(int)(today.AddDays(3).DayOfWeek + 6) % 7],
                        IsFutureDay = true
                    }
                }
            };

            return forecast;
        }

        private List<string> GetMockScrollingAnnouncements()
        {
            return new List<string>
            {
                "Yeni dönem kayıtları başladı",
                "Final haftası programı açıklandı",
                "Yaz stajı başvuruları devam ediyor",
                "Bitirme projesi sunumları yaklaşıyor",
                "Yapay zeka laboratuvarı hizmete girdi",
                "Öğrenci kulüp etkinlikleri başlıyor"
            };
        }

        private List<string> GetMockCredits()
        {
            return new List<string>
            {
                "Emeği Geçenler: Barış Köse, Ahmet Yılmaz, Ayşe Demir, Mehmet Kaya, Fatma Özkan, Can Yıldız, Zeynep Arslan, Emre Çelik"
            };
        }

        // API endpoint for scrolling announcements - returns only titles
        [HttpGet]
        public async Task<IActionResult> GetScrollingAnnouncements()
        {
            // Önce admin'in özel kayan yazısı var mı bak
            var ticker = await _dbContext.TickerConfigs.FirstOrDefaultAsync(t => t.IsActive);
            if (ticker != null && !string.IsNullOrWhiteSpace(ticker.CustomText))
            {
                return Json(new { customText = ticker.CustomText });
            }

            var (announcements, news) = await GetAnnouncementsAndNews();
            
            var result = new
            {
                duyurular = announcements.Select(x => new { title = x.Title }).ToList(),
                haberler = news.Select(x => new { title = x.Title }).ToList()
            };
            
            return Json(result);
        }
    }
}
