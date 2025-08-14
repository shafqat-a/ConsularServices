using Microsoft.EntityFrameworkCore;
using FrameworkQ.ConsularServices.Services;
using FrameworkQ.ConsularServices.Users;

namespace FrameworkQ.ConsularServices.Data
{
    public class ConsularDbContext : DbContext
    {
        public ConsularDbContext(DbContextOptions<ConsularDbContext> options) : base(options)
        {
        }

        // DbSets for your entities
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermissionMap> RolePermissionMaps { get; set; }
        public DbSet<RoleUserMap> RoleUserMaps { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<StationLog> StationLogs { get; set; }
        public DbSet<ServiceInstance> ServiceInstances { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            
            // Configure Npgsql to use UTC for all DateTime values
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", false);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure PostgreSQL schema
            modelBuilder.HasDefaultSchema("public");

            // Configure table names explicitly
            modelBuilder.Entity<User>().ToTable("user");
            modelBuilder.Entity<Role>().ToTable("role");
            modelBuilder.Entity<Permission>().ToTable("permission");
            modelBuilder.Entity<RolePermissionMap>().ToTable("role_permission_map");
            modelBuilder.Entity<RoleUserMap>().ToTable("role_user_map");
            modelBuilder.Entity<Service>().ToTable("service_info");
            modelBuilder.Entity<Station>().ToTable("station");
            modelBuilder.Entity<Token>().ToTable("token");
            modelBuilder.Entity<StationLog>().ToTable("station_log");
            modelBuilder.Entity<ServiceInstance>().ToTable("service_instance");

            // Configure DateTime properties for PostgreSQL UTC
            modelBuilder.Entity<Token>()
                .Property(t => t.GeneratedAt)
                .HasColumnType("timestamp with time zone");
            
            modelBuilder.Entity<Token>()
                .Property(t => t.AppointmentAt)
                .HasColumnType("timestamp with time zone");
            
            modelBuilder.Entity<Token>()
                .Property(t => t.CompletedAt)
                .HasColumnType("timestamp with time zone");

            modelBuilder.Entity<ServiceInstance>()
                .Property(si => si.PaymentMadeAt)
                .HasColumnType("timestamp with time zone");
            
            modelBuilder.Entity<ServiceInstance>()
                .Property(si => si.DeliveryDate)
                .HasColumnType("timestamp with time zone");
            
            modelBuilder.Entity<ServiceInstance>()
                .Property(si => si.DeliveredAt)
                .HasColumnType("timestamp with time zone");

            modelBuilder.Entity<StationLog>()
                .Property(sl => sl.CreatedAt)
                .HasColumnType("timestamp with time zone");
            
            modelBuilder.Entity<StationLog>()
                .Property(sl => sl.StartTime)
                .HasColumnType("timestamp with time zone");
            
            modelBuilder.Entity<StationLog>()
                .Property(sl => sl.EndTime)
                .HasColumnType("timestamp with time zone");

            // Configure composite keys
            modelBuilder.Entity<RolePermissionMap>()
                .HasKey(rpm => new { rpm.RoleId, rpm.PermissionId });

            modelBuilder.Entity<RoleUserMap>()
                .HasKey(rum => new { rum.UserId, rum.RoleId });

            // Configure relationships
            modelBuilder.Entity<RolePermissionMap>()
                .HasOne<Role>()
                .WithMany()
                .HasForeignKey(rpm => rpm.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RolePermissionMap>()
                .HasOne<Permission>()
                .WithMany()
                .HasForeignKey(rpm => rpm.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RoleUserMap>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(rum => rum.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RoleUserMap>()
                .HasOne<Role>()
                .WithMany()
                .HasForeignKey(rum => rum.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure ServiceInstance relationships
            modelBuilder.Entity<ServiceInstance>()
                .HasOne<Service>()
                .WithMany()
                .HasForeignKey(si => si.ServiceInfoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ServiceInstance>()
                .HasOne<Token>()
                .WithMany()
                .HasForeignKey(si => si.TokenId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure StationLog relationships
            modelBuilder.Entity<StationLog>()
                .HasOne<Station>()
                .WithMany()
                .HasForeignKey(sl => sl.StationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StationLog>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(sl => sl.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure PostgreSQL array types
            modelBuilder.Entity<Token>()
                .Property(t => t.ServiceType)
                .HasColumnType("bigint[]");

            modelBuilder.Entity<ServiceInstance>()
                .Property(si => si.AttachmentsRecieved)
                .HasColumnType("text[]");

            // Configure identity columns
            modelBuilder.Entity<User>()
                .Property(u => u.UserId)
                .UseIdentityAlwaysColumn();

            modelBuilder.Entity<Role>()
                .Property(r => r.RoleID)
                .UseIdentityAlwaysColumn();

            modelBuilder.Entity<Service>()
                .Property(s => s.ServiceId)
                .UseIdentityAlwaysColumn();

            modelBuilder.Entity<Station>()
                .Property(s => s.StationId)
                .UseIdentityAlwaysColumn();

            modelBuilder.Entity<StationLog>()
                .Property(sl => sl.LogId)
                .UseIdentityAlwaysColumn();

            modelBuilder.Entity<ServiceInstance>()
                .Property(si => si.ServiceInstanceId)
                .UseIdentityAlwaysColumn();

            // Configure sequences for token generation
            modelBuilder.HasSequence<long>("sequence_seq", "public")
                .StartsAt(1)
                .IncrementsBy(1);

            // Seed data for permissions
            modelBuilder.Entity<Permission>().HasData(
                new Permission { PermissionID = Permission.UPDATE_USER, PermissionName = "UPDATE_USER" },
                new Permission { PermissionID = Permission.DELETE_USER, PermissionName = "DELETE_USER" },
                new Permission { PermissionID = Permission.DISABLE_USER, PermissionName = "DISABLE_USER" },
                new Permission { PermissionID = Permission.CHANGE_PASSWORD, PermissionName = "CHANGE_PASSWORD" },
                new Permission { PermissionID = Permission.UPDATE_ROLE, PermissionName = "UPDATE_ROLE" },
                new Permission { PermissionID = Permission.MODIFY_ROLE, PermissionName = "MODIFY_ROLE" },
                new Permission { PermissionID = Permission.DELETE_ROLE, PermissionName = "DELETE_ROLE" },
                new Permission { PermissionID = Permission.UPDATE_SERVICE_INFO, PermissionName = "UPDATE_SERVICE_INFO" },
                new Permission { PermissionID = Permission.CREATE_TOKEN, PermissionName = "CREATE_TOKEN" },
                new Permission { PermissionID = Permission.UPDATE_TOKEN, PermissionName = "UPDATE_TOKEN" },
                new Permission { PermissionID = Permission.UPDATE_SERVICE_INSTANCE, PermissionName = "UPDATE_SERVICE_INSTANCE" }
            );
        }
    }
}
