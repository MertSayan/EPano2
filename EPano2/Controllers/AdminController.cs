using EPano2.Data;
using EPano2.Interfaces;
using EPano2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace EPano2.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IVideoService _videoService;
        private readonly ApplicationDbContext _dbContext;

        public AdminController(IVideoService videoService, ApplicationDbContext dbContext)
        {
            _videoService = videoService;
            _dbContext = dbContext;
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

            var tickerConfig = await _dbContext.TickerConfigs.FirstOrDefaultAsync();
            if (tickerConfig == null)
            {
                tickerConfig = new TickerConfig();
                _dbContext.TickerConfigs.Add(tickerConfig);
                await _dbContext.SaveChangesAsync();
            }

            var vm = new AdminDashboardViewModel
            {
                DefaultVideoUrl = videoConfig.DefaultVideoUrl,
                IsDefaultActive = videoConfig.IsDefaultActive,
                ExtraVideoLinks = videoConfig.ExtraVideoLinks
                    .OrderBy(x => x.DisplayOrder)
                    .ToList(),
                TickerCustomText = tickerConfig.CustomText,
                TickerIsActive = tickerConfig.IsActive
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
        public async Task<IActionResult> SaveDefaultVideo(string? defaultVideoUrl, bool useDefault = false)
        {
            var config = await _dbContext.VideoConfigs
                .Include(v => v.ExtraVideoLinks)
                .FirstOrDefaultAsync() ?? new VideoConfig();

            config.DefaultVideoUrl = string.IsNullOrWhiteSpace(defaultVideoUrl)
                ? null
                : defaultVideoUrl.Trim();

            // Varsayılan ancak geçerli bir URL varsa aktif edilebilsin
            config.IsDefaultActive = !string.IsNullOrWhiteSpace(config.DefaultVideoUrl) && useDefault;

            if (config.Id == 0)
            {
                _dbContext.VideoConfigs.Add(config);
            }

            await _dbContext.SaveChangesAsync();
            TempData["VideoMessage"] = "Varsayılan video linki kaydedildi.";
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddExtraVideoLink(string url, int? displayOrder)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                TempData["VideoError"] = "Video linki boş olamaz.";
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

            int order = displayOrder ?? (config.ExtraVideoLinks.Any()
                ? config.ExtraVideoLinks.Max(x => x.DisplayOrder) + 1
                : 1);

            config.ExtraVideoLinks.Add(new ExtraVideoLink
            {
                Url = url.Trim(),
                DisplayOrder = order,
                IsActive = true
            });

            await _dbContext.SaveChangesAsync();
            TempData["VideoMessage"] = "Ekstra video linki eklendi.";
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteExtraVideoLink(int id)
        {
            var link = await _dbContext.ExtraVideoLinks.FindAsync(id);
            if (link != null)
            {
                _dbContext.ExtraVideoLinks.Remove(link);
                await _dbContext.SaveChangesAsync();
            }

            TempData["VideoMessage"] = "Video linki silindi.";
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveTickerConfig(string? customText, bool isActive)
        {
            var ticker = await _dbContext.TickerConfigs.FirstOrDefaultAsync() ?? new TickerConfig();

            ticker.CustomText = string.IsNullOrWhiteSpace(customText) ? null : customText.Trim();
            ticker.IsActive = isActive;

            if (ticker.Id == 0)
            {
                _dbContext.TickerConfigs.Add(ticker);
            }

            await _dbContext.SaveChangesAsync();
            TempData["TickerMessage"] = "Kayan yazı ayarı kaydedildi.";
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
        public IActionResult SavePlaylist(Video model)
        {
            if (string.IsNullOrWhiteSpace(model.YoutubePlaylistUrl))
            {
                TempData["Error"] = "Playlist URL boş olamaz!";
                return RedirectToAction("Videos");
            }

            _videoService.SavePlaylist(model.YoutubePlaylistUrl);

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
    }
}
