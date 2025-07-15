using FinTrackForWindows.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace FinTrackForWindows.Data
{
    public class AppDatabaseContext : DbContext
    {
        public AppDatabaseContext() { }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<UserSettingsModel> UserSettings { get; set; }
        public DbSet<AccountModel> Accounts { get; set; }
        public DbSet<TransactionModel> Transactions { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<BudgetModel> Budgets { get; set; }
        public DbSet<BudgetCategoryModel> BudgetCategories { get; set; }
        public DbSet<NotificationModel> Notifications { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var userProfileFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                var dbPath = Path.Combine(userProfileFolder, "FinTrackWindows.db");

                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserModel>(entity =>
            {
                entity.ToTable("Users");

                entity.HasOne(u => u.UserSettings)
                      .WithOne(s => s.User)
                      .HasForeignKey<UserSettingsModel>(s => s.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.Accounts)
                      .WithOne(a => a.User)
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.Categories)
                      .WithOne(c => c.User)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.Transactions)
                      .WithOne(t => t.User)
                      .HasForeignKey(t => t.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.Budgets)
                      .WithOne(b => b.User)
                      .HasForeignKey(b => b.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.Notifications)
                        .WithOne(n => n.User)
                        .HasForeignKey(n => n.UserId)
                        .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserSettingsModel>(entity =>
            {
                entity.ToTable("UserSettings");
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Id).ValueGeneratedOnAdd();
                entity.HasIndex(s => s.UserId).IsUnique();
            });

            modelBuilder.Entity<AccountModel>(entity =>
            {
                entity.ToTable("Accounts");
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Id).ValueGeneratedOnAdd();
                entity.Property(a => a.Balance).HasColumnType("decimal(18, 2)").IsRequired();
                entity.Property(a => a.Name).IsRequired();
                entity.HasIndex(a => new { a.UserId, a.Name }).IsUnique();
            });

            modelBuilder.Entity<TransactionModel>(entity =>
            {
                entity.ToTable("Transactions");
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Id).ValueGeneratedOnAdd();
                entity.Property(t => t.Amount).HasColumnType("decimal(18, 2)").IsRequired();
                entity.HasIndex(t => t.UserId);
                entity.HasIndex(t => t.CategoryId);
                entity.HasIndex(t => t.TransactionDateUtc);
                entity.HasIndex(t => t.AccountId);
            });

            modelBuilder.Entity<CategoryModel>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id).ValueGeneratedOnAdd();
                entity.Property(c => c.Name).HasColumnName("CategoryName").IsRequired();
                entity.Property(c => c.Type).HasColumnName("CategoryType").HasConversion<string>().IsRequired();
                entity.HasIndex(c => new { c.UserId, c.Name, c.Type, }).IsUnique();

                entity
                    .HasMany(c => c.BudgetAllocations)
                    .WithOne(bc => bc.Category)
                    .HasForeignKey(bc => bc.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasMany(c => c.Transactions)
                    .WithOne(t => t.Category)
                    .HasForeignKey(t => t.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<BudgetModel>(entity =>
            {
                entity.ToTable("Budgets");
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Id).ValueGeneratedOnAdd();
                entity.Property(b => b.Name).HasColumnName("BudgetName").IsRequired();
                entity.Property(b => b.Description).IsRequired(false);

                entity.HasMany(b => b.BudgetCategories).WithOne(bc => bc.Budget).HasForeignKey(bc => bc.BudgetId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BudgetCategoryModel>(entity =>
            {
                entity.ToTable("BudgetCategories");
                entity.HasKey(bc => bc.Id);
                entity.Property(bc => bc.Id).ValueGeneratedOnAdd();
                entity.Property(bc => bc.AllocatedAmount).HasColumnType("decimal(18, 2)").IsRequired();
                entity.HasIndex(bc => new { bc.BudgetId, bc.CategoryId }).IsUnique();
            });

            modelBuilder.Entity<NotificationModel>(entity =>
            {
                entity.ToTable("Notifications");
                entity.HasKey(n => n.NotificationId);
                entity.Property(n => n.NotificationId).ValueGeneratedOnAdd();
                entity.Property(n => n.MessageHead).IsRequired().HasMaxLength(200);
                entity.Property(n => n.MessageBody).IsRequired().HasMaxLength(1000);
                entity.Property(n => n.CreatedAtUtc).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(n => n.IsRead).IsRequired().HasDefaultValue(false);
                entity.HasIndex(n => n.UserId);
            });
        }
    }
}
