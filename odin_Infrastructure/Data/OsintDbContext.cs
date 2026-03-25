using Microsoft.EntityFrameworkCore;
using odin_Domain.Entities;

namespace odin_Infrastructure.Data
{
    public class OsintDbContext(DbContextOptions<OsintDbContext> options) : DbContext(options)
    {
        public DbSet<MeshtasticNode> MeshtasticNodes { get; set; }
        public DbSet<MeshtasticMessage> MeshtasticMessages { get; set; }
        public DbSet<OsintEvent> OsintEvents { get; set; }
        public DbSet<SigintCapture> SigintCaptures { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // CRITICAL: Tells Postgress to load the spatial extension
            modelBuilder.HasPostgresExtension("postgis");

            ConfigureMeshtastic(modelBuilder);
            ConfigureOsint(modelBuilder);
            ConfigureSigint(modelBuilder);

        }

        private void ConfigureMeshtastic(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MeshtasticNode>(entity =>
            {
                entity.ToTable("meshtastic_nodes");
                entity.HasKey(e => e.NodeId);
                entity.Property(e => e.NodeId).HasMaxLength(16);
                entity.Property(e => e.LongName).HasMaxLength(50);
                entity.Property(e => e.ShortName).HasMaxLength(4);
                entity.Property(e => e.MacAddress).HasMaxLength(17);
                entity.Property(e => e.Voltage).HasColumnType("decimal(4,2)");

                // Explicit Timestamp Mapping
                entity.Property(e => e.LastHeard)
                      .HasColumnType("timestamp with time zone")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<MeshtasticMessage>(entity =>
            {
                entity.ToTable("meshtastic_messages");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SenderId).HasMaxLength(16).IsRequired();
                entity.Property(e => e.ReceiverId).HasMaxLength(16);
                entity.Property(e => e.Snr).HasColumnType("decimal(5,2)");

                // Explicit Timestamp Mapping
                entity.Property(e => e.Timestamp)
                      .HasColumnType("timestamp with time zone")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Configure the Foreign Key relationship
                entity.HasOne(m => m.Sender)
                      .WithMany(n => n.Messages)
                      .HasForeignKey(m => m.SenderId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void ConfigureOsint(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OsintEvent>(entity =>
            {
                entity.ToTable("osint_events");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SourceName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.EventCategory).HasMaxLength(50).IsRequired();

                // Store the Enum as a string in the DB for readability, rather than an int
                entity.Property(e => e.Severity).HasConversion<string>();

                // Explicitly tell Postgres to use the native JSONB type for unstructured payloads
                entity.Property(e => e.RawData).HasColumnType("jsonb");

                // Base Geometry allows for Points or Polygons
                entity.Property(e => e.Location).HasColumnType("geometry");

                entity.Property(e => e.DiscoveredAt)
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.ExpiresAt)
                    .HasColumnType("timestamp with time zone"); // Nullable, so no default value
            });
        }

        private void ConfigureSigint(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SigintCapture>(entity =>
            {
                entity.ToTable("sigint_captures");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Type).HasConversion<string>().HasMaxLength(20);
                entity.Property(e => e.Identifier).HasMaxLength(100).IsRequired();

                entity.Property(e => e.Metadata).HasColumnType("jsonb");

                // Explicit Point geometry
                entity.Property(e => e.CaptureLocation).HasColumnType("geometry(Point, 4326)");

                // Explicitly map the timestamp and let Postgres handle the default insertion time
                entity.Property(e => e.CapturedAt)
                      .HasColumnType("timestamp with time zone")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
        }
    }
}

