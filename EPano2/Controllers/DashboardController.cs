using EPano2.Models;
using Microsoft.AspNetCore.Mvc;

namespace EPano2.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            var viewModel = new DashboardViewModel
            {
                Videos = GetMockVideos(),
                Announcements = GetMockAnnouncements(),
                Weather = GetMockWeather(),
                WeatherForecast = GetMockWeatherForecast(),
                ScrollingAnnouncements = GetMockScrollingAnnouncements(),
                Credits = GetMockCredits()
            };

            return View(viewModel);
        }

        private List<Video> GetMockVideos()
        {
            return new List<Video>
            {
                new Video { Id = 1, Title = "Bilgisayar Mühendisliği Tanıtım", Url = "https://sample-videos.com/zip/10/mp4/SampleVideo_1280x720_1mb.mp4", Description = "Bölümümüzün tanıtım videosu", UploadDate = DateTime.Now.AddDays(-5), DisplayOrder = 1 },
                new Video { Id = 2, Title = "Yazılım Geliştirme Süreçleri", Url = "https://sample-videos.com/zip/10/mp4/SampleVideo_1280x720_2mb.mp4", Description = "Yazılım geliştirme metodolojileri", UploadDate = DateTime.Now.AddDays(-3), DisplayOrder = 2 },
                new Video { Id = 3, Title = "Veri Yapıları ve Algoritmalar", Url = "https://sample-videos.com/zip/10/mp4/SampleVideo_1280x720_5mb.mp4", Description = "Temel veri yapıları eğitimi", UploadDate = DateTime.Now.AddDays(-1), DisplayOrder = 3 }
            };
        }

        private List<Announcement> GetMockAnnouncements()
        {
            return new List<Announcement>
            {
                new Announcement { 
                    Id = 1, 
                    Title = "2024 Bahar Dönemi Final Sınavları", 
                    Content = "Final sınavları 15 Haziran tarihinde başlayacaktır. Sınav programı bölüm panosunda ilan edilmiştir.", 
                    PosterImageUrl = "https://images.unsplash.com/photo-1523050854058-8df90110c9f1?w=800&h=400&fit=crop",
                    CreatedDate = DateTime.Now.AddDays(-2), 
                    DisplayOrder = 1,
                    IsPosterStyle = true
                },
                new Announcement { 
                    Id = 2, 
                    Title = "Yaz Stajı Başvuruları", 
                    Content = "2024 yaz stajı başvuruları başlamıştır. Son başvuru tarihi 30 Mayıs'tır.", 
                    PosterImageUrl = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=800&h=400&fit=crop",
                    CreatedDate = DateTime.Now.AddDays(-1), 
                    DisplayOrder = 2,
                    IsPosterStyle = true
                },
                new Announcement { 
                    Id = 3, 
                    Title = "Bitirme Projesi Sunumları", 
                    Content = "Bitirme projesi sunumları 20-25 Haziran tarihleri arasında yapılacaktır.", 
                    PosterImageUrl = "https://images.unsplash.com/photo-1516321318423-f06f85e504b3?w=800&h=400&fit=crop",
                    CreatedDate = DateTime.Now, 
                    DisplayOrder = 3,
                    IsPosterStyle = true
                },
                new Announcement { 
                    Id = 4, 
                    Title = "Yeni Laboratuvar Açılışı", 
                    Content = "Yapay zeka laboratuvarımız hizmete girmiştir. Öğrencilerimiz randevu alarak kullanabilir.", 
                    PosterImageUrl = "https://images.unsplash.com/photo-1518709268805-4e9042af2176?w=800&h=400&fit=crop",
                    CreatedDate = DateTime.Now.AddDays(-3), 
                    DisplayOrder = 4,
                    IsPosterStyle = true
                }
            };
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
    }
}
