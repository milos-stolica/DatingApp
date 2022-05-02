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

        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //configuring UserLike entity
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

            //configuring Message entity
            builder.Entity<Message>()
                   .HasOne(m => m.Sender)
                   .WithMany(au => au.MessagesSent)
                   .HasForeignKey(m => m.SenderId) //this may be unnecessary
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                   .HasOne(m => m.Recipient)
                   .WithMany(au => au.MessagesReceived)
                   .HasForeignKey(m => m.RecipientId)//this may be unnecessary
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
