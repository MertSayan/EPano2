using EPano2.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EPano2.Controllers
{
    public class AnnounementController : Controller
    {
        private readonly IAnnouncementService _announcementService;

        public AnnounementController(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        public async Task<IActionResult> Duyurular()
        {
            var announcements = await _announcementService.GetAnnouncements();
            return View(announcements);
        }
        public async Task<IActionResult> Haberler()
        {
            var news = await _announcementService.GetNews();
            return View(news);
        }
    }
}
