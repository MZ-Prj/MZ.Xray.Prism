using Microsoft.EntityFrameworkCore;
using MZ.Domain.Entities;

namespace MZ.Infrastructure
{
    public class DbConfig : DbContext
    {
        public static string Root = @"Data Source=./instance.db";

        public DbSet<UserEntity> User { get; set; }

        public DbConfig() { }
        public DbConfig(DbContextOptions<DbConfig> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(Root);
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Username)
                      .IsRequired()
                      .HasMaxLength(4);
                entity.Property(u => u.Email)
                      .IsRequired();
                entity.ToTable("User");
            });

        }

        public void OnCommand(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>().ToTable("User");
            base.OnModelCreating(modelBuilder);
        }
    }
}
