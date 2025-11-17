using EPano2.Models;
using Microsoft.AspNetCore.Mvc;

namespace EPano2.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            ViewBag.TotalVideos = GetMockVideos().Count;
            ViewBag.TotalAnnouncements = GetMockAnnouncements().Count;
            ViewBag.ActiveVideos = GetMockVideos().Count(v => v.IsActive);
            ViewBag.ActiveAnnouncements = GetMockAnnouncements().Count(a => a.IsActive);
            
            return View();
        }

        public IActionResult Videos()
        {
            var videos = GetMockVideos();
            return View(videos);
        }

        public IActionResult Announcements()
        {
            var announcements = GetMockAnnouncements();
            return View(announcements);
        }

        [HttpPost]
        public IActionResult AddVideo(Video video)
        {
            // Mock implementation - in real app, save to database
            return RedirectToAction("Videos");
        }

        [HttpPost]
        public IActionResult EditVideo(Video video)
        {
            // Mock implementation - in real app, update in database
            return RedirectToAction("Videos");
        }

        [HttpPost]
        public IActionResult DeleteVideo(int id)
        {
            // Mock implementation - in real app, delete from database
            return RedirectToAction("Videos");
        }

        [HttpPost]
        public IActionResult AddAnnouncement(Announcement announcement)
        {
            // Mock implementation - in real app, save to database
            return RedirectToAction("Announcements");
        }

        [HttpPost]
        public IActionResult EditAnnouncement(Announcement announcement)
        {
            // Mock implementation - in real app, update in database
            return RedirectToAction("Announcements");
        }

        [HttpPost]
        public IActionResult DeleteAnnouncement(int id)
        {
            // Mock implementation - in real app, delete from database
            return RedirectToAction("Announcements");
        }

        private List<Video> GetMockVideos()
        {
            return new List<Video>
            {
                new Video { Id = 1, Title = "Bilgisayar Mühendisliği Tanıtım", Url = "https://sample-videos.com/zip/10/mp4/SampleVideo_1280x720_1mb.mp4", Description = "Bölümümüzün tanıtım videosu", UploadDate = DateTime.Now.AddDays(-5), DisplayOrder = 1 },
                new Video { Id = 2, Title = "Yazılım Geliştirme Süreçleri", Url = "https://sample-videos.com/zip/10/mp4/SampleVideo_1280x720_2mb.mp4", Description = "Yazılım geliştirme metodolojileri", UploadDate = DateTime.Now.AddDays(-3), DisplayOrder = 2 },
                new Video { Id = 3, Title = "Veri Yapıları ve Algoritmalar", Url = "https://sample-videos.com/zip/10/mp4/SampleVideo_1280x720_5mb.mp4", Description = "Temel veri yapıları eğitimi", UploadDate = DateTime.Now.AddDays(-1), DisplayOrder = 3 },
                new Video { Id = 4, Title = "Web Programlama Temelleri", Url = "https://sample-videos.com/zip/10/mp4/SampleVideo_1280x720_1mb.mp4", Description = "HTML, CSS, JavaScript temelleri", UploadDate = DateTime.Now.AddDays(-7), DisplayOrder = 4 },
                new Video { Id = 5, Title = "Veritabanı Yönetimi", Url = "https://sample-videos.com/zip/10/mp4/SampleVideo_1280x720_2mb.mp4", Description = "SQL ve veritabanı tasarımı", UploadDate = DateTime.Now.AddDays(-10), DisplayOrder = 5 }
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
                },
                new Announcement { 
                    Id = 5, 
                    Title = "Öğrenci Kulüp Etkinlikleri", 
                    Content = "Bilgisayar Mühendisliği Öğrenci Kulübü tarafından düzenlenen etkinlikler başlamıştır.", 
                    PosterImageUrl = "https://images.unsplash.com/photo-1522202176988-66273c2fd55f?w=800&h=400&fit=crop",
                    CreatedDate = DateTime.Now.AddDays(-4), 
                    DisplayOrder = 5,
                    IsPosterStyle = true
                },
                new Announcement { 
                    Id = 6, 
                    Title = "Akademik Takvim Güncellemesi", 
                    Content = "2024-2025 akademik yılı takvimi güncellenmiştir. Detaylar için bölüm sekreterliğine başvurunuz.", 
                    PosterImageUrl = "https://images.unsplash.com/photo-1434030216411-0b793f4b4173?w=800&h=400&fit=crop",
                    CreatedDate = DateTime.Now.AddDays(-5), 
                    DisplayOrder = 6,
                    IsPosterStyle = true
                },
                new Announcement { 
                    Id = 7, 
                    Title = "Teknoloji Seminerleri", 
                    Content = "Her hafta düzenlenen teknoloji seminerlerine tüm öğrencilerimiz davetlidir.", 
                    PosterImageUrl = "https://images.unsplash.com/photo-1518709268805-4e9042af2176?w=800&h=400&fit=crop",
                    CreatedDate = DateTime.Now.AddDays(-6), 
                    DisplayOrder = 7,
                    IsPosterStyle = true
                },
                new Announcement { 
                    Id = 8, 
                    Title = "Proje Yarışması", 
                    Content = "Üniversite geneli proje yarışması için başvurular başlamıştır.", 
                    PosterImageUrl = "https://images.unsplash.com/photo-1551288049-bebda4e38f71?w=800&h=400&fit=crop",
                    CreatedDate = DateTime.Now.AddDays(-7), 
                    DisplayOrder = 8,
                    IsPosterStyle = true
                }
            };
        }
    }
}
