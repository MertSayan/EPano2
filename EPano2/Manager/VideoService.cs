using EPano2.Interfaces;
using EPano2.Models;

namespace EPano2.Manager
{
    public class VideoService:IVideoService
    {
        private static Video _playlist = new Video
        {
            Id = Guid.NewGuid(),
            YoutubePlaylistUrl = "https://www.youtube.com/playlist?list=PL123456"
        };

        public Video GetPlaylist()
        {
            return _playlist;
        }

        public void SavePlaylist(string playlistUrl)
        {
            _playlist.YoutubePlaylistUrl = playlistUrl;
        }
    }
}
