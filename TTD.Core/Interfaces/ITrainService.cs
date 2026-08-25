using System.Collections.Generic;
using System.Threading.Tasks;
using TTD.Data.Models;

namespace TTD.Core.Interfaces
{
    public interface ITrainService
    {
        Task<IEnumerable<Train>> GetAllTrainsAsync();
        Task<Train?> GetTrainByIdAsync(int id);
        Task<Train> AddTrainAsync(Train train);
        Task<Train> UpdateTrainAsync(Train train);
        Task<bool> DeleteTrainAsync(int id);
        Task<IEnumerable<Train>> SearchTrainsAsync(string searchTerm);
        Task<IEnumerable<Train>> GetTrainsByVMaxAsync(int minVMax, int maxVMax);
        Task<bool> TrainExistsAsync(int id);
    }
}