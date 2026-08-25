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
    public class TrainService : ITrainService
    {
        private readonly AppDbContext _context;

        public TrainService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Train>> GetAllTrainsAsync()
        {
            return await _context.Trains
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<Train?> GetTrainByIdAsync(int id)
        {
            return await _context.Trains
                .Include(t => t.Schedules)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Train> AddTrainAsync(Train train)
        {
            if (train == null)
                throw new ArgumentNullException(nameof(train));

            await _context.Trains.AddAsync(train);
            await _context.SaveChangesAsync();
            return train;
        }

        public async Task<Train> UpdateTrainAsync(Train train)
        {
            if (train == null)
                throw new ArgumentNullException(nameof(train));

            var existingTrain = await _context.Trains.FindAsync(train.Id);
            if (existingTrain == null)
                throw new ArgumentException($"Train with ID {train.Id} not found.");

            existingTrain.Name = train.Name;
            existingTrain.Model = train.Model;
            existingTrain.VMax = train.VMax;
            existingTrain.Power = train.Power;
            existingTrain.Weight = train.Weight;
            existingTrain.TractiveEffort = train.TractiveEffort;
            existingTrain.ModelYear = train.ModelYear;
            existingTrain.IsElectric = train.IsElectric;
            existingTrain.ImagePath = train.ImagePath;

            _context.Trains.Update(existingTrain);
            await _context.SaveChangesAsync();
            return existingTrain;
        }

        public async Task<bool> DeleteTrainAsync(int id)
        {
            var train = await _context.Trains
                .Include(t => t.Schedules)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (train == null)
                return false;

            if (train.Schedules != null && train.Schedules.Any())
                throw new InvalidOperationException("Cannot delete train with assigned schedules.");

            _context.Trains.Remove(train);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Train>> SearchTrainsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllTrainsAsync();

            return await _context.Trains
                .Where(t => t.Name.Contains(searchTerm) ||
                           t.Model.Contains(searchTerm))
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Train>> GetTrainsByVMaxAsync(int minVMax, int maxVMax)
        {
            return await _context.Trains
                .Where(t => t.VMax >= minVMax && t.VMax <= maxVMax)
                .OrderBy(t => t.VMax)
                .ToListAsync();
        }

        public async Task<bool> TrainExistsAsync(int id)
        {
            return await _context.Trains.AnyAsync(t => t.Id == id);
        }
    }
}