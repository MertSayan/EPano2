using EPano2.Models;

namespace EPano2.Interfaces
{
    public interface IVideoService
    {
        Video GetPlaylist();
        void SavePlaylist(string playlistUrl);
    }
}
