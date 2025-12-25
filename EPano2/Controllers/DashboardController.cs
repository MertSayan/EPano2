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

            var videoFilePaths = new List<string>();
            string? defaultVideoFilePath = null;

            if (videoConfig != null)
            {
                defaultVideoFilePath = videoConfig.DefaultVideoFilePath;

                // Aktif ekstra videoları sıraya göre al
                // Eğer aktif ekstra videolar varsa onlar dönecek (varsayılan video değil)
                var activeExtras = videoConfig.ExtraVideoLinks
                    .Where(x => x.IsActive && !string.IsNullOrWhiteSpace(x.FilePath))
                    .OrderBy(x => x.DisplayOrder)
                    .Select(x => x.FilePath!)
                    .ToList();

                if (activeExtras.Any())
                {
                    // Aktif ekstra videolar varsa onları kullan (varsayılan video değil)
                    videoFilePaths = activeExtras;
                }
                // Aktif ekstra video yoksa varsayılan video kullanılacak (frontend'de kontrol edilecek)
            }

            // JS'te kullanmak için JSON olarak da gönder
            ViewBag.VideoFilePathsJson = JsonConvert.SerializeObject(videoFilePaths);
            ViewBag.DefaultVideoFilePath = defaultVideoFilePath;

            // ---- VIEWMODEL ----
            var (announcements, news) = await GetAnnouncementsAndNews();
            var etkinlikler = await GetEtkinlikler();
            var viewModel = new DashboardViewModel
            {
                Videos = new Video(),
                VideoFilePaths = videoFilePaths,
                DefaultVideoFilePath = defaultVideoFilePath,
                Announcements = announcements,
                News = news,
                Etkinlikler = etkinlikler,
                Weather = GetMockWeather(),
                WeatherForecast = GetMockWeatherForecast(),
                ScrollingAnnouncements = GetMockScrollingAnnouncements(),
                Credits = GetMockCredits()
            };

            return View(viewModel);
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

        private async Task<List<AnnouncementDto>> GetEtkinlikler()
        {
            // Fetch events
            var etkinlikler = await _announcementService.GetEtkinlikler();

            // Convert to DTOs - Map API fields to DTO: adi->Title, duyuruMetni->Content, afisUrl->PosterImageUrl
            var etkinlikList = etkinlikler.Select(x => new AnnouncementDto
            {
                ID = x.ID,
                Title = x.Baslik, // Maps from API field "adi"
                Content = x.Icerik, // Maps from API field "duyuruMetni"
                CreatedDate = x.BaslangicTarihi ?? x.KayitTarihi, // Maps from API field "tarih"
                PosterImageUrl = x.Resim, // Maps from API field "afisUrl"
                DisplayOrder = x.ID,
                Haber = false // We'll use data-etkinlik attribute in view to distinguish
            }).OrderBy(x => x.ID).ToList();

            return etkinlikList;
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
            // Önce aktif ticker item'ları kontrol et
            var activeTickerItems = await _dbContext.TickerItems
                .Where(t => t.IsActive)
                .OrderBy(t => t.DisplayOrder)
                .ThenBy(t => t.CreatedAt)
                .ToListAsync();

            if (activeTickerItems.Any())
            {
                // Aktif ticker'ları birleştir
                var combinedText = string.Join(" • ", activeTickerItems.Select(t => t.Text));
                return Json(new { customText = combinedText });
            }

            // Aktif ticker yoksa API'den gelen duyuru, haber ve etkinlikleri kullan
            var (announcements, news) = await GetAnnouncementsAndNews();
            var etkinlikler = await GetEtkinlikler();
            
            var result = new
            {
                haberler = news.Select(x => new { title = x.Title }).ToList(),
                duyurular = announcements.Select(x => new { title = x.Title }).ToList(),
                etkinlikler = etkinlikler.Select(x => new { title = x.Title }).ToList()
            };
            
            return Json(result);
        }

        // API endpoint for video configuration (for SignalR real-time updates)
        [HttpGet]
        public async Task<IActionResult> GetVideoConfig()
        {
            var videoConfig = await _dbContext.VideoConfigs
                .Include(v => v.ExtraVideoLinks)
                .FirstOrDefaultAsync();

            var videoFilePaths = new List<string>();
            string? defaultVideoFilePath = null;

            if (videoConfig != null)
            {
                defaultVideoFilePath = videoConfig.DefaultVideoFilePath;

                var activeExtras = videoConfig.ExtraVideoLinks
                    .Where(x => x.IsActive && !string.IsNullOrWhiteSpace(x.FilePath))
                    .OrderBy(x => x.DisplayOrder)
                    .Select(x => x.FilePath!)
                    .ToList();

                if (activeExtras.Any())
                {
                    videoFilePaths = activeExtras;
                }
            }

            return Json(new
            {
                videoFilePaths = videoFilePaths,
                defaultVideoFilePath = defaultVideoFilePath
            });
        }

        // API endpoint for announcements data (for SignalR real-time updates)
        [HttpGet]
        public async Task<IActionResult> GetAnnouncementsData()
        {
            var (announcements, news) = await GetAnnouncementsAndNews();
            var etkinlikler = await GetEtkinlikler();

            return Json(new
            {
                announcements = announcements,
                news = news,
                etkinlikler = etkinlikler
            });
        }
    }
}
