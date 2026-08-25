using System;
using System.Linq;
using System.Threading.Tasks;
using TTD.Core.Interfaces;

namespace OpenTTDManager.TTD.Main.Reports
{
    public static class ScheduleReport
    {
        public static async Task Execute(IScheduleService scheduleService)
        {
            var schedules = await scheduleService.GetAllSchedulesAsync();
            
            Console.WriteLine("   📋 ROZKŁADY JAZDY:");
            if (!schedules.Any())
            {
                Console.WriteLine("   ⚠️ Brak kursów w bazie.");
                return;
            }

            foreach (var schedule in schedules.Take(10)) // pokaż tylko 10 pierwszych
            {
                Console.WriteLine($"   - {schedule.DepartureTime:hh\\:mm} - " +
                                  $"{schedule.Train?.Name ?? "Brak pociągu"} " +
                                  $"({schedule.Route?.Name ?? "Brak trasy"}) " +
                                  $"{(schedule.IsActive ? "✅" : "❌")} " +
                                  $"{(!string.IsNullOrEmpty(schedule.Notes) ? $"({schedule.Notes})" : "")}");
            }

            if (schedules.Count() > 10)
                Console.WriteLine($"   ... i {schedules.Count() - 10} więcej.");
        }
    }
}