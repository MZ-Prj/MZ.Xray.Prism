using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MZ.Domain.Entities;
using MZ.Domain.Enums;
using System;
using System.IO;

namespace MZ.Infrastructure
{
    /// <summary>
    /// EF Core 마이그레이션에서 AppDbContext 생성
    /// </summary>
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        /// <summary>
        /// DbContext를 생성
        /// </summary>
        public AppDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var dbPath = configuration.GetSection("Database:Path").Value;

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite(dbPath);

            return new AppDbContext(optionsBuilder.Options);
        }
    }

    /// <summary>
    /// 애플리케이션의 메인 DB 컨텍스트 (EF Core)  
    /// - 모든 도메인 엔티티의 DB 매핑, 관계, 제약조건 설정  
    /// - SaveChanges시 자동 타임스탬프 관리
    /// </summary>
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

            // User 
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

                // 1:N Zeffect
                entity.HasMany(u => u.Zeffect)
                    .WithOne(m => m.User)
                    .HasForeignKey(m => m.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // UserSetting
            modelBuilder.Entity<UserSettingEntity>(entity =>
            {
                entity.HasKey(s => s.Id);

                entity.Property(s => s.Theme).IsRequired();
                entity.Property(s => s.Language).IsRequired();

                // 1:1 UserEntity 
                entity.HasOne(s => s.User)
                    .WithOne(u => u.UserSetting)
                    .HasForeignKey<UserSettingEntity>(s => s.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 1:N UserButtonEntity 
                entity.HasMany(s => s.Buttons)
                    .WithOne(b => b.UserSetting)
                    .HasForeignKey(b => b.UserSettingId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            //UserSetting - Button
            modelBuilder.Entity<UserButtonEntity>(entity =>
            {
                entity.HasKey(b => b.Id);

                entity.Property(b => b.Name).IsRequired();
                entity.Property(b => b.IsVisibility).IsRequired();
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

            // MaterialControl
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

        /// <summary>
        /// SaveChanges 오버라이드  
        /// - UserEntity의 생성/수정 시간 자동 업데이트  
        /// </summary>
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