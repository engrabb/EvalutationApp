using System.ComponentModel.DataAnnotations;

namespace EvalAppBackEnd.Models
{
    public class VideoComment
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public TimeSpan Timestamp { get; set; }

        public DateTime CreatedAt { get; set; }

        [Required]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        [Required]
        public int VideoId { get; set; }

        public Video Video { get; set; }
    }
}
