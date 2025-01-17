﻿using System.ComponentModel.DataAnnotations;

namespace EvalAppBackEnd.Models
{
    public class Video
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string YoutubeVideoId { get; set; }

        public string Description { get; set; }
        public string ThumbnailUrl { get; set; }

        [Required]
        public int PlaylistId { get; set; }

        public Playlist Playlist { get; set; }

        public ICollection<VideoComment> Comments { get; set; }
    }

}
