using EvalAppBackEnd.Data;
using EvalAppBackEnd.DTOs;
using EvalAppBackEnd.Models;
using EvalAppBackEnd.Services;
using Google.Apis.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace EvalAppBackEnd.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IYouTubeService _youtubeService;

        public PlaylistController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IYouTubeService youtubeService)
        {
            _context = context;
            _userManager = userManager;
            _youtubeService = youtubeService;
        }

        [HttpPost]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> CreatePlaylist([FromBody] CreatePlaylistDto dto)
        {
            try
            {
                // Validate the playlist ID first
                var isValid = await _youtubeService.ValidatePlaylistId(dto.YoutubePlaylistId);
                if (!isValid)
                {
                    return BadRequest(new { message = "Invalid or inaccessible YouTube playlist" });
                }

                var user = await _userManager.GetUserAsync(User);

                var playlist = new Playlist
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    YoutubePlaylistId = dto.YoutubePlaylistId,
                    CreatedById = user.Id,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Playlists.Add(playlist);
                await _context.SaveChangesAsync();

                // Fetch videos using the service
                var videos = await _youtubeService.GetPlaylistVideos(dto.YoutubePlaylistId);
                foreach (var video in videos)
                {
                    _context.Videos.Add(new Video
                    {
                        PlaylistId = playlist.Id,
                        Title = video.Title,
                        YoutubeVideoId = video.VideoId,
                        Description = video.Description,
                        ThumbnailUrl = video.ThumbnailUrl
                    });
                }
                await _context.SaveChangesAsync();

                // Return the created playlist with videos
                return CreatedAtAction(
                    nameof(GetPlaylistResponse),
                    new { id = playlist.Id },
                    await GetPlaylistResponse(playlist.Id));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error creating playlist: {ex.Message}" });
            }
        }

        // Add this helper method
        private async Task<PlaylistResponseDto> GetPlaylistResponse(int playlistId)
        {
            var playlist = await _context.Playlists
                .Include(p => p.CreatedBy)
                .Include(p => p.Videos)
                    .ThenInclude(v => v.Comments)
                        .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(p => p.Id == playlistId);

            return new PlaylistResponseDto
            {
                Id = playlist.Id,
                Title = playlist.Title,
                Description = playlist.Description,
                YoutubePlaylistId = playlist.YoutubePlaylistId,
                CreatedByName = $"{playlist.CreatedBy.FirstName} {playlist.CreatedBy.LastName}",
                CreatedAt = playlist.CreatedAt,
                Videos = playlist.Videos.Select(v => new VideoDto
                {
                    Id = v.Id,
                    Title = v.Title,
                    YoutubeVideoId = v.YoutubeVideoId,
                    Description = v.Description,
                    ThumbnailUrl = v.ThumbnailUrl,
                    Comments = v.Comments.Select(c => new CommentDto
                    {
                        Id = c.Id,
                        Content = c.Content,
                        Timestamp = c.Timestamp,
                        UserName = $"{c.User.FirstName} {c.User.LastName}",
                        CreatedAt = c.CreatedAt
                    }).ToList()
                }).ToList()
            };
        }
    }
}
