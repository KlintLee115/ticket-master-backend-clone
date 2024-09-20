using Microsoft.EntityFrameworkCore;
using events_tickets_management_backend.Models;

namespace events_tickets_management_backend.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public virtual DbSet<Artist> Artists { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Location> Locations { get; set; }
    public virtual DbSet<Ticket> Tickets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=mydatabase;Username=postgres;Password=root;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Artist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Artist_pkey");

            entity.ToTable("Artist");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Event_pkey");

            entity.ToTable("Event");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ArtistId).HasColumnName("artistId");
            entity.Property(e => e.BeginDatetime)
                .HasPrecision(0)
                .HasColumnName("begin_datetime");
            entity.Property(e => e.EndDatetime)
                .HasPrecision(0)
                .HasColumnName("end_datetime");
            entity.Property(e => e.LocationId).HasColumnName("locationId");
            entity.Property(e => e.Title).HasColumnName("title");

            entity.HasOne(d => d.Artist).WithMany(p => p.Events)
                .HasForeignKey(d => d.ArtistId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("Event_artistId_fkey");

            entity.HasOne(d => d.Location).WithMany(p => p.Events)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("Event_locationId_fkey");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Location_pkey");

            entity.ToTable("Location");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address).HasColumnName("address");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => new { e.EventId, e.RowNumber, e.SectionNumber, e.SeatNumber }).HasName("tickets_eventid_pkey");

            entity.ToTable("tickets");

            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.RowNumber).HasColumnName("row_number");
            entity.Property(e => e.SectionNumber).HasColumnName("section_number");
            entity.Property(e => e.SeatNumber).HasColumnName("seat_number");
            entity.Property(e => e.Buyeremail).HasColumnName("buyeremail");
            entity.Property(e => e.Isbought)
                .HasDefaultValue(false)
                .HasColumnName("isbought");
            entity.Property(e => e.Price)
                .HasPrecision(7, 2)
                .HasColumnName("price");
            entity.Property(e => e.PurchasedTime)
                .HasPrecision(6)
                .HasColumnName("purchased_time");
        });
    }
}
