using System;
using System.IO;
using System.Linq;
using ConsoleApp1.TTD.Data;
using Microsoft.EntityFrameworkCore;

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

            // ===== KONFIGURACJA ŚCIEŻKI =====
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ttd_database.db");
            Console.WriteLine($"📁 Ścieżka bazy danych: {dbPath}");
            Console.WriteLine("");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite($"Data Source={dbPath}");

            using (var dbContext = new AppDbContext(optionsBuilder.Options))
            {
                try
                {
                    // ===== TWORZENIE BAZY DANYCH I TABEL =====
                    Console.WriteLine("Tworzenie bazy danych i tabel...");

                    // ENSURE CREATED - tworzy tabele na podstawie modeli (BEZ migracji)
                    bool created = dbContext.Database.EnsureCreated();

                    if (created)
                        Console.WriteLine("✅ Baza danych i tabele zostały utworzone.");
                    else
                        Console.WriteLine("✅ Baza danych już istniała.");

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