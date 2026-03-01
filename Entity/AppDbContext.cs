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
        public virtual DbSet<MovieCategory> MovieCategories { get; set; }
        public virtual DbSet<Movie> Movies { get; set; }

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
        }
    }
}