using Microsoft.EntityFrameworkCore;

namespace TeamUpAPI.Models
{
    public class TeamUpContext : DbContext
    {
        public TeamUpContext(DbContextOptions<TeamUpContext> options) : base(options)
        { }

        public DbSet<UserProfile> Users { get; set; }
        public DbSet<UserSettings> Settings { get; set; }
        public DbSet<AccessibilitySettings> AccessibilitySettings { get; set; }
        public DbSet<ContentPreferences> ContentPreferences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserProfile>()
                .HasOne(up => up.UserSettings)
                .WithOne(us => us.UserProfile)
                .HasForeignKey<UserSettings>(us => us.UserID);

            modelBuilder.Entity<UserProfile>()
                .HasOne(up => up.Accessibility)
                .WithOne(a => a.UserProfile)
                .HasForeignKey<AccessibilitySettings>(a => a.UserID);

            modelBuilder.Entity<UserProfile>()
                .HasOne(up => up.ContentPreferences)
                .WithOne(cp => cp.UserProfile)
                .HasForeignKey<ContentPreferences>(cp => cp.UserID);

            base.OnModelCreating(modelBuilder);
        }
    }
}
