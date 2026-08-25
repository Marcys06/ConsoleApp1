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
    public class StationService : IStationService
    {
        private readonly AppDbContext _context;

        public StationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Station>> GetAllStationsAsync()
        {
            return await _context.Stations
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<Station?> GetStationByIdAsync(int id)
        {
            return await _context.Stations
                .Include(s => s.RouteStations)
                    .ThenInclude(rs => rs.Route)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Station> AddStationAsync(Station station)
        {
            if (station == null)
                throw new ArgumentNullException(nameof(station));

            await _context.Stations.AddAsync(station);
            await _context.SaveChangesAsync();
            return station;
        }

        public async Task<Station> UpdateStationAsync(Station station)
        {
            if (station == null)
                throw new ArgumentNullException(nameof(station));

            var existingStation = await _context.Stations.FindAsync(station.Id);
            if (existingStation == null)
                throw new ArgumentException($"Station with ID {station.Id} not found.");

            existingStation.Name = station.Name;
            existingStation.Latitude = station.Latitude;
            existingStation.Longitude = station.Longitude;
            existingStation.MapX = station.MapX;
            existingStation.MapY = station.MapY;
            existingStation.IsPassenger = station.IsPassenger;
            existingStation.IsCargo = station.IsCargo;

            _context.Stations.Update(existingStation);
            await _context.SaveChangesAsync();
            return existingStation;
        }

        public async Task<bool> DeleteStationAsync(int id)
        {
            var station = await _context.Stations
                .Include(s => s.RouteStations)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (station == null)
                return false;

            if (station.RouteStations != null && station.RouteStations.Any())
                throw new InvalidOperationException("Cannot delete station that is part of a route.");

            _context.Stations.Remove(station);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Station>> SearchStationsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllStationsAsync();

            return await _context.Stations
                .Where(s => s.Name.Contains(searchTerm))
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Station>> GetStationsByTypeAsync(bool isPassenger, bool isCargo)
        {
            return await _context.Stations
                .Where(s => s.IsPassenger == isPassenger || s.IsCargo == isCargo)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<bool> StationExistsAsync(int id)
        {
            return await _context.Stations.AnyAsync(s => s.Id == id);
        }
    }
}