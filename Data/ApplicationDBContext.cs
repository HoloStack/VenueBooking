using Microsoft.EntityFrameworkCore;
using venueBooking.Models;
using System.Collections.Generic;

namespace venueBooking.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options)
        : base(options) { }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Booking> Bookings { get; set; }  // Added Bookings DbSet
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Many-to-many: Venue <-> EventType
            modelBuilder.Entity<Venue>()
                .HasMany(v => v.SupportedEventTypes)
                .WithMany(et => et.Venues)
                .UsingEntity<Dictionary<string, object>>(
                    "VenueEventType",
                    r => r.HasOne<EventType>().WithMany().HasForeignKey("EventTypeId"),
                    l => l.HasOne<Venue>().WithMany().HasForeignKey("VenueId"),
                    je =>
                    {
                        je.HasKey("VenueId", "EventTypeId");
                        je.ToTable("VenueEventTypes");
                    }
                );

            // Booking relationships (removed unique constraint to allow multiple bookings per event)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Event)
                .WithMany()
                .HasForeignKey(b => b.EventId);
                
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Venue)
                .WithMany()
                .HasForeignKey(b => b.VenueId);
        }
    }
}