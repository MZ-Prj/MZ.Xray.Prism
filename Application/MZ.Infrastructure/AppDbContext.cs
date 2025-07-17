using Microsoft.EntityFrameworkCore;
using MZ.Domain.Entities;
using MZ.Domain.Enums;
using System;

namespace MZ.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // AppSetting
            modelBuilder.Entity<AppSettingEntity>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.IsUsernameSave).HasDefaultValue(false);
            });

            // 1:1 User 
            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Username)
                    .IsRequired()
                    .HasMaxLength(4);
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(255);
                entity.Property(u => u.Role).HasDefaultValue(UserRole.User);
                entity.Property(u => u.CreateDate).HasDefaultValueSql("GETDATE()");
                entity.Property(u => u.LastLoginDate).HasDefaultValueSql("GETDATE()");

                // 1:1 UserSetting
                entity.HasOne(u => u.UserSetting)
                    .WithOne(s => s.User)
                    .HasForeignKey<UserSettingEntity>(s => s.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 1:1 Calibration
                entity.HasOne(u => u.Calibration)
                    .WithOne(c => c.User)
                    .HasForeignKey<CalibrationEntity>(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 1:1 Filter
                entity.HasOne(u => u.Filter)
                    .WithOne(f => f.User)
                    .HasForeignKey<FilterEntity>(f => f.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 1:1 Material
                entity.HasOne(u => u.Material)
                    .WithOne(m => m.User)
                    .HasForeignKey<MaterialEntity>(m => m.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // UserSetting
            modelBuilder.Entity<UserSettingEntity>(entity =>
            {
                entity.HasKey(s => s.UserId);
            });

            // Calibration
            modelBuilder.Entity<CalibrationEntity>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.HasOne(c => c.User)
                    .WithOne(u => u.Calibration)
                    .HasForeignKey<CalibrationEntity>(c => c.UserId);
            });

            // Image
            modelBuilder.Entity<ImageEntity>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.Property(i => i.CreateDate).HasDefaultValueSql("GETDATE()");

                // 1:N ObjectDetection
                entity.HasMany(i => i.ObjectDetections)
                    .WithOne(o => o.Image)
                    .HasForeignKey(o => o.ImageId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Filter
            modelBuilder.Entity<FilterEntity>(entity =>
            {
                entity.HasKey(f => f.Id);
                entity.HasOne(f => f.User)
                    .WithOne(u => u.Filter)
                    .HasForeignKey<FilterEntity>(f => f.UserId);
            });

            // Material
            modelBuilder.Entity<MaterialEntity>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.HasOne(m => m.User)
                    .WithOne(u => u.Material)
                    .HasForeignKey<MaterialEntity>(m => m.UserId);

                // 1:N MaterialCurve
                entity.HasMany(m => m.MaterialControls)
                    .WithOne(c => c.Material)
                    .HasForeignKey(c => c.MaterialId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // MaterialCurve
            modelBuilder.Entity<MaterialControlEntity>(entity =>
            {
                entity.HasKey(m => m.Id);
            });

            // Zeffect
            modelBuilder.Entity<ZeffectControlEntity>(entity =>
            {
                entity.HasKey(z => z.Id);
                entity.Property(z => z.Check).HasDefaultValue(false);
            });

            // AIOption
            modelBuilder.Entity<AIOptionEntity>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.CreateDate).HasDefaultValueSql("GETDATE()");
                entity.Property(a => a.Cuda).HasDefaultValue(false);
                entity.Property(a => a.PrimeGpu).HasDefaultValue(false);
                entity.Property(a => a.IsChecked).HasDefaultValue(false);

                // 1:N  Category
                entity.HasMany(a => a.Categories)
                    .WithOne(c => c.AIOption)
                    .HasForeignKey(c => c.AIOptionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Category
            modelBuilder.Entity<CategoryEntity>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.IsUsing).HasDefaultValue(true);
            });

            // ObjectDetection
            modelBuilder.Entity<ObjectDetectionEntity>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.CreateDate).HasDefaultValueSql("GETDATE()");
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
                            user.CreateDate = DateTime.UtcNow;
                            break;
                        case EntityState.Modified:
                            user.LastLoginDate = DateTime.UtcNow;
                            break;
                    }
                }
            }

            return base.SaveChanges();
        }
    }
}