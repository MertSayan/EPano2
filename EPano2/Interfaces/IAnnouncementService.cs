using EPano2.Models;

namespace EPano2.Interfaces
{
    public interface IAnnouncementService
    {
        Task<List<Announcement>> GetAnnouncements();
        Task<List<Announcement>> GetNews();
        Task<List<Etkinlik>> GetEtkinlikler();
    }
}
