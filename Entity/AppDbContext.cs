using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Entity
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        // User Related
        public virtual DbSet<AppUser> AppUsers { get; set; }
        public virtual DbSet<AppRole> AppRoles { get; set; }
        public virtual DbSet<Music> Musics { get; set; }
        public virtual DbSet<MusicCategory> MusicCategories { get; set; }
        public virtual DbSet<MusicCategoryLink> MusicCategoryLinks { get; set; }
        public virtual DbSet<MovieCategory> MovieCategories { get; set; }
        public virtual DbSet<MovieCategoryLink> MovieCategoryLinks { get; set; }
        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<Playlist> Playlists { get; set; }

        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Identity table names
            modelBuilder.Entity<AppUser>().ToTable("asp_net_users");
            modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("asp_net_user_tokens");
            modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("asp_net_user_logins");
            modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("asp_net_user_claims");
            modelBuilder.Entity<AppRole>().ToTable("asp_net_roles");
            modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("asp_net_user_roles");
            modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("asp_net_role_claims");

            // Configure many-to-many relationship between MovieCategory and MusicCategory
            modelBuilder.Entity<MovieCategory>()
                .HasMany(mc => mc.SuggestedMusicCategories)
                .WithMany(m => m.MovieCategories)
                .UsingEntity<Dictionary<string, object>>(
                    "MovieCategoryMusicCategory",
                    j => j.HasOne<MusicCategory>().WithMany().HasForeignKey("MusicCategoryId"),
                    j => j.HasOne<MovieCategory>().WithMany().HasForeignKey("MovieCategoryId"),
                    j =>
                    {
                        j.HasKey("MovieCategoryId", "MusicCategoryId");
                        j.ToTable("movie_category_music_category");
                    });

            // Configure Playlist relationships
            modelBuilder.Entity<Playlist>()
                .HasOne(p => p.Movie)
                .WithMany()
                .HasForeignKey(p => p.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Playlist>()
                .HasMany(p => p.Musics)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "PlaylistMusic",
                    j => j.HasOne<Music>().WithMany().HasForeignKey("MusicId"),
                    j => j.HasOne<Playlist>().WithMany().HasForeignKey("PlaylistId"),
                    j =>
                    {
                        j.HasKey("PlaylistId", "MusicId");
                        j.ToTable("playlist_musics");
                    });
        }
    }
}