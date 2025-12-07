using System.Collections.Generic;

namespace EPano2.Models
{
    /// <summary>
    /// Admin Dashboard ekranında kullanılacak basit ViewModel.
    /// </summary>
    public class AdminDashboardViewModel
    {
        public string? DefaultVideoUrl { get; set; }
        public bool IsDefaultActive { get; set; }
        public List<ExtraVideoLink> ExtraVideoLinks { get; set; } = new List<ExtraVideoLink>();

        public List<TickerItem> TickerItems { get; set; } = new List<TickerItem>();
    }
}


