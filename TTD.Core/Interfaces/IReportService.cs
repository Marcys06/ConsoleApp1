using ConsoleApp1.TTD.Data;
using ConsoleApp1.TTD.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TTD.Core.Interfaces;
using TTD.Data;

namespace TTD.Core.Services
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        // ===== METODY Z INTERFEJSU =====

        public async Task GenerateTrainsReport(string outputPath)
        {
            var trains = await _context.Trains.OrderBy(t => t.Name).ToListAsync();

            using var writer = new StreamWriter(outputPath);
            writer.WriteLine("=== RAPORT POCIĄGÓW ===");
            writer.WriteLine($"Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            writer.WriteLine();

            foreach (var train in trains)
            {
                writer.WriteLine($"- {train.Name} (Model: {train.Model}, Vmax: {train.VMax} km/h, Elektryczny: {(train.IsElectric ? "Tak" : "Nie")})");
            }
        }

        public async Task GenerateRoutesReport(string outputPath)
        {
            var routes = await _context.Routes
                .Include(r => r.RouteStations)
                    .ThenInclude(rs => rs.Station)
                .OrderBy(r => r.Name)
                .ToListAsync();

            using var writer = new StreamWriter(outputPath);
            writer.WriteLine("=== RAPORT TRAS ===");
            writer.WriteLine($"Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            writer.WriteLine();

            foreach (var route in routes)
            {
                var stops = route.RouteStations?.OrderBy(rs => rs.StopOrder).ToList() ?? new();
                string stopNames = string.Join(" → ", stops.Select(s => s.Station?.Name ?? "?"));
                writer.WriteLine($"- {route.Name} (Aktywna: {(route.IsActive ? "Tak" : "Nie")})");
                writer.WriteLine($"  Trasa: {stopNames}");
                writer.WriteLine();
            }
        }

        public async Task GenerateScheduleReport(int routeId, string outputPath)
        {
            var route = await _context.Routes
                .Include(r => r.Schedules)
                    .ThenInclude(s => s.Train)
                .FirstOrDefaultAsync(r => r.Id == routeId);

            if (route == null)
            {
                throw new ArgumentException($"Trasa o ID {routeId} nie istnieje.");
            }

            using var writer = new StreamWriter(outputPath);
            writer.WriteLine($"=== RAPORT ROZKŁADU JAZDY ===");
            writer.WriteLine($"Trasa: {route.Name}");
            writer.WriteLine($"Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            writer.WriteLine();

            var schedules = route.Schedules?.OrderBy(s => s.DepartureTime).ToList() ?? new();
            foreach (var schedule in schedules)
            {
                writer.WriteLine($"- {schedule.DepartureTime:hh\\:mm} - {schedule.Train?.Name ?? "Brak pociągu"} {(schedule.IsActive ? "✅" : "❌")} {(!string.IsNullOrEmpty(schedule.Notes) ? $"({schedule.Notes})" : "")}");
            }
        }

        public async Task<DatabaseStatistics> GetStatisticsAsync()
        {
            return new DatabaseStatistics
            {
                TrainCount = await _context.Trains.CountAsync(),
                StationCount = await _context.Stations.CountAsync(),
                RouteCount = await _context.Routes.CountAsync(),
                ScheduleCount = await _context.Schedules.CountAsync(),
                ScheduleTravelTimeCount = await _context.ScheduleTravelTimes.CountAsync()
            };
        }
    }
}