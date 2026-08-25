using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TTD.Core.Interfaces;
using TTD.Data.Models;

namespace TTD.Main.ConsoleTools
{
    public static class ExportData
    {
        public static async Task Execute(ITrainService trainService, IStationService stationService, IRouteService routeService)
        {
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

                // Eksport pociągów
                var trains = await trainService.GetAllTrainsAsync();
                await ExportToCsvAsync(trains, Path.Combine(desktopPath, $"trains_{timestamp}.csv"));

                // Eksport stacji
                var stations = await stationService.GetAllStationsAsync();
                await ExportToCsvAsync(stations, Path.Combine(desktopPath, $"stations_{timestamp}.csv"));

                // Eksport tras
                var routes = await routeService.GetAllRoutesAsync();
                await ExportToCsvAsync(routes, Path.Combine(desktopPath, $"routes_{timestamp}.csv"));

                Console.WriteLine($"   ✅ Eksport zakończony! Pliki zapisane na pulpicie.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Błąd eksportu: {ex.Message}");
            }
        }

        private static async Task ExportToCsvAsync<T>(IEnumerable<T> data, string filePath)
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
    }
}