using EPano2.Interfaces;
using EPano2.Models;
using EPano2.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EPano2.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IAnnouncementService _announcementService;

        public DashboardController(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }
        public async Task<IActionResult> Index()
        {
            // ---- STATIC PLAYLIST ----
            var video = new Video
            {
                Id = Guid.NewGuid(),
                YoutubePlaylistUrl = "https://www.youtube.com/watch?v=NPUTdqYUa9A&list=PLui0qrYBvKS-OeLalPILU5SxnG_VGPLQk"
            };

            // Playlist ID çıkar
            string playlistId = "";
            if (video.YoutubePlaylistUrl.Contains("list="))
                playlistId = video.YoutubePlaylistUrl.Split("list=")[1];

            ViewBag.PlaylistId = playlistId;

            // ---- STATIC VIEWMODEL ----
            var (announcements, news) = await GetAnnouncementsAndNews();
            var viewModel = new DashboardViewModel
            {
                Videos = video,
                Announcements = announcements,
                News = news,
                Weather = GetMockWeather(),
                WeatherForecast = GetMockWeatherForecast(),
                ScrollingAnnouncements = GetMockScrollingAnnouncements(),
                Credits = GetMockCredits()
            };

            return View(viewModel);
        }

        private Video GetMockVideos()
        {
            return new Video
            {
                Id = Guid.NewGuid(),
                YoutubePlaylistUrl = "https://www.youtube.com/watch?v=NPUTdqYUa9A&list=PLui0qrYBvKS-OeLalPILU5SxnG_VGPLQk"
            };
            
            //return new List<Video>
            //{
            //    new Video { Id = 1, Title = "Bilgisayar Mühendisliği Tanıtım", Url = "https://sample-videos.com/zip/10/mp4/SampleVideo_1280x720_1mb.mp4", Description = "Bölümümüzün tanıtım videosu", UploadDate = DateTime.Now.AddDays(-5), DisplayOrder = 1 },
            //    new Video { Id = 2, Title = "Yazılım Geliştirme Süreçleri", Url = "https://sample-videos.com/zip/10/mp4/SampleVideo_1280x720_2mb.mp4", Description = "Yazılım geliştirme metodolojileri", UploadDate = DateTime.Now.AddDays(-3), DisplayOrder = 2 },
            //    new Video { Id = 3, Title = "Veri Yapıları ve Algoritmalar", Url = "https://sample-videos.com/zip/10/mp4/SampleVideo_1280x720_5mb.mp4", Description = "Temel veri yapıları eğitimi", UploadDate = DateTime.Now.AddDays(-1), DisplayOrder = 3 }
            //};
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
