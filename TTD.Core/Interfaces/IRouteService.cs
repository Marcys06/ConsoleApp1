using System.Collections.Generic;
using System.Threading.Tasks;
using TTD.Data.Models;

namespace TTD.Core.Interfaces
{
    public interface IRouteService
    {
        Task<IEnumerable<Route>> GetAllRoutesAsync();
        Task<Route?> GetRouteByIdAsync(int id);
        Task<Route> AddRouteAsync(Route route);
        Task<Route> UpdateRouteAsync(Route route);
        Task<bool> DeleteRouteAsync(int id);
        Task<IEnumerable<Route>> GetActiveRoutesAsync();
        Task<IEnumerable<Route>> GetRoutesByStationAsync(int stationId);
        Task<bool> RouteExistsAsync(int id);
    }
}