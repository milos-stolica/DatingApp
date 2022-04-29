using DatingApp.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppUser> Users { get; set; }

        public DbSet<UserLike> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserLike>()
                   .HasKey(ul => new { ul.SourceUserId, ul.LikedUserId });

            builder.Entity<UserLike>()
                   .HasOne(ul => ul.SourceUser)
                   .WithMany(au => au.LikedUsers)
                   .HasForeignKey(ul => ul.SourceUserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserLike>()
                   .HasOne(ul => ul.LikedUser)
                   .WithMany(ap => ap.LikedByUsers)
                   .HasForeignKey(ul => ul.LikedUserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
