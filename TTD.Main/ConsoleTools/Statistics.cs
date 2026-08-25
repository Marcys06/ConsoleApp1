using System;
using System.Linq;
using TTD.Data;

namespace TTD.Main.ConsoleTools
{
    public static class Statistics
    {
        public static void Execute(AppDbContext dbContext)
        {
            Console.WriteLine("   📊 STATYSTYKI BAZY DANYCH:");
            Console.WriteLine($"   🚂 Pociągi: {dbContext.Trains.Count()}");
            Console.WriteLine($"   🏢 Stacje: {dbContext.Stations.Count()}");
            Console.WriteLine($"   🛤️ Trasy: {dbContext.Routes.Count()}");
            Console.WriteLine($"   🕐 Kursy: {dbContext.Schedules.Count()}");
            Console.WriteLine($"   ⏱️ Czasy przejazdu: {dbContext.ScheduleTravelTimes.Count()}");
            
            if (dbContext.Routes.Any())
            {
                Console.WriteLine($"\n   📋 Trasy: {string.Join(", ", dbContext.Routes.Select(r => r.Name).Take(5))}");
            }
        }
    }
}