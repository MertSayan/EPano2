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


        public IActionResult Dashboard()
        {
            return View();
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
