using ExtService.GateWay.API.Models.DBModels;
using Microsoft.EntityFrameworkCore;

namespace ExtService.GateWay.API.Utilities.DBUtils
{
    public class GateWayContext : DbContext
    {
        public GateWayContext(DbContextOptions<GateWayContext> options) : base(options)
        {
        }
        public DbSet<MethodInfo> MethodInfoSet { get; set; }
        public DbSet<SystemInfo> SystemInfoSet { get; set; }
        public DbSet<UserInfo> Users { get; set; }
        public DbSet<Identification> IdentificationSet { get; set; }
        public DbSet<Billing> BillingSet { get; set; }
        public DbSet<NotificationInfo> NotificationInfoSet { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MethodInfo>(entity =>
            {
                entity.ToTable("MethodInfo");
                entity.HasKey(m => m.MethodId);
                entity.Property(m => m.MethodName).IsRequired();
                entity.HasMany(m => m.BillingSet)
                    .WithOne(b => b.Method)
                    .HasForeignKey(b => b.MethodId);
            });

            modelBuilder.Entity<SystemInfo>(entity =>
            {
                entity.ToTable("SystemInfo");
                entity.HasKey(s => s.SystemId);
                entity.HasMany(s => s.Users)
                    .WithOne(u => u.SystemInfo)
                    .HasForeignKey(u => u.SystemId);
                entity.HasMany(s => s.IdentificationSet)
                    .WithOne(i => i.SystemInfo)
                    .HasForeignKey(i => i.SystemId);
                entity.HasMany(s => s.NotificationInfoSet)
                    .WithOne(n => n.SystemInfo)
                    .HasForeignKey(n => n.SystemId);
            });

            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.ToTable("UserInfo");
                entity.HasKey(u => u.UserId);
                entity.HasOne(u => u.SystemInfo)
                    .WithMany(s => s.Users)
                    .HasForeignKey(u => u.SystemId);
            });

            modelBuilder.Entity<Identification>(entity =>
            {
                entity.ToTable("Identification");
                entity.HasKey(i => i.IdentificationId);
                entity.HasOne(i => i.SystemInfo)
                    .WithMany(s => s.IdentificationSet)
                    .HasForeignKey(i => i.SystemId);
                entity.HasMany(i => i.BillingSet)
                    .WithOne(b => b.Identification)
                    .HasForeignKey(b => b.IdentificationId);
            });

            modelBuilder.Entity<Billing>(entity =>
            {
                entity.ToTable("Billing");
                entity.HasKey(b => b.BillingId);
                entity.HasOne(b => b.Identification)
                    .WithMany(i => i.BillingSet)
                    .HasForeignKey(b => b.IdentificationId);
                entity.HasOne(b => b.Method)
                    .WithMany(m => m.BillingSet)
                    .HasForeignKey(b => b.MethodId);
                entity.HasMany(b => b.NotificationInfoSet)
                    .WithOne(n => n.Billing)
                    .HasForeignKey(n => n.BillingId);
            });

            modelBuilder.Entity<NotificationInfo>(entity =>
            {
                entity.ToTable("NotificationInfo");
                entity.HasKey(n => n.NotificationId);
                entity.HasOne(n => n.SystemInfo)
                    .WithMany(s => s.NotificationInfoSet)
                    .HasForeignKey(n => n.SystemId);
                entity.HasOne(n => n.Billing)
                    .WithMany(b => b.NotificationInfoSet)
                    .HasForeignKey(n => n.BillingId);
            });
        }

    }
}
