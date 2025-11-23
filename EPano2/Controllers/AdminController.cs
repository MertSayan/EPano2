using EPano2.Interfaces;
using EPano2.Models;
using Microsoft.AspNetCore.Mvc;

namespace EPano2.Controllers
{
    public class AdminController : Controller
    {
        private readonly IVideoService _videoService;

        public AdminController(IVideoService videoService)
        {
            _videoService = videoService;
        }

        public IActionResult Dashboard()
        {
            return View();
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
