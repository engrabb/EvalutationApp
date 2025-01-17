using System.ComponentModel.DataAnnotations;

namespace EvalAppBackEnd.DTOs
{
    public class CreatePlaylistDto
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public string YoutubePlaylistId { get; set; }
    }

    public class AddCommentDto
    {
        [Required]
        public string Content { get; set; }

        [Required]
        public double TimestampSeconds { get; set; }
    }

    public class PlaylistResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string YoutubePlaylistId { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<VideoDto> Videos { get; set; }
    }

    public class VideoDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string YoutubeVideoId { get; set; }
        public string Description { get; set; }
        public string ThumbnailUrl { get; set; }
        public List<CommentDto> Comments { get; set; }
    }

    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public TimeSpan Timestamp { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class YoutubeVideoInfo
    {
        public string Title { get; set; }
        public string VideoId { get; set; }
        public string Description { get; set; }
        public string ThumbnailUrl { get; set; }
    }
}
    