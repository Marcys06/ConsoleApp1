using System;
using System.IO;
using System.Threading.Tasks;
using TTD.Core.Interfaces;

namespace OpenTTDManager.TTD.Main.Reports
{
    public static class FullReport
    {
        public static async Task Execute(ITrainService trainService, IRouteService routeService, IScheduleService scheduleService)
        {
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string reportPath = Path.Combine(desktopPath, $"full_report_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

                using var writer = new StreamWriter(reportPath);
                
                writer.WriteLine("╔═══════════════════════════════════════════════════════╗");
                writer.WriteLine("║         OPEN TTD MANAGER - PEŁNY RAPORT              ║");
                writer.WriteLine("╚═══════════════════════════════════════════════════════╝");
                writer.WriteLine($"Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                writer.WriteLine();

                // Pociągi
                var trains = await trainService.GetAllTrainsAsync();
                writer.WriteLine("🚂 POCIĄGI:");
                foreach (var train in trains)
                {
                    writer.WriteLine($"   - {train.Name} (Model: {train.Model}, Vmax: {train.VMax} km/h)");
                }
                writer.WriteLine();

                // Trasy
                var routes = await routeService.GetAllRoutesAsync();
                writer.WriteLine("🛤️ TRASY:");
                foreach (var route in routes)
                {
                    writer.WriteLine($"   - {route.Name} (Aktywna: {(route.IsActive ? "Tak" : "Nie")})");
                }
                writer.WriteLine();

                // Kursy
                var schedules = await scheduleService.GetAllSchedulesAsync();
                writer.WriteLine("🕐 KURSY:");
                foreach (var schedule in schedules)
                {
                    writer.WriteLine($"   - {schedule.DepartureTime:hh\\:mm} - {schedule.Train?.Name} ({schedule.Route?.Name})");
                }

                Console.WriteLine($"   ✅ Raport zapisany: {reportPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Błąd generowania raportu: {ex.Message}");
            }
        }
    }
}