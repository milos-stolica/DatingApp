using DatingApp.API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext : IdentityDbContext<AppUser, 
                                                 AppRole, 
                                                 int, 
                                                 IdentityUserClaim<int>, 
                                                 AppUserRole, 
                                                 IdentityUserLogin<int>, 
                                                 IdentityRoleClaim<int>, 
                                                 IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<UserLike> Likes { get; set; }

        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //configuring AppUserRole entity
            //builder.Entity<AppUserRole>()
            //       .HasKey(ar => new { ar.UserId, ar.RoleId }); - default

            builder.Entity<AppUserRole>()
                .HasOne(ur => ur.User)
                .WithMany(au => au.Roles)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
            //.OnDelete(DeleteBehavior.Cascade); - default

            builder.Entity<AppUserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(ar => ar.Users)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
                //.OnDelete(DeleteBehavior.Cascade); - default

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
