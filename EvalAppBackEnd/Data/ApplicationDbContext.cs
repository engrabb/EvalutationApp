using EvalAppBackEnd.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace EvalAppBackEnd.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<VideoComment> VideoComments { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Playlist - CreatedBy relationship
            builder.Entity<Playlist>()
                .HasOne(p => p.CreatedBy)
                .WithMany()
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            // VideoComment - User relationship
            builder.Entity<VideoComment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Video - Playlist relationship
            builder.Entity<Video>()
                .HasOne(v => v.Playlist)
                .WithMany(p => p.Videos)
                .HasForeignKey(v => v.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade);

            // VideoComment - Video relationship
            builder.Entity<VideoComment>()
                .HasOne(vc => vc.Video)
                .WithMany(v => v.Comments)
                .HasForeignKey(vc => vc.VideoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
