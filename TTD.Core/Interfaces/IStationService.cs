using System.Collections.Generic;
using System.Threading.Tasks;
using TTD.Data.Models;

namespace TTD.Core.Interfaces
{
    public interface IStationService
    {
        Task<IEnumerable<Station>> GetAllStationsAsync();
        Task<Station?> GetStationByIdAsync(int id);
        Task<Station> AddStationAsync(Station station);
        Task<Station> UpdateStationAsync(Station station);
        Task<bool> DeleteStationAsync(int id);
        Task<IEnumerable<Station>> SearchStationsAsync(string searchTerm);
        Task<IEnumerable<Station>> GetStationsByTypeAsync(bool isPassenger, bool isCargo);
        Task<bool> StationExistsAsync(int id);
    }
}