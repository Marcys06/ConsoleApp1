using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TTD.Data;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== OpenTTD Manager ===");
            Console.WriteLine($"Platforma: .NET {Environment.Version}");
            Console.WriteLine("Aplikacja do zarządzania rozkładami jazdy");
            Console.WriteLine("");

            // ===== KONFIGURACJA Z PRAWIDŁOWĄ ŚCIEŻKĄ =====
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ttd_database.db");
            Console.WriteLine($"📁 Ścieżka bazy danych: {dbPath}");
            Console.WriteLine("");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite($"Data Source={dbPath}");

            using (var dbContext = new AppDbContext(optionsBuilder.Options))
            {
                try
                {
                    // ===== AUTOMATYCZNA MIGRACJA (TWORZY BAZĘ JAK NIE ISTNIEJE) =====
                    Console.WriteLine("Sprawdzanie bazy danych...");
                    dbContext.Database.Migrate();
                    Console.WriteLine("✅ Baza danych jest gotowa.");
                    Console.WriteLine("");

                    // ===== TEST POŁĄCZENIA =====
                    bool canConnect = dbContext.Database.CanConnect();

                    if (canConnect)
                    {
                        Console.WriteLine("✅ Połączenie z bazą danych działa.");
                        Console.WriteLine($"   Dostawca: {dbContext.Database.ProviderName ?? "Nieznany"}");
                        Console.WriteLine("");
                        Console.WriteLine("Tabele w bazie danych:");
                        Console.WriteLine($"  - Trains: {(dbContext.Trains?.Any() == true ? "✅" : "⚠️ Pusta (brak danych)")}");
                        Console.WriteLine($"  - Stations: {(dbContext.Stations?.Any() == true ? "✅" : "⚠️ Pusta (brak danych)")}");
                        Console.WriteLine($"  - Routes: {(dbContext.Routes?.Any() == true ? "✅" : "⚠️ Pusta (brak danych)")}");
                        Console.WriteLine($"  - RouteStations: {(dbContext.RouteStations?.Any() == true ? "✅" : "⚠️ Pusta (brak danych)")}");
                        Console.WriteLine($"  - Schedules: {(dbContext.Schedules?.Any() == true ? "✅" : "⚠️ Pusta (brak danych)")}");
                        Console.WriteLine($"  - ScheduleTravelTimes: {(dbContext.ScheduleTravelTimes?.Any() == true ? "✅" : "⚠️ Pusta (brak danych)")}");
                    }
                    else
                    {
                        Console.WriteLine("❌ Nie można połączyć się z bazą danych.");
                        Console.WriteLine("   Sprawdź connection string.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Błąd: {ex.Message}");
                    Console.WriteLine($"   Szczegóły: {ex.InnerException?.Message ?? "Brak"}");
                }
            }

            Console.WriteLine("");
            Console.WriteLine("Naciśnij dowolny klawisz, aby zakończyć...");
            Console.ReadKey();
        }
    }
}