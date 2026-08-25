using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TTD.Data;
using TTD.Data.Models;
using TTD.Core.Interfaces;

namespace TTD.Core.Services
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Train>> GetAllTrainsAsync()
        {
            return await _context.Trains
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Route>> GetAllRoutesAsync()
        {
            return await _context.Routes
                .Include(r => r.RouteStations)
                    .ThenInclude(rs => rs.Station)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Schedule>> GetScheduleForRouteAsync(int routeId)
        {
            return await _context.Schedules
                .Where(s => s.RouteId == routeId)
                .Include(s => s.Train)
                .Include(s => s.Route)
                .Include(s => s.TravelTimes)
                    .ThenInclude(tt => tt.RouteStation)
                        .ThenInclude(rs => rs.Station)
                .OrderBy(s => s.DepartureTime)
                .ToListAsync();
        }

        public async Task ExportToCsvAsync<T>(IEnumerable<T> data, string filePath)
        {
            try
            {
                var properties = typeof(T).GetProperties();
                using var writer = new StreamWriter(filePath);
                
                // Nagłówki
                writer.WriteLine(string.Join(",", properties.Select(p => p.Name)));
                
                // Dane
                foreach (var item in data)
                {
                    var values = properties.Select(p => p.GetValue(item)?.ToString() ?? "");
                    writer.WriteLine(string.Join(",", values));
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Błąd eksportu do CSV: {ex.Message}", ex);
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