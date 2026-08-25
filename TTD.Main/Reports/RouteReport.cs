using System;
using System.Linq;
using System.Threading.Tasks;
using TTD.Core.Interfaces;

namespace OpenTTDManager.TTD.Main.Reports
{
    public static class RouteReport
    {
        public static async Task Execute(IRouteService routeService)
        {
            var routes = await routeService.GetAllRoutesAsync();
            
            Console.WriteLine("   📋 LISTA TRAS:");
            if (!routes.Any())
            {
                Console.WriteLine("   ⚠️ Brak tras w bazie.");
                return;
            }

            foreach (var route in routes)
            {
                var stops = route.RouteStations?.OrderBy(rs => rs.StopOrder).ToList() ?? new();
                string stopNames = string.Join(" → ", stops.Select(s => s.Station?.Name ?? "?"));
                Console.WriteLine($"   - {route.Name} (Aktywna: {(route.IsActive ? "Tak" : "Nie")}, " +
                                  $"Stacje: {stopNames})");
            }
        }
    }
}