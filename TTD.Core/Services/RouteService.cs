using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TTD.Data;
using TTD.Data.Models;
using TTD.Core.Interfaces;

namespace TTD.Core.Services
{
    public class RouteService : IRouteService
    {
        private readonly AppDbContext _context;

        public RouteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Route>> GetAllRoutesAsync()
        {
            return await _context.Routes
                .Include(r => r.RouteStations)
                    .ThenInclude(rs => rs.Station)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<Route?> GetRouteByIdAsync(int id)
        {
            return await _context.Routes
                .Include(r => r.RouteStations)
                    .ThenInclude(rs => rs.Station)
                .Include(r => r.Schedules)
                    .ThenInclude(s => s.Train)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Route> AddRouteAsync(Route route)
        {
            if (route == null)
                throw new ArgumentNullException(nameof(route));

            await _context.Routes.AddAsync(route);
            await _context.SaveChangesAsync();
            return route;
        }

        public async Task<Route> UpdateRouteAsync(Route route)
        {
            if (route == null)
                throw new ArgumentNullException(nameof(route));

            var existingRoute = await _context.Routes.FindAsync(route.Id);
            if (existingRoute == null)
                throw new ArgumentException($"Route with ID {route.Id} not found.");

            existingRoute.Name = route.Name;
            existingRoute.IsActive = route.IsActive;
            existingRoute.Notes = route.Notes;

            _context.Routes.Update(existingRoute);
            await _context.SaveChangesAsync();
            return existingRoute;
        }

        public async Task<bool> DeleteRouteAsync(int id)
        {
            var route = await _context.Routes
                .Include(r => r.RouteStations)
                .Include(r => r.Schedules)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null)
                return false;

            if (route.Schedules != null && route.Schedules.Any())
                throw new InvalidOperationException("Cannot delete route with assigned schedules.");

            _context.Routes.Remove(route);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Route>> GetActiveRoutesAsync()
        {
            return await _context.Routes
                .Where(r => r.IsActive)
                .Include(r => r.RouteStations)
                    .ThenInclude(rs => rs.Station)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Route>> GetRoutesByStationAsync(int stationId)
        {
            return await _context.Routes
                .Where(r => r.RouteStations.Any(rs => rs.StationId == stationId))
                .Include(r => r.RouteStations)
                    .ThenInclude(rs => rs.Station)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<bool> RouteExistsAsync(int id)
        {
            return await _context.Routes.AnyAsync(r => r.Id == id);
        }
    }
}