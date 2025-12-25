using EPano2.Data;
using EPano2.Interfaces;
using EPano2.Models;
using EPano2.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace EPano2.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IVideoService _videoService;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHubContext<AppUpdateHub> _hubContext;

        public AdminController(IVideoService videoService, ApplicationDbContext dbContext, IHubContext<AppUpdateHub> hubContext)
        {
            _videoService = videoService;
            _dbContext = dbContext;
            _hubContext = hubContext;
        }

        // /Admin isteği varsayılan olarak bu aksiyona gelir
        public IActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }

        public async Task<IActionResult> Dashboard()
        {
            // Video config tek kayıt mantığıyla kullanılıyor
            var videoConfig = await _dbContext.VideoConfigs
                .Include(v => v.ExtraVideoLinks)
                .FirstOrDefaultAsync();

            if (videoConfig == null)
            {
                videoConfig = new VideoConfig();
                _dbContext.VideoConfigs.Add(videoConfig);
                await _dbContext.SaveChangesAsync();
            }

            var tickerItems = await _dbContext.TickerItems
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.CreatedAt)
                .ToListAsync();

            var vm = new AdminDashboardViewModel
            {
                DefaultVideoFilePath = videoConfig.DefaultVideoFilePath,
                ExtraVideoLinks = videoConfig.ExtraVideoLinks
                    .OrderBy(x => x.DisplayOrder)
                    .ToList(),
                TickerItems = tickerItems
            };

            return View(vm);
        }

        // GET: /Login
        [HttpGet("/Login")]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View("Login");
        }

        // POST: /Login
        [HttpPost("/Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı adı ve şifre zorunludur.");
                return View("Login");
            }

            var user = await _dbContext.AdminUsers
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Geçersiz kullanıcı adı veya şifre.");
                return View("Login");
            }

            // Cookie tabanlı authentication için ClaimsPricipal oluştur
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role ?? "Admin")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [DisableRequestSizeLimit] // Video dosyaları için size limit kaldırıldı
        public async Task<IActionResult> SaveDefaultVideo(IFormFile videoFile)
        {
            try
            {
                if (videoFile == null || videoFile.Length == 0)
                {
                    TempData["ErrorMessage"] = "Lütfen bir video dosyası seçin.";
                    return RedirectToAction("Dashboard");
                }

                // Video formatı kontrolü
                var allowedExtensions = new[] { ".mp4", ".webm", ".ogg", ".mov" };
                var fileExtension = Path.GetExtension(videoFile.FileName).ToLowerInvariant();
                if (string.IsNullOrWhiteSpace(fileExtension) || !allowedExtensions.Contains(fileExtension))
                {
                    TempData["ErrorMessage"] = "Sadece video formatları kabul edilir (mp4, webm, ogg, mov).";
                    return RedirectToAction("Dashboard");
                }

                var config = await _dbContext.VideoConfigs
                    .Include(v => v.ExtraVideoLinks)
                    .FirstOrDefaultAsync() ?? new VideoConfig();

                if (config.Id == 0)
                {
                    _dbContext.VideoConfigs.Add(config);
                    await _dbContext.SaveChangesAsync();
                }

                // Güvenli dosya adı oluştur
                var safeFileName = GenerateSafeFileName(videoFile?.FileName ?? "video.mp4");
                var videosPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos");
                
                // Klasör yoksa oluştur
                if (!Directory.Exists(videosPath))
                {
                    Directory.CreateDirectory(videosPath);
                }

                // Eski default video varsa sil
                if (!string.IsNullOrWhiteSpace(config.DefaultVideoFilePath))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", config.DefaultVideoFilePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        try
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                        catch { } // Silme hatası görmezden gel
                    }
                }

                // Yeni dosyayı kaydet
                var filePath = Path.Combine(videosPath, safeFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    if (videoFile != null)
                    {
                        await videoFile.CopyToAsync(stream);
                    }
                }

                // Veritabanına kaydet
                config.DefaultVideoFilePath = $"/videos/{safeFileName}";
                await _dbContext.SaveChangesAsync();

                // SignalR ile tüm istemcilere güncelleme bildir
                await _hubContext.Clients.All.SendAsync("AppDataUpdated");

                TempData["SuccessMessage"] = "Varsayılan video başarıyla yüklendi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Video yükleme hatası: {ex.Message}";
            }

            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [DisableRequestSizeLimit] // Video dosyaları için size limit kaldırıldı
        public async Task<IActionResult> AddExtraVideoLink(IFormFile videoFile)
        {
            // ModelState'i temizle - video upload için gerekli değil
            ModelState.Clear();
            
            try
            {
                if (videoFile == null || videoFile.Length == 0)
                {
                    TempData["ErrorMessage"] = "Lütfen bir video dosyası seçin.";
                    return RedirectToAction("Dashboard");
                }

                // Video formatı kontrolü
                var allowedExtensions = new[] { ".mp4", ".webm", ".ogg", ".mov" };
                var fileExtension = Path.GetExtension(videoFile.FileName).ToLowerInvariant();
                if (string.IsNullOrWhiteSpace(fileExtension) || !allowedExtensions.Contains(fileExtension))
                {
                    TempData["ErrorMessage"] = "Sadece video formatları kabul edilir (mp4, webm, ogg, mov).";
                    return RedirectToAction("Dashboard");
                }

                var config = await _dbContext.VideoConfigs
                    .Include(v => v.ExtraVideoLinks)
                    .FirstOrDefaultAsync() ?? new VideoConfig();

                if (config.Id == 0)
                {
                    _dbContext.VideoConfigs.Add(config);
                    await _dbContext.SaveChangesAsync();
                }

                // Güvenli dosya adı oluştur
                var safeFileName = GenerateSafeFileName(videoFile?.FileName ?? "video.mp4");
                var videosPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos");
                
                // Klasör yoksa oluştur
                if (!Directory.Exists(videosPath))
                {
                    Directory.CreateDirectory(videosPath);
                }

                // Yeni dosyayı kaydet
                var filePath = Path.Combine(videosPath, safeFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    if (videoFile != null)
                    {
                        await videoFile.CopyToAsync(stream);
                    }
                }

                // displayOrder'ı form'dan al
                var displayOrderStr = Request.Form["displayOrder"].ToString();
                int order;
                if (!string.IsNullOrWhiteSpace(displayOrderStr) && int.TryParse(displayOrderStr, out int parsedOrder))
                {
                    order = parsedOrder;
                }
                else
                {
                    order = config.ExtraVideoLinks.Any()
                        ? config.ExtraVideoLinks.Max(x => x.DisplayOrder) + 1
                        : 1;
                }

                config.ExtraVideoLinks.Add(new ExtraVideoLink
                {
                    FilePath = $"/videos/{safeFileName}",
                    DisplayOrder = order,
                    IsActive = true
                });

                // Veritabanına kaydet
                await _dbContext.SaveChangesAsync();

                // SignalR ile tüm istemcilere güncelleme bildir
                await _hubContext.Clients.All.SendAsync("AppDataUpdated");

                TempData["SuccessMessage"] = "Video başarıyla yüklendi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Video yükleme hatası: {ex.Message}";
            }

            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteExtraVideoLink(int id)
        {
            var link = await _dbContext.ExtraVideoLinks.FindAsync(id);
            if (link != null)
            {
                // Dosyayı sil
                if (!string.IsNullOrWhiteSpace(link.FilePath))
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", link.FilePath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        try
                        {
                            System.IO.File.Delete(filePath);
                        }
                        catch { } // Silme hatası görmezden gel
                    }
                }

                _dbContext.ExtraVideoLinks.Remove(link);
                await _dbContext.SaveChangesAsync();

                // SignalR ile tüm istemcilere güncelleme bildir
                await _hubContext.Clients.All.SendAsync("AppDataUpdated");

                TempData["SuccessMessage"] = "Video başarıyla silindi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Video bulunamadı.";
            }

            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateExtraVideoLink(int id, string isActive)
        {
            var link = await _dbContext.ExtraVideoLinks.FindAsync(id);
            if (link == null)
            {
                TempData["ErrorMessage"] = "Video bulunamadı.";
                return RedirectToAction("Dashboard");
            }

            // String'den boolean'a dönüştür
            bool newActiveState = isActive == "true" || isActive == "True";
            var oldState = link.IsActive;
            link.IsActive = newActiveState;
            await _dbContext.SaveChangesAsync();

            // SignalR ile tüm istemcilere güncelleme bildir
            await _hubContext.Clients.All.SendAsync("AppDataUpdated");

            // Mesajı doğru duruma göre göster
            TempData["SuccessMessage"] = $"Video başarıyla {(newActiveState ? "aktif" : "pasif")} edildi.";
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTickerItem(string text, int? displayOrder)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                TempData["ErrorMessage"] = "Ticker metni boş olamaz.";
                return RedirectToAction("Dashboard");
            }

            int order = displayOrder ?? (_dbContext.TickerItems.Any()
                ? _dbContext.TickerItems.Max(x => x.DisplayOrder) + 1
                : 1);

            var tickerItem = new TickerItem
            {
                Text = text.Trim(),
                IsActive = true,
                DisplayOrder = order,
                CreatedAt = DateTime.Now
            };

            _dbContext.TickerItems.Add(tickerItem);
            await _dbContext.SaveChangesAsync();

            // SignalR ile tüm istemcilere güncelleme bildir
            await _hubContext.Clients.All.SendAsync("AppDataUpdated");

            TempData["SuccessMessage"] = "Ticker yazısı başarıyla eklendi.";
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTickerItem(int id, bool isActive)
        {
            var tickerItem = await _dbContext.TickerItems.FindAsync(id);
            if (tickerItem == null)
            {
                TempData["ErrorMessage"] = "Ticker yazısı bulunamadı.";
                return RedirectToAction("Dashboard");
            }

            tickerItem.IsActive = isActive;
            await _dbContext.SaveChangesAsync();

            // SignalR ile tüm istemcilere güncelleme bildir
            await _hubContext.Clients.All.SendAsync("AppDataUpdated");

            TempData["SuccessMessage"] = "Ticker yazısı başarıyla güncellendi.";
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTickerItem(int id)
        {
            var tickerItem = await _dbContext.TickerItems.FindAsync(id);
            if (tickerItem == null)
            {
                TempData["ErrorMessage"] = "Ticker yazısı bulunamadı.";
                return RedirectToAction("Dashboard");
            }

            _dbContext.TickerItems.Remove(tickerItem);
            await _dbContext.SaveChangesAsync();

            // SignalR ile tüm istemcilere güncelleme bildir
            await _hubContext.Clients.All.SendAsync("AppDataUpdated");

            TempData["SuccessMessage"] = "Ticker yazısı başarıyla silindi.";
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult Videos()
        {
            var playlist = _videoService.GetPlaylist();
            return View(playlist);
        }

        [HttpPost]
        public async Task<IActionResult> SavePlaylist(Video model)
        {
            if (string.IsNullOrWhiteSpace(model.YoutubePlaylistUrl))
            {
                TempData["Error"] = "Playlist URL boş olamaz!";
                return RedirectToAction("Videos");
            }

            _videoService.SavePlaylist(model.YoutubePlaylistUrl);

            // SignalR ile tüm istemcilere güncelleme bildir
            await _hubContext.Clients.All.SendAsync("AppDataUpdated");

            TempData["Success"] = "Playlist başarıyla güncellendi!";
            return RedirectToAction("Videos");
        }

        public IActionResult Announcements()
        {
            var announcements = GetMockAnnouncements();
            return View(announcements);
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
            return null;
        }

        private List<Announcement> GetMockAnnouncements()
        {
            return null;
        }

        /// <summary>
        /// Güvenli dosya adı oluşturur (özel karakterleri temizler, Türkçe karakterleri dönüştürür)
        /// </summary>
        private string GenerateSafeFileName(string originalFileName)
        {
            if (string.IsNullOrWhiteSpace(originalFileName))
            {
                return $"video_{Guid.NewGuid():N}.mp4";
            }

            var nameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
            var extension = Path.GetExtension(originalFileName);
            
            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = ".mp4";
            }
            
            // Türkçe karakterleri dönüştür
            nameWithoutExtension = nameWithoutExtension
                .Replace("ğ", "g").Replace("Ğ", "G")
                .Replace("ü", "u").Replace("Ü", "U")
                .Replace("ş", "s").Replace("Ş", "S")
                .Replace("ı", "i").Replace("İ", "I")
                .Replace("ö", "o").Replace("Ö", "O")
                .Replace("ç", "c").Replace("Ç", "C");

            // Özel karakterleri ve boşlukları temizle
            var safeName = Regex.Replace(nameWithoutExtension, @"[^a-zA-Z0-9_-]", "_");
            
            // Çoklu alt çizgileri tek alt çizgiye dönüştür
            safeName = Regex.Replace(safeName, @"_+", "_");
            
            // Başta ve sonda alt çizgi varsa kaldır
            safeName = safeName.Trim('_');
            
            // Boşsa varsayılan isim ver
            if (string.IsNullOrWhiteSpace(safeName))
            {
                safeName = "video";
            }

            // Guid ekle (aynı isimli dosyaların üzerine yazılmasını önlemek için)
            var guid = Guid.NewGuid().ToString("N").Substring(0, 8);
            return $"{safeName}_{guid}{extension}";
        }
    }
}
