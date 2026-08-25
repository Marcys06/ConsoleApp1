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
    public class ScheduleService : IScheduleService
    {
        private readonly AppDbContext _context;

        public ScheduleService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Schedule>> GetAllSchedulesAsync()
        {
            var schedules = await _context.Schedules
                .Include(s => s.Route)
                .Include(s => s.Train)
                .Include(s => s.TravelTimes)
                    .ThenInclude(tt => tt.RouteStation)
                        .ThenInclude(rs => rs.Station)
                .ToListAsync();

            // Sortowanie po pobraniu danych (LINQ to Objects)
            return schedules.OrderBy(s => s.DepartureTime);
        }
        public async Task<Schedule?> GetScheduleByIdAsync(int id)
        {
            return await _context.Schedules
                .Include(s => s.Route)
                .Include(s => s.Train)
                .Include(s => s.TravelTimes)
                    .ThenInclude(tt => tt.RouteStation)
                        .ThenInclude(rs => rs.Station)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Schedule> AddScheduleAsync(Schedule schedule)
        {
            if (schedule == null)
                throw new ArgumentNullException(nameof(schedule));

            if (await IsScheduleUniqueAsync(schedule.RouteId, schedule.DepartureTime))
            {
                await _context.Schedules.AddAsync(schedule);
                await _context.SaveChangesAsync();
                return schedule;
            }
            else
            {
                throw new InvalidOperationException("Schedule with this departure time already exists for this route.");
            }
        }

        public async Task<Schedule> UpdateScheduleAsync(Schedule schedule)
        {
            if (schedule == null)
                throw new ArgumentNullException(nameof(schedule));

            var existingSchedule = await _context.Schedules.FindAsync(schedule.Id);
            if (existingSchedule == null)
                throw new ArgumentException($"Schedule with ID {schedule.Id} not found.");

            existingSchedule.RouteId = schedule.RouteId;
            existingSchedule.TrainId = schedule.TrainId;
            existingSchedule.DepartureTime = schedule.DepartureTime;
            existingSchedule.IsActive = schedule.IsActive;
            existingSchedule.ValidFrom = schedule.ValidFrom;
            existingSchedule.ValidTo = schedule.ValidTo;
            existingSchedule.Notes = schedule.Notes;

            _context.Schedules.Update(existingSchedule);
            await _context.SaveChangesAsync();
            return existingSchedule;
        }

        public async Task<bool> DeleteScheduleAsync(int id)
        {
            var schedule = await _context.Schedules
                .Include(s => s.TravelTimes)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (schedule == null)
                return false;

            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Schedule>> GetSchedulesByRouteAsync(int routeId)
        {
            return await _context.Schedules
                .Where(s => s.RouteId == routeId)
                .Include(s => s.Train)
                .Include(s => s.TravelTimes)
                    .ThenInclude(tt => tt.RouteStation)
                        .ThenInclude(rs => rs.Station)
                .OrderBy(s => s.DepartureTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Schedule>> GetSchedulesByTrainAsync(int trainId)
        {
            return await _context.Schedules
                .Where(s => s.TrainId == trainId)
                .Include(s => s.Route)
                .Include(s => s.TravelTimes)
                .OrderBy(s => s.DepartureTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Schedule>> GetSchedulesByDepartureTimeAsync(TimeSpan from, TimeSpan to)
        {
            return await _context.Schedules
                .Where(s => s.DepartureTime >= from && s.DepartureTime <= to)
                .Include(s => s.Route)
                .Include(s => s.Train)
                .OrderBy(s => s.DepartureTime)
                .ToListAsync();
        }

        public async Task<bool> IsScheduleUniqueAsync(int routeId, TimeSpan departureTime)
        {
            return !await _context.Schedules
                .AnyAsync(s => s.RouteId == routeId && s.DepartureTime == departureTime);
        }
    }
}