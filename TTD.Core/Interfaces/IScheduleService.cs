using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TTD.Data.Models;

namespace TTD.Core.Interfaces
{
    public interface IScheduleService
    {
        Task<IEnumerable<Schedule>> GetAllSchedulesAsync();
        Task<Schedule?> GetScheduleByIdAsync(int id);
        Task<Schedule> AddScheduleAsync(Schedule schedule);
        Task<Schedule> UpdateScheduleAsync(Schedule schedule);
        Task<bool> DeleteScheduleAsync(int id);
        Task<IEnumerable<Schedule>> GetSchedulesByRouteAsync(int routeId);
        Task<IEnumerable<Schedule>> GetSchedulesByTrainAsync(int trainId);
        Task<IEnumerable<Schedule>> GetSchedulesByDepartureTimeAsync(TimeSpan from, TimeSpan to);
        Task<bool> IsScheduleUniqueAsync(int routeId, TimeSpan departureTime);
    }
}