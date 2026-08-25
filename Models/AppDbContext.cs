using Microsoft.EntityFrameworkCore;
using TTD.Data.Models;

namespace TTD.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Train> Trains { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<RouteStation> RouteStations { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<ScheduleTravelTime> ScheduleTravelTimes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== RouteStation =====
            // Id jest kluczem głównym
            modelBuilder.Entity<RouteStation>()
                .HasKey(rs => rs.Id);

            // Unikalność złożenia RouteId + StationId + StopOrder
            modelBuilder.Entity<RouteStation>()
                .HasIndex(rs => new { rs.RouteId, rs.StationId, rs.StopOrder })
                .IsUnique()
                .HasDatabaseName("IX_RouteStation_Unique");

            // ===== RELACJE =====

            // Route -> RouteStation
            modelBuilder.Entity<RouteStation>()
                .HasOne(rs => rs.Route)
                .WithMany(r => r.RouteStations)
                .HasForeignKey(rs => rs.RouteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Station -> RouteStation
            modelBuilder.Entity<RouteStation>()
                .HasOne(rs => rs.Station)
                .WithMany(s => s.RouteStations)
                .HasForeignKey(rs => rs.StationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Route -> Schedule
            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.Route)
                .WithMany(r => r.Schedules)
                .HasForeignKey(s => s.RouteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Train -> Schedule
            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.Train)
                .WithMany(t => t.Schedules)
                .HasForeignKey(s => s.TrainId)
                .OnDelete(DeleteBehavior.Restrict);

            // Schedule -> ScheduleTravelTime
            modelBuilder.Entity<ScheduleTravelTime>()
                .HasOne(st => st.Schedule)
                .WithMany(s => s.TravelTimes)
                .HasForeignKey(st => st.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            // RouteStation -> ScheduleTravelTime (poprawione!)
            modelBuilder.Entity<ScheduleTravelTime>()
                .HasOne(st => st.RouteStation)
                .WithMany(rs => rs.ScheduleTravelTimes)
                .HasForeignKey(st => st.RouteStationId)  // ← teraz celuje w RouteStation.Id
                .OnDelete(DeleteBehavior.Cascade);

            // ===== INDEKSY =====

            modelBuilder.Entity<Schedule>()
                .HasIndex(s => new { s.RouteId, s.DepartureTime })
                .IsUnique()
                .HasDatabaseName("IX_Schedule_Route_Departure");

            modelBuilder.Entity<Schedule>()
                .HasIndex(s => s.TrainId)
                .HasDatabaseName("IX_Schedule_TrainId");

            modelBuilder.Entity<RouteStation>()
                .HasIndex(rs => rs.StopOrder)
                .HasDatabaseName("IX_RouteStation_StopOrder");
        }
    }
}