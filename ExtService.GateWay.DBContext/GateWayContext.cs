using ExtService.GateWay.DBContext.DBFunctions.PGSQLAssignableFunctions;
using ExtService.GateWay.DBContext.DBModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ExtService.GateWay.DBContext
{
    public class GateWayContext : DbContext
    {
        public GateWayContext() : base()
        {
        }

        public GateWayContext(DbContextOptions<GateWayContext> options) : base(options)
        {
        }

        public DbSet<DBModels.MethodInfo> MethodInfoSet { get; set; }
        public DbSet<MethodHeaders> MethodHeadersSet { get; set; }
        public DbSet<Plugins> PluginsSet { get; set; }
        public DbSet<PluginParameters> PluginParametersSet { get; set; }
        public DbSet<PluginLinks> PluginLinksSet { get; set; }
        public DbSet<SubMethodInfo> SubMethodInfoSet { get; set; }
        public DbSet<SystemInfo> SystemInfoSet { get; set; }
        public DbSet<UserInfo> Users { get; set; }
        public DbSet<Identification> IdentificationSet { get; set; }
        public DbSet<BillingConfig> BillingConfigSet { get; set; }
        public DbSet<Billing> BillingSet { get; set; }
        public DbSet<NotificationInfo> NotificationInfoSet { get; set; }
        public DbSet<TransactionLog> TransactionLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var config = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();

                var connectionString = config.GetConnectionString("DefaultConnection");
                optionsBuilder.UseNpgsql(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresExtension("uuid-ossp");
            modelBuilder.HasPostgresExtension("pgcrypto");

            modelBuilder.Entity<DBModels.MethodInfo>(entity =>
            {
                entity.Property(m => m.MethodId)
                    .HasDefaultValueSql("uuid_generate_v4()")
                    .IsRequired();
                entity.HasMany(m => m.BillingSet)
                    .WithOne(b => b.Method)
                    .HasForeignKey(b => b.MethodId);
                entity.HasMany(m => m.BillingConfigSet)
                    .WithOne(b => b.Method)
                    .HasForeignKey(b => b.MethodId);
                entity.HasMany(m => m.SubMethodInfoSet)
                    .WithOne(s => s.Method)
                    .HasForeignKey(s => s.MethodId);
                entity.HasMany(m => m.MethodHeaders)
                    .WithOne(h => h.Method)
                    .HasForeignKey(h => h.MethodId);
            });

            modelBuilder.Entity<SubMethodInfo>(entity =>
            {
                entity.Property(s => s.SubMethodId)
                    .HasDefaultValueSql("uuid_generate_v4()")
                    .IsRequired();
                entity.HasOne(s => s.Method)
                    .WithMany(m => m.SubMethodInfoSet)
                    .HasForeignKey(s => s.MethodId);
            });

            modelBuilder.Entity<MethodHeaders>(entity =>
            {
                entity.Property(h => h.MethodHeaderId)
                    .HasDefaultValueSql("uuid_generate_v4()")
                    .IsRequired();
                entity.HasOne(h => h.Method)
                    .WithMany(m => m.MethodHeaders)
                    .HasForeignKey(h => h.MethodId);
                entity.HasOne(h => h.Plugin)
                    .WithMany(p => p.MethodHeaders)
                    .HasForeignKey(h => h.PluginId);
                entity.HasMany(h => h.PluginLinks)
                    .WithOne(l => l.MethodHeader)
                    .HasForeignKey(l => l.MethodHeaderId);
            });

            modelBuilder.Entity<Plugins>(entity =>
            {
                entity.Property(p => p.PluginId)
                    .HasDefaultValueSql("uuid_generate_v4()")
                    .IsRequired();
                entity.HasMany(p => p.MethodHeaders)
                    .WithOne(h => h.Plugin)
                    .HasForeignKey(h => h.PluginId);
                entity.HasMany(p => p.PluginLinks)
                    .WithOne(l => l.Plugin)
                    .HasForeignKey(l => l.PluginId);
            });

            modelBuilder.Entity<PluginParameters>(entity =>
            {
                entity.Property(p => p.ParameterId)
                    .HasDefaultValueSql("uuid_generate_v4()")
                    .IsRequired();
                entity.HasMany(p => p.PluginLinks)
                    .WithOne(l => l.Parameter)
                    .HasForeignKey(l => l.ParameterId);
            });

            modelBuilder.Entity<PluginLinks>(entity =>
            {
                entity.Property(l => l.LinkId)
                    .HasDefaultValueSql("uuid_generate_v4()")
                    .IsRequired();
                entity.HasOne(l => l.Plugin)
                    .WithMany(p => p.PluginLinks)
                    .HasForeignKey(l => l.PluginId);
                entity.HasOne(l => l.MethodHeader)
                    .WithMany(h => h.PluginLinks)
                    .HasForeignKey(l => l.MethodHeaderId);
                entity.HasOne(l => l.Parameter)
                    .WithMany(p => p.PluginLinks)
                    .HasForeignKey(l => l.ParameterId);
            });

            modelBuilder.Entity<SystemInfo>(entity =>
            {
                entity.Property(s => s.SystemId)
                    .HasDefaultValueSql("uuid_generate_v4()")
                    .IsRequired();
                entity.HasMany(s => s.Users)
                    .WithOne(u => u.SystemInfo)
                    .HasForeignKey(u => u.SystemId);
                entity.HasMany(s => s.IdentificationSet)
                    .WithOne(i => i.SystemInfo)
                    .HasForeignKey(i => i.SystemId);
            });

            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.Property(u => u.UserId)
                    .HasDefaultValueSql("uuid_generate_v4()")
                    .IsRequired();
                entity.HasOne(u => u.SystemInfo)
                    .WithMany(s => s.Users)
                    .HasForeignKey(u => u.SystemId);
            });

            modelBuilder.Entity<Identification>(entity =>
            {
                entity.Property(i => i.IdentificationId)
                    .HasDefaultValueSql("uuid_generate_v4()")
                    .IsRequired();
                entity.HasOne(i => i.SystemInfo)
                    .WithMany(s => s.IdentificationSet)
                    .HasForeignKey(i => i.SystemId);
                entity.HasMany(i => i.BillingSet)
                    .WithOne(b => b.Identification)
                    .HasForeignKey(b => b.IdentificationId);
                entity.HasMany(i => i.BillingConfigSet)
                    .WithOne(b => b.Identification)
                    .HasForeignKey(b => b.IdentificationId);
            });

            modelBuilder.Entity<BillingConfig>(entity =>
            {
                entity.Property(b => b.BillingConfigId)
                    .HasDefaultValueSql("uuid_generate_v4()")
                    .IsRequired();
                entity.HasCheckConstraint("CK_PeriodInDays_Greater_Than_Zero", "\"PeriodInDays\" > 0");
                entity.HasCheckConstraint("CK_RequestLimitPerPeriod_Greater_Than_Zero", "\"RequestLimitPerPeriod\" > 0");
                entity.HasCheckConstraint("CK_StartDate_Less_Than_EndDate", "\"StartDate\" < \"EndDate\"");
                entity.HasOne(b => b.Identification)
                    .WithMany(i => i.BillingConfigSet)
                    .HasForeignKey(b => b.IdentificationId);
                entity.HasOne(b => b.Method)
                    .WithMany(m => m.BillingConfigSet)
                    .HasForeignKey(b => b.MethodId);
                entity.HasMany(b => b.NotificationInfoSet)
                    .WithOne(n => n.BillingConfig)
                    .HasForeignKey(n => n.BillingConfigId);
                entity.HasMany(b => b.BillingRecords)
                    .WithOne(b => b.BillingConfig)
                    .HasForeignKey(b => b.BillingConfigId);
            });

            modelBuilder.Entity<Billing>(entity =>
            {
                entity.Property(b => b.BillingId)
                    .HasDefaultValueSql("uuid_generate_v4()")
                    .IsRequired();
                entity.HasCheckConstraint("CK_RequestLimit_Greater_Than_Zero", "\"RequestLimit\" > 0");
                entity.HasCheckConstraint("CK_RequestCount_GTE_Zero", "\"RequestCount\" >= 0");
                entity.HasCheckConstraint("CK_StartDate_Less_Than_EndDate", "\"StartDate\" < \"EndDate\"");
                entity.HasOne(b => b.Identification)
                    .WithMany(i => i.BillingSet)
                    .HasForeignKey(b => b.IdentificationId);
                entity.HasOne(b => b.Method)
                    .WithMany(m => m.BillingSet)
                    .HasForeignKey(b => b.MethodId);
                entity.HasOne(b => b.BillingConfig)
                    .WithMany(b => b.BillingRecords)
                    .HasForeignKey(b => b.BillingConfigId);
                entity.HasMany(b => b.NotificationInfoSet)
                    .WithOne(n => n.Billing)
                    .HasForeignKey(n => n.BillingId);
                entity.HasAlternateKey(b => new { b.BillingConfigId, b.StartDate, b.EndDate });
            });

            modelBuilder.Entity<NotificationInfo>(entity =>
            {
                entity.Property(n => n.NotificationId)
                    .HasDefaultValueSql("uuid_generate_v4()")
                    .IsRequired();
                entity.HasCheckConstraint("CK_NotificationLimitPercentage_Range", "\"NotificationLimitPercentage\" > 0 AND \"NotificationLimitPercentage\" < 100");
                entity.HasOne(n => n.BillingConfig)
                    .WithMany(b => b.NotificationInfoSet)
                    .HasForeignKey(n => n.BillingConfigId);
                entity.HasOne(n => n.Billing)
                    .WithMany(b => b.NotificationInfoSet)
                    .HasForeignKey(n => n.BillingId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .IsRequired(false);
            });

            modelBuilder.Entity<TransactionLog>(entity =>
            {
                entity.Property(t => t.TransactionLogId)
                    .HasDefaultValueSql("uuid_generate_v4()")
                    .IsRequired();
            });

            modelBuilder.HasDbFunction(typeof(DecryptPluginParameterValueFunction).GetMethod("Execute"))
                .HasSchema(DecryptPluginParameterValueFunction.Schema)
                .HasName(DecryptPluginParameterValueFunction.FunctionName);
        }

    }
}
