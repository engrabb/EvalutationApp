using System.ComponentModel.DataAnnotations;

namespace EvalAppBackEnd.Models
{
    public class Playlist
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public string YoutubePlaylistId { get; set; }

        [Required]
        public string CreatedById { get; set; }

        public ApplicationUser CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<Video> Videos { get; set; }
    }

}
