using Microsoft.EntityFrameworkCore;
using MZ.Domain.Entities;
using System;

namespace MZ.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User
            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Username)
                      .IsRequired();
                entity.Property(u => u.PasswordHash)
                      .IsRequired();
                entity.Property(u => u.Email)
                      .IsRequired();
                entity.ToTable("User");

                // 1:1 UserSetting
                entity.HasOne(u => u.UserSetting)
                      .WithOne(s => s.User)
                      .HasForeignKey<UserSettingEntity>(s => s.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // UserSetting
            modelBuilder.Entity<UserSettingEntity>(entity =>
            {
                entity.HasKey(u => u.UserId);
                entity.ToTable("UserSetting");
            });

            // AppSetting
            modelBuilder.Entity<AppSettingEntity>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.ToTable("AppSetting");
            });

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries();

            foreach (var entry in entries)
            {
                if (entry.Entity is UserEntity user)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            user.CreateDate = DateTime.Now;
                            break;
                        case EntityState.Modified:
                            user.LastLoginDate = DateTime.Now;
                            break;
                    }
                }
            }

            return base.SaveChanges();
        }
    }
}