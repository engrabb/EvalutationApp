using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using EvalAppBackEnd.DTOs;

namespace EvalAppBackEnd.Services
{
    public class YouTubeService : IYouTubeService
    {
        private readonly Google.Apis.YouTube.v3.YouTubeService _youtubeService;

        public YouTubeService(IConfiguration configuration)
        {
            _youtubeService = new Google.Apis.YouTube.v3.YouTubeService(new BaseClientService.Initializer
            {
                ApiKey = configuration["YouTube:ApiKey"]
            });
        }

        public async Task<List<YoutubeVideoInfo>> GetPlaylistVideos(string playlistId)
        {
            var videos = new List<YoutubeVideoInfo>();
            var nextPageToken = "";

            try
            {
                do
                {
                    var playlistRequest = _youtubeService.PlaylistItems.List("snippet");
                    playlistRequest.PlaylistId = playlistId;
                    playlistRequest.MaxResults = 50;
                    playlistRequest.PageToken = nextPageToken;

                    var response = await playlistRequest.ExecuteAsync();
                    nextPageToken = response.NextPageToken;

                    foreach (var item in response.Items)
                    {
                        videos.Add(new YoutubeVideoInfo
                        {
                            Title = item.Snippet.Title,
                            VideoId = item.Snippet.ResourceId.VideoId,
                            Description = item.Snippet.Description
                        });
                    }
                } while (!string.IsNullOrEmpty(nextPageToken));

                return videos;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching playlist: {ex.Message}");
            }
        }

        public async Task<bool> ValidatePlaylistId(string playlistId)
        {
            try
            {
                var playlistRequest = _youtubeService.PlaylistItems.List("snippet");
                playlistRequest.PlaylistId = playlistId;
                playlistRequest.MaxResults = 1;

                var response = await playlistRequest.ExecuteAsync();
                return response.Items.Any();
            }
            catch
            {
                return false;
            }
        }
    }
}
