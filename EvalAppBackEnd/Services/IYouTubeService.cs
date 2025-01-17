using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using EvalAppBackEnd.DTOs;
using EvalAppBackEnd.Services;

namespace EvalAppBackEnd.Services
{
    public interface IYouTubeService
    {
        Task<List<YoutubeVideoInfo>> GetPlaylistVideos(string playlistId);
        Task<bool> ValidatePlaylistId(string playlistId);
    }
}
